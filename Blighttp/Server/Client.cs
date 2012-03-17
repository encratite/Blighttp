using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

using Nil;

namespace Blighttp
{
	public class Client
	{
		const int ReceiveBufferSize = 0x1000;
		const int MaximumBufferSize = 0x10000;
		const int ReceiveTimeout = 5000;

		const string Separator = "\r\n";
		const string EndOfHeader = Separator + Separator;

		WebServer ClientServer;
		Socket ClientSocket;
		string Buffer;

		public Client(WebServer server, Socket socket)
		{
			ClientServer = server;
			ClientSocket = socket;
			ClientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, ReceiveTimeout);
			Buffer = "";
		}

		bool Read()
		{
			byte[] ReceiveBuffer = new byte[ReceiveBufferSize];
			try
			{
				int bytesRead = ClientSocket.Receive(ReceiveBuffer, SocketFlags.None);
				Buffer += Encoding.UTF8.GetString(ReceiveBuffer, 0, bytesRead);
				return bytesRead > 0;
			}
			catch (SocketException)
			{
				return false;
			}
		}

		void WriteLine(string message, params object[] arguments)
		{
			Output.WriteLine(string.Format("[{0}] {1}", GetAddress(), message), arguments);
		}

		string GetAddress()
		{
			IPEndPoint endPoint = (IPEndPoint)ClientSocket.RemoteEndPoint;
			return endPoint.Address.ToString();
		}

		void Terminate(string reason = null)
		{
			if (reason != null)
				WriteLine(reason);
			ClientSocket.Close();
			ClientServer.RemoveClient(this);
		}

		Request ProcessHeader(string header)
		{
			List<string> lines = header.Tokenise(Separator);
			if (lines.Count == 0)
				throw new ClientException("Invalid number of entries in header");
			List<string> requestTokens = lines[0].Tokenise(" ");
			if (requestTokens.Count != 3)
				throw new ClientException("Invalid number of tokens in request line");
			string requestTypeString = requestTokens[0];
			string path = requestTokens[1];
			string versionString = requestTokens[2];
			RequestType type;
			switch (requestTypeString)
			{
				case "GET":
					type = RequestType.Get;
					break;

				case "POST":
					type = RequestType.Post;
					break;

				default:
					throw new ClientException("Unknown request type");
			}
			double httpVersion;
			switch (versionString)
			{
				case "HTTP/1.0":
					httpVersion = 1.0;
					break;

				case "HTTP/1.1":
					httpVersion = 1.1;
					break;

				default:
					throw new ClientException("Unknown protocol version specified");
			}
			Dictionary<string, string> headers = new Dictionary<string, string>();
			foreach (var line in lines.GetRange(1, lines.Count - 1))
			{
				const string target = ":";
				int offset = line.IndexOf(target);
				if (offset == -1)
					throw new ClientException("Invalid line in header");
				string key = line.Substring(0, offset);
				string value = line.Substring(offset + target.Length);
				headers[key] = value.Trim();
			}
			Request request = new Request(GetAddress(), type, path, httpVersion, headers);
			return request;
		}

		public void Run()
		{
			Request request;
			while (true)
			{
				bool connectionActive = Read();
				if (!connectionActive)
				{
					Terminate("Disconnected while reading headers");
					return;
				}
				if (Buffer.Length > MaximumBufferSize)
				{
					Terminate("Maximum buffer size exceeded");
					return;
				}

				int offset = Buffer.IndexOf(EndOfHeader);
				if (offset > 0)
				{
					//Header has been fully read
					try
					{
						string header = Buffer.Substring(0, offset);
						Buffer = Buffer.Remove(0, offset + EndOfHeader.Length);
						request = ProcessHeader(header);
					}
					catch(ClientException exception)
					{
						Terminate(exception.Message);
						return;
					}
					break;
				}

				//Header still not available, continue reading
			}

			if (request.ContentLength.HasValue)
			{
				//The request might have a body
				if (request.ContentLength > MaximumBufferSize)
				{
					Terminate("Body exceeds maximum buffer size");
					return;
				}
				while (Buffer.Length < request.ContentLength)
				{
					bool connectionActive = Read();
					if (!connectionActive)
					{
						Terminate("Disconnected while reading body");
						return;
					}
				}

				string type;
				if (request.Headers.TryGetValue("Content-Type", out type))
				{
					if (type == "application/x-www-form-urlencoded")
					{
						string content = Buffer;
						request.ProcessContent(content);
					}
					else
						throw new ClientException("Unknown content encoding");
				}
			}

			Reply reply = ClientServer.HandleRequest(request);
			ClientSocket.Send(reply.GetData());

			Terminate();
		}
	}
}
