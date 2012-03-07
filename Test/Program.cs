using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Blighttp;

namespace Test
{
	class Program
	{
		static Reply Test(Request request)
		{
			string body = "<b>Success!</b>";
			Reply reply = new Reply(ReplyCode.Ok, ContentType.Markup, body);
			return reply;
		}

		static void Main(string[] arguments)
		{
			Server server = new Server("127.0.0.1", 9000);
			Handler handler = new Handler("blight", Test);
			server.Add(handler);
			server.Run();
		}
	}
}
