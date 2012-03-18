using System;
using System.Threading;

using Blighttp;

namespace Test
{
	class Program
	{
		static Handler SubmissionHandler;

		static Reply MarkupTest(Request request)
		{
			Document document = new Document("<Test>");
			string body = Markup.Paragraph(Markup.Bold("Success!"));
			body += Markup.Paragraph(string.Format("The path of this handler is: {0}", Markup.Bold(request.RequestHandler.GetPath())));
			string form = Markup.Text("input1") + Markup.Text("input2") + Markup.Submit("Submit");
			body += Markup.Form(SubmissionHandler.GetPath(), form);
			Reply reply = new Reply(document.Render(body));
			return reply;
		}

		static Reply SubmissionTest(Request request)
		{
			Document document = new Document("Submission");
			string body = "";
			foreach (var entry in request.Content)
				body += Markup.Paragraph(string.Format("{0}: {1}", entry.Key, entry.Value));
			Reply reply = new Reply(document.Render(body));
			return reply;
		}

		static Reply ExceptionTest(Request request)
		{
			throw new Exception("This is a test");
		}

		static string ChunkedTest(Request request)
		{
			Thread.Sleep(2000);
			return Markup.InlineScript("alert('Success');");
		}

		static void Main(string[] arguments)
		{
			WebServer server = new WebServer("127.0.0.1", 9000);

			Handler container = new Handler("blight");
			server.Add(container);

			Handler markupHandler = new Handler("markup", MarkupTest);
			container.Add(markupHandler);

			Handler exceptionHandler = new Handler("exception", ExceptionTest);
			container.Add(exceptionHandler);

			SubmissionHandler = new Handler("submission", SubmissionTest);
			container.Add(SubmissionHandler);

			Handler chunkedHandler = Handler.ChunkedHandler("chunked", ChunkedTest, ContentType.Markup);
			container.Add(chunkedHandler);

			server.Run();
		}
	}
}
