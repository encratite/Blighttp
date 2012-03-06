using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Nil;

namespace Blighttp
{
	class Client
	{
		const int ReceiveBufferSize = 0x1000;
		const int MaximumBufferSize = 0x10000;
		const int ReceiveTimeout = 5000;

		const string Separator = "\r\n";
		const string EndOfHeader = Separator + Separator;

		Server ClientServer;
		Socket ClientSocket;
		string Buffer;

		public Client(Server server, Socket socket)
		{
			ClientServer = server;
			ClientSocket = socket;
			ClientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, ReceiveTimeout);
			Buffer = "";
		}

		bool Read()
		{
			byte[] ReceiveBuffer = new byte[ReceiveBufferSize];
			int bytesRead = ClientSocket.Receive(ReceiveBuffer, SocketFlags.None);
			Buffer += ReceiveBuffer.ToString();
			return bytesRead > 0;
		}

		void WriteLine(string message, params object[] arguments)
		{
			IPEndPoint endPoint = (IPEndPoint)ClientSocket.RemoteEndPoint;
			Output.WriteLine(string.Format("[{0}] {1}", endPoint.Address, message), arguments);
		}

		void Terminate(string reason)
		{
			WriteLine(reason);
			ClientSocket.Close();
			ClientServer.RemoveClient(this);
		}

		public void Run()
		{
			WriteLine("Connected");
			while (true)
			{
				bool connectionActive = Read();
				if (!connectionActive)
				{
					Terminate("Disconnected");
					return;
				}
				if (Buffer.Length >= MaximumBufferSize)
				{
					Terminate("Maximum buffer size exceeded");
					return;
				}

				int offset = Buffer.IndexOf(EndOfHeader);
				if (offset == -1)
				{
					//Header still not available, continue reading
					continue;
				}

				//Header has been fully read
				string header = Buffer.Substring(0, offset);
				Buffer.Remove(0, offset + EndOfHeader.Length);
				break;
			}
		}
	}
}
