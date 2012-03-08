using System;
using System.Collections.Generic;

namespace Blighttp
{
	class ReplyCodeData
	{
		public int Code;
		public string Description;

		public ReplyCodeData(int code, string description)
		{
			Code = code;
			Description = description;
		}
	};
}
