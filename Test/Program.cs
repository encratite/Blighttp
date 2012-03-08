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
			Head head = new Head("Test");
			Body body = new Body();
			Document document = new Document(head, body);
			body.Add(new Bold("Success!"));
			Reply reply = new Reply(ReplyCode.Ok, ContentType.Markup, document);
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
