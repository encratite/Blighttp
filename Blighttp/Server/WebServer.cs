using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web;

using Nil;

namespace Blighttp
{
	public delegate void RequestObserver(Request request);

	public class WebServer
	{
		string Host;
		int Port;

		Socket ServerSocket;

		List<Handler> Handlers;


		RequestObserver Observer;

		public WebServer(string host, int port, RequestObserver observer = null)
		{
			Host = host;
			Port = port;
			Observer = observer;
			ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			Handlers = new List<Handler>();
		}

		void BindAndListen(int listenCount = -1)
		{
			IPAddress address = Dns.GetHostAddresses(Host)[0];
			IPEndPoint endPoint = new IPEndPoint(address, Port);
			ServerSocket.Bind(endPoint);
			ServerSocket.Listen(listenCount);
		}

		public void Run()
		{
			BindAndListen();

			while (true)
			{
				Socket clientSocket = ServerSocket.Accept();
				Client client = new Client(this, clientSocket);
			}
		}

		List<string> ConvertPath(string path)
		{
			List<string> tokens = path.Tokenise("/");
			var output = from x in tokens where x.Length > 0 select HttpUtility.UrlDecode(x);
			return output.ToList();
		}

		public Reply HandleRequest(Request request)
		{
			if (Observer != null)
				Observer(request);

			try
			{
				List<string> remainingPath = ConvertPath(request.Path);
				foreach (var handler in Handlers)
				{
					Reply reply = handler.RouteRequest(request, remainingPath);
					if (reply != null)
						return reply;
				}
			}
			catch (HandlerException exception)
			{
				Reply exceptionReply = new Reply(ReplyCode.BadRequest, ContentType.Plain, exception.Message);
				return exceptionReply;
			}
			catch (Exception exception)
			{
				if (Debugger.IsAttached)
				{
					//While a debugger is attached, it's more convenient to go right to the source of an exception
					throw;
				}
				else
				{
					//Print the exception to the server console but don't leak any information to the client
					string message = string.Format("An exception of type {0} occurred at {1}: {2}\n", exception.GetType().ToString(), exception.Source, exception.Message);
					foreach (var trace in exception.StackTrace)
						message += string.Format("{0}\n", trace);
					Output.Write(message);
					Reply exceptionReply = new Reply(ReplyCode.InternalServerError, ContentType.Plain, "Internal server error");
					return exceptionReply;
				}
			}
			Reply invalidReply = new Reply(ReplyCode.NotFound, ContentType.Plain, "Invalid path");
			return invalidReply;
		}

		public void Add(Handler handler)
		{
			Handlers.Add(handler);
		}
	}
}
