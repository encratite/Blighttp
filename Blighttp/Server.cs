using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

using Nil;

namespace Blighttp
{
	public class Server
	{
		string Host;
		int Port;

		Socket ServerSocket;

		List<Handler> Handlers;

		HashSet<Client> Clients;

		public Server(string host, int port)
		{
			Host = host;
			Port = port;
			ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			Handlers = new List<Handler>();
			Clients = new HashSet<Client>();
		}

		void BindAndListen()
		{
			IPAddress address = Dns.GetHostEntry(Host).AddressList[0];
			IPEndPoint endPoint = new IPEndPoint(address, Port);
			ServerSocket.Bind(endPoint);
			ServerSocket.Listen(-1);
		}

		public void Run()
		{
			BindAndListen();

			Console.WriteLine("Running on {0}:{1}", Host, Port);

			while (true)
			{
				Socket clientSocket = ServerSocket.Accept();
				Client client = new Client(this, clientSocket);
				lock (Clients)
					Clients.Add(client);
				client.Run();
			}
		}

		public void RemoveClient(Client client)
		{
			lock (Clients)
				Clients.Remove(client);
		}

		List<string> ConvertPath(string path)
		{
			List<string> tokens = path.Tokenise("/");
			var output = from x in tokens where x.Length > 0 select x;
			if (output.Count() == 0)
				throw new HandlerException("Encountered an empty path after tokenisation");
			return output.ToList();
		}

		public Reply HandleRequest(Request request)
		{
			List<string> remainingPath = ConvertPath(request.Path);
			foreach (var handler in Handlers)
			{
				Reply reply = handler.RouteRequest(request, remainingPath);
				if (reply != null)
					return reply;
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
