using System;

using Blighttp;

namespace Test
{
	class Program
	{
		static Reply MarkupTest(Request request)
		{
			Document document = new Document("<Test>");
			string body = Markup.Bold("Success!");
			Reply reply = new Reply(ReplyCode.Ok, ContentType.Markup, document.Render(body));
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
