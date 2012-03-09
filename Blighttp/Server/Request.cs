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
		public RequestType Type;
		public string Path;
		public double Version;
		public Dictionary<string, string> Headers;
		public int? ContentLength;
		public Dictionary<string, string> Content;

		//Set by the Handler
		public List<object> Arguments;
		public Handler RequestHandler;

		public Request(RequestType type, string path, double version, Dictionary<string, string> headers)
		{
			Type = type;
			Path = path;
			Version = version;
			Headers = headers;
			const string ContentLengthKey = "Content-Length";
			if (Headers.ContainsKey(ContentLengthKey))
			{
				try
				{
					ContentLength = Convert.ToInt32(Headers[ContentLengthKey]);
				}
				catch (FormatException)
				{
					throw new ClientException("Invalid content length specified");
				}
			}
			else
				ContentLength = null;

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
