using System.Collections.Generic;

namespace Blighttp
{
	public enum ReplyCode
	{
		Ok,
		BadRequest,
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
		static Dictionary<ReplyCode, ReplyCodeData> NumericReplyCodes = new Dictionary<ReplyCode, ReplyCodeData>()
		{
			{ReplyCode.Ok, new ReplyCodeData(200, "OK")},
			{ReplyCode.BadRequest, new ReplyCodeData(400, "Bad request")},
			{ReplyCode.Forbidden, new ReplyCodeData(403, "Forbidden")},
			{ReplyCode.NotFound, new ReplyCodeData(404, "Not found")},
			{ReplyCode.InternalServerError, new ReplyCodeData(500, "Internal server error")},
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

		public Reply(string body)
		{
			Code = ReplyCode.Ok;
			Type = ContentType.Markup;
			Body = body;
		}

		public string GetData()
		{
			ReplyCodeData codeData = NumericReplyCodes[Code];
			string data = string.Format("HTTP/1.1 {0} {1}\r\n", codeData.Code, codeData.Description);
			data += string.Format("Content-Type: {0}\r\n", ContentTypeStrings[Type]);
			data += string.Format("Content-Length: {0}\r\n\r\n", Body.Length);
			data += Body;
			return data;
		}
	}
}
