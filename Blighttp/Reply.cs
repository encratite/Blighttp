using System.Collections.Generic;

namespace Blighttp
{
	public enum ReplyCode
	{
		Ok,
		Forbidden,
		NotFound,
		InternalServerError,
	};

	public enum ContentType
	{
		Plain,
		Markup,
	};

	public class Reply
	{
		static Dictionary<ReplyCode, int> NumericReplyCodes = new Dictionary<ReplyCode, int>()
		{
			{ReplyCode.Ok, 200},
			{ReplyCode.Forbidden, 403},
			{ReplyCode.NotFound, 404},
			{ReplyCode.InternalServerError, 500},
		};

		static Dictionary<ContentType, string> ContentTypeStrings = new Dictionary<ContentType, string>()
		{
			{ContentType.Plain, "text/plain"},
			{ContentType.Markup, "text/html"},
		};

		ReplyCode Code;
		ContentType Type;
		string Body;

		public Reply(ReplyCode code, ContentType type, string body)
		{
			Code = code;
			Type = type;
			Body = body;
		}

		public string GetData()
		{
			string data = string.Format("HTTP/1.1 {0} {1}\r\n", Code, NumericReplyCodes[Code]);
			data += string.Format("Content-Type: {0}\r\n", ContentTypeStrings[Type]);
			data += string.Format("Content-Length: {0}\r\n\r\n", Body.Length);
			data += Body;
			return data;
		}
	}
}
