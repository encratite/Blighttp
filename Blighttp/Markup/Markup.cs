using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Blighttp
{
	public class Markup
	{
		public static string HtmlEncode(string input)
		{
			return HttpUtility.HtmlEncode(input);
		}

		public static string UrlEncode(string input)
		{
			string output = HttpUtility.UrlEncode(input);
			output = output.Replace("+", "%20");
			return output;
		}

		public static Dictionary<string, string> GetAttributes(params string[] pairs)
		{
			if (pairs.Length % 2 != 0)
				throw new Exception("Invalid pair arguments");
			Dictionary<string, string> output = new Dictionary<string, string>();
			for (int i = 0; i < pairs.Length; i += 2)
				output[pairs[i]] = pairs[i + 1];
			return output;
		}

		static string GetAttributeString(Dictionary<string, string> attributes)
		{
			string output = "";
			foreach (var pair in attributes)
			{
				if (pair.Value == null)
					continue;
				string value;
				if (pair.Key == "href" || pair.Key == "src")
					value = pair.Value;
				else
					value = HtmlEncode(pair.Value);
				output += string.Format(" {0}=\"{1}\"", pair.Key, value);
			}
			return output;
		}

		public static string Tag(string tag, Dictionary<string, string> attributes)
		{
			return string.Format("<{0}{1}>\n", tag, GetAttributeString(attributes));
		}

		public static string ClassTag(string tag, string style, string id, Dictionary<string, string> attributes)
		{
			attributes["class"] = style;
			attributes["id"] = id;
			return Tag(tag, attributes);
		}

		public static string ContentTag(string tag, string content, Dictionary<string, string> attributes = null, bool innerNewlines = true)
		{
			if (attributes == null)
				attributes = new Dictionary<string, string>();
			if (content.Length == 0)
				innerNewlines = false;
			string newlineString = innerNewlines ? "\n" : "";
			return string.Format("<{0}{1}>{2}{3}{2}</{0}>\n", tag, GetAttributeString(attributes), newlineString, content.Trim());
		}

		public static string ClassContentTag(string tag, string content, string style, string id, bool innerNewlines = true)
		{
			Dictionary<string, string> attributes = new Dictionary<string, string>();
			attributes["class"] = style;
			attributes["id"] = id;
			return ContentTag(tag, content, attributes, innerNewlines);
		}

		public static string Escape(string input)
		{
			return HtmlEncode(input);
		}

		public static string Html(string content)
		{
			return string.Format("<!DOCTYPE html>\n{0}", ContentTag("html", content));
		}

		public static string Head(string content)
		{
			return ContentTag("head", content);
		}

		public static string Title(string title)
		{
			return ContentTag("title", Escape(title), null, false);
		}

		public static string Meta(Dictionary<string, string> attributes)
		{
			return Tag("meta", attributes);
		}

		public static string MetaLink(string relationship, Dictionary<string, string> attributes)
		{
			attributes["rel"] = relationship;
			return Tag("link", attributes);
		}

		public static string Body(string content)
		{
			return ContentTag("body", content);
		}

		public static string Paragraph(string content, string style = null, string id = null)
		{
			return ClassContentTag("p", content, style, id);
		}

		public static string Diverse(string content, string style = null, string id = null)
		{
			return ClassContentTag("div", content, style, id);
		}

		public static string UnorderedList(string content, string style = null, string id = null)
		{
			return ClassContentTag("ul", content, style, id);
		}

		public static string ListEntry(string content, string style = null, string id = null)
		{
			return ClassContentTag("li", content, style, id, false);
		}

		public static string Link(string uri, string content, string style = null, string id = null)
		{
			Dictionary<string, string> attributes = new Dictionary<string, string>();
			attributes["href"] = uri;
			attributes["class"] = style;
			attributes["id"] = id;
			return ContentTag("a", content, attributes, false);
		}

		public static string Bold(string content, string style = null, string id = null)
		{
			return ClassContentTag("b", content, style, id, false);
		}

		public static string Span(string content, string style = null, string id = null)
		{
			return ClassContentTag("span", content, style, id, false);
		}

		public static string Image(string uri, string description, string style = null, string id = null)
		{
			Dictionary<string, string> attributes = new Dictionary<string, string>();
			attributes["src"] = uri;
			attributes["alt"] = description;
			return ClassTag("img", style, id, attributes);
		}

		public static string Script(string uri)
		{
			Dictionary<string, string> attributes = new Dictionary<string, string>();
			attributes["src"] = uri;
			return ContentTag("script", "", attributes, false);
		}

		public static string InlineScript(string script)
		{
			return ContentTag("script", script);
		}

		public static string Table(string content, string style = null, string id = null)
		{
			return ClassContentTag("table", content, style, id);
		}

		public static string TableRow(string content, string style = null, string id = null)
		{
			return ClassContentTag("tr", content, style, id);
		}

		public static string TableCell(string content, string style = null, string id = null)
		{
			return ClassContentTag("td", content, style, id, false);
		}

		public static string TableHead(string content, string style = null, string id = null)
		{
			return ClassContentTag("th", content, style, id, false);
		}

		public static string Caption(string content, string style = null, string id = null)
		{
			return ClassContentTag("caption", content, style, id, false);
		}

		public static string InlineFrame(string uri, string style = null, string id = null)
		{
			Dictionary<string, string> attributes = new Dictionary<string, string>();
			attributes["src"] = uri;
			attributes["class"] = style;
			attributes["id"] = id;
			return ContentTag("iframe", "", attributes, false);
		}

		public static string Form(string uri, string content, string style = null, string id = null)
		{
			Dictionary<string, string> attributes = new Dictionary<string,string>();
			attributes["class"] = style;
			attributes["id"] = id;
			attributes["method"] = "post";
			attributes["action"] = uri;
			return ContentTag("form", content, attributes);
		}

		public static string Input(string type, string name, string value = null, string style = null, string id = null)
		{
			Dictionary<string, string> attributes = new Dictionary<string, string>();
			attributes["class"] = style;
			attributes["id"] = id;
			attributes["type"] = type;
			attributes["name"] = name;
			attributes["value"] = value;
			return Tag("input", attributes);
		}

		public static string Text(string name, string value = null, string style = null, string id = null)
		{
			return Input("text", name, value, style, id);
		}

		public static string Hidden(string name, string value = null, string style = null, string id = null)
		{
			return Input("hidden", name, value, style, id);
		}

		public static string Submit(string description, string style = null, string id = null)
		{
			return Input("submit", null, description, style, id);
		}

		public static string Select(string name, string content, string style = null, string id = null)
		{
			Dictionary<string, string> attributes = new Dictionary<string, string>();
			attributes["name"] = name;
			attributes["class"] = style;
			attributes["id"] = id;
			return ContentTag("select", content, attributes);
		}

		public static string Option(string value, string content, bool selected = false, string style = null, string id = null)
		{
			Dictionary<string, string> attributes = new Dictionary<string, string>();
			attributes["value"] = value;
			attributes["class"] = style;
			attributes["id"] = id;
			if (selected)
				attributes["selected"] = "selected";
			return ContentTag("option", content, attributes, false);
		}
	}
}
