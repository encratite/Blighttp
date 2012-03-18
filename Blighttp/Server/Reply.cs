
using System;
using System.Collections.Generic;
using System.Text;

namespace Blighttp
{
	public enum ReplyCode
	{
		Ok,
		Found,
		BadRequest,
		Forbidden,
		NotFound,
		InternalServerError,
	};

	public enum ContentType
	{
		JSON,
		Plain,
		Markup,
	};

	public class Reply
	{
		static Dictionary<ReplyCode, ReplyCodeData> NumericReplyCodes = new Dictionary<ReplyCode, ReplyCodeData>()
		{
			{ReplyCode.Ok, new ReplyCodeData(200, "OK")},
			{ReplyCode.Found, new ReplyCodeData(302, "Found")},
			{ReplyCode.BadRequest, new ReplyCodeData(400, "Bad request")},
			{ReplyCode.Forbidden, new ReplyCodeData(403, "Forbidden")},
			{ReplyCode.NotFound, new ReplyCodeData(404, "Not found")},
			{ReplyCode.InternalServerError, new ReplyCodeData(500, "Internal server error")},
		};

		static Dictionary<ContentType, string> ContentTypeStrings = new Dictionary<ContentType, string>()
		{
			{ContentType.JSON, "application/json"},
			{ContentType.Plain, "text/plain"},
			{ContentType.Markup, "text/html"},
		};

		public bool IsChunked
		{
			get
			{
				return IsChunkedHandler;
			}
		}

		ReplyCode Code;
		ContentType Type;

		bool IsChunkedHandler;

		//For non-chunked transfers
		string Body;

		bool IsReferral;
		string Location;

		//For chunked transfers
		ChunkedHandlerDelegateType ChunkedHandlerDelegate;

		UTF8Encoding Encoding = new UTF8Encoding();

		//Emtpty constructor for named construction
		public Reply()
		{
		}

		//Constructor for non-chunked replies
		public Reply(ReplyCode code, ContentType type, string body)
		{
			Initialise(code, type);
			Body = body;
		}

		//More convenient constructor non-chunked 200 OK replies
		public Reply(string body)
		{
			Initialise(ReplyCode.Ok, ContentType.Markup);
			Body = body;
		}

		//Constructor for chunked replies
		public Reply(ReplyCode code, ContentType type, ChunkedHandlerDelegateType chunkedHandlerDelegate)
		{
			Initialise(code, type);
			IsChunkedHandler = true;
			ChunkedHandlerDelegate = chunkedHandlerDelegate;
		}

		void Initialise(ReplyCode code, ContentType type)
		{
			Code = code;
			Type = type;
			IsChunkedHandler = false;
			ChunkedHandlerDelegate = null;
			IsReferral = false;
			Encoding = new UTF8Encoding();
		}

		public static Reply Referral(string uri)
		{
			Reply reply = new Reply();
			reply.Initialise(ReplyCode.Found, ContentType.Plain);
			reply.IsReferral = true;
			reply.Location = uri;
			reply.Body = "";
			return reply;
		}

		string GetCommonHeader()
		{
			ReplyCodeData codeData = NumericReplyCodes[Code];
			string header = string.Format("HTTP/1.1 {0} {1}\r\n", codeData.Code, codeData.Description);
			header += string.Format("Content-Type: {0}\r\n", ContentTypeStrings[Type]);
			return header;
		}

		public byte[] GetChunkedHeader()
		{
			string header = GetCommonHeader();
			header += "Transfer-Encoding: chunked\r\n\r\n";
			byte[] headerBytes = Encoding.GetBytes(header);
			return headerBytes;
		}

		byte[] MergeHeaderAndBody(byte[] header, byte[] body)
		{
			byte[] output = new byte[body.Length + header.Length];
			Buffer.BlockCopy(header, 0, output, 0, header.Length);
			Buffer.BlockCopy(body, 0, output, header.Length, body.Length);

			return output;
		}

		public byte[] GetChunkedData(Request request, out bool WasLastChunk)
		{
			string chunkBody = ChunkedHandlerDelegate(request);
			if (chunkBody == null)
			{
				WasLastChunk = true;
				return Encoding.GetBytes("0\r\n");
			}
			else
			{
				WasLastChunk = false;
				const string tail = "\r\n";
				byte[] chunkBodyBytes = Encoding.GetBytes(chunkBody + tail);
				int chunkBodySize = chunkBodyBytes.Length - tail.Length;
				string chunkHeader = string.Format("{0:X}\r\n", chunkBodySize);
				byte[] chunkHeaderBytes = Encoding.GetBytes(chunkHeader);
				return MergeHeaderAndBody(chunkHeaderBytes, chunkBodyBytes);
			}

		}

		public byte[] GetData()
		{
			byte[] bodyBytes = Encoding.GetBytes(Body);
			string header = GetCommonHeader();
			if (IsReferral)
				header += string.Format("Location: {0}\r\n", Location);
			else
				header += string.Format("Content-Length: {0}\r\n", bodyBytes.Length);
			header += "\r\n";
			byte[] headerBytes = Encoding.GetBytes(header);
			return MergeHeaderAndBody(headerBytes, bodyBytes);
		}
	}
}
