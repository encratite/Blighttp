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
	public class WebServer
	{
		bool Running;

		string Host;
		int Port;

		public readonly bool UseRealIP;

		IRequestObserver RequestObserver;
		ICatchAll CatchAll;

		Socket ServerSocket;

		List<Handler> Handlers;

		public WebServer(string host, int port, bool useRealIP = false, IRequestObserver requestObserver = null, ICatchAll catchAll = null)
		{
			Host = host;
			Port = port;
			UseRealIP = useRealIP;
			RequestObserver = requestObserver;
			CatchAll = catchAll;
			ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			Handlers = new List<Handler>();
		}

		void BindAndListen(int listenCount = -1)
		{
			IPAddress address;
			if (Host == null || Host == "")
				address = IPAddress.Any;
			else
				address = Dns.GetHostAddresses(Host)[0];
			IPEndPoint endPoint = new IPEndPoint(address, Port);
			ServerSocket.Bind(endPoint);
			ServerSocket.Listen(listenCount);
		}

		public void Run()
		{
			BindAndListen();

			Running = true;

			while (Running)
			{
				Socket clientSocket = ServerSocket.Accept();
				Client client = new Client(this, clientSocket);
				client.Run();
			}
		}

		public void Terminate()
		{
			Running = false;

			ServerSocket.Close();
		}

		List<string> ConvertPath(string path)
		{
			List<string> tokens = path.Tokenise("/");
			var output = from x in tokens where x.Length > 0 select HttpUtility.UrlDecode(x);
			return output.ToList();
		}

		Reply TryHandlers(Request request)
		{
			List<string> remainingPath = ConvertPath(request.Path);
			foreach (var handler in Handlers)
			{
				Reply reply = handler.RouteRequest(request, remainingPath);
				if (reply != null)
					return reply;
			}
			return null;
		}

		public Reply HandleRequest(Request request)
		{
			if (RequestObserver != null)
				RequestObserver.ObserveRequest(request);

			if (Debugger.IsAttached)
			{
				try
				{
					Reply reply = TryHandlers(request);
					if (reply != null)
						return reply;
				}
				catch (HandlerException exception)
				{
					Reply exceptionReply = new Reply(ReplyCode.BadRequest, ContentType.Plain, exception.Message);
					return exceptionReply;
				}
			}
			else
			{
				try
				{
					Reply reply = TryHandlers(request);
					if (reply != null)
						return reply;
				}
				catch (HandlerException exception)
				{
					Reply exceptionReply = new Reply(ReplyCode.BadRequest, ContentType.Plain, exception.Message);
					return exceptionReply;
				}
				catch (Exception exception)
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
			if (CatchAll == null)
			{
				Reply invalidReply = new Reply(ReplyCode.NotFound, ContentType.Plain, "Invalid path");
				return invalidReply;
			}
			else
				return CatchAll.CatchAll(request);
		}

		public void Add(Handler handler)
		{
			Handlers.Add(handler);
		}
	}
}
