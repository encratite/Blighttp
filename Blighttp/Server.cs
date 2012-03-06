using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Blighttp
{
	public class Server
	{
		string Host;
		int Port;

		Socket ServerSocket;

		HashSet<Client> Clients;

		public Server(string host, int port)
		{
			Host = host;
			Port = port;
			ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
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
	}
}
