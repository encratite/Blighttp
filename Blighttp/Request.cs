using System;
using System.Collections.Generic;

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

		//Set by the Handler
		public List<object> Arguments;

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
		}

		public void ProcessBody(string body)
		{
			//Placeholder for the future, currently unused
		}
	}
}
