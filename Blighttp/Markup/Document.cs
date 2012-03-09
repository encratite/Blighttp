using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blighttp
{
	public class Document
	{
		public string Title;
		public string Icon;
		public string Stylesheet;

		public Document()
		{
		}

		public Document(string title)
		{
			Title = title;
		}

		public Document(string icon, string stylesheet)
		{
			Icon = icon;
			Stylesheet = stylesheet;
		}

		public string Render(string bodyContent)
		{
			string charset = Markup.Meta(Markup.GetAttributes("charset", "utf-8"));
			string title = Markup.Title(Title);
			string headContent = charset + title;
			if (Icon != null)
				headContent += Markup.MetaLink("icon", Markup.GetAttributes("type", "image/ico", "href", Icon));
			if (Stylesheet != null)
				headContent += Markup.MetaLink("stylesheet", Markup.GetAttributes("type", "text/css", "media", "screen", "href", Stylesheet));
			string head = Markup.Head(headContent);
			string body = Markup.Body(bodyContent);
			string output = Markup.Html(head + body);
			return output;
		}
	}
}
