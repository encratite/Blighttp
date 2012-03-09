using System;

using Blighttp;

namespace Test
{
	class Program
	{
		static Reply MarkupTest(Request request)
		{
			Head head = new Head("<Test>");
			Body body = new Body();
			Document document = new Document(head, body);
			body.Add(new Bold("Success!"));
			Reply reply = new Reply(ReplyCode.Ok, ContentType.Markup, document);
			return reply;
		}

		static Reply ExceptionTest(Request request)
		{
			throw new Exception("This is a test");
		}

		static void Main(string[] arguments)
		{
			Server server = new Server("127.0.0.1", 9000);

			Handler container = new Handler("blight");
			server.Add(container);

			Handler markup = new Handler("markup", MarkupTest);
			container.Add(markup);

			Handler exception = new Handler("exception", ExceptionTest);
			container.Add(exception);

			server.Run();
		}
	}
}
