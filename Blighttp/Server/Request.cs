using System;
using System.Collections.Generic;
using System.Web;

using Nil;

namespace Blighttp
{
	public enum RequestType
	{
		Get,
		Post,
	};

	public class Request
	{
		public string ClientAddress;

		public RequestType Type;
		public string Path;
		public double Version;
		public Dictionary<string, string> Headers;
		public int? ContentLength;
		public Dictionary<string, string> Content;

		//Set by the Handler
		public List<object> Arguments;
		public Handler RequestHandler;

		public Request(string clientAddress, RequestType type, string path, double version, Dictionary<string, string> headers, bool useRealIP)
		{
			Type = type;
			Path = path;
			Version = version;
			Headers = headers;
			string lengthString;
			if (Headers.TryGetValue("Content-Length", out lengthString))
			{
				try
				{
					ContentLength = Convert.ToInt32(lengthString);
				}
				catch (FormatException)
				{
					throw new ClientException("Invalid content length specified");
				}
			}
			else
				ContentLength = null;

			if (useRealIP)
			{
				if (!Headers.TryGetValue("X-Real-IP", out ClientAddress))
					ClientAddress = clientAddress;
			}
			else
				ClientAddress = clientAddress;

			Content = new Dictionary<string, string>();

			//Arguments are null until set by a non-default Handler
			Arguments = null;

			RequestHandler = null;
		}

		public void ProcessContent(string content)
		{
			content = HttpUtility.UrlDecode(content);
			List<string> equations = content.Tokenise("&");
			foreach (var equation in equations)
			{
				List<string> tokens = equation.Tokenise("=");
				if (tokens.Count != 2)
					throw new ClientException("Invalid URL encoded data");
				string name = tokens[0];
				string value = tokens[1];
				Content[name] = value;
			}
		}
	}
}
