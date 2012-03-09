using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Blighttp
{
	public class Markup
	{
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
				output += string.Format(" {0}=\"{1}\"", pair.Key, WebUtility.HtmlEncode(pair.Value));
			return output;
		}

		public static string Tag(string tag, Dictionary<string, string> attributes)
		{
			return string.Format("<{0}{1}>\n", tag, GetAttributeString(attributes));
		}

		public static string ClassTag(string tag, string styleClass, Dictionary<string, string> attributes)
		{
			if (styleClass != null)
				attributes["class"] = styleClass;
			return Tag(tag, attributes);
		}

		public static string ContentTag(string tag, string content, Dictionary<string, string> attributes = null, bool innerNewlines = true)
		{
			if (attributes == null)
				attributes = new Dictionary<string, string>();
			string newlineString = innerNewlines ? "\n" : "";
			return string.Format("<{0}{1}>{2}{3}{2}</{0}>\n", tag, GetAttributeString(attributes), newlineString, content.Trim());
		}

		public static string ClassContentTag(string tag, string content, string styleClass, bool innerNewlines = true)
		{
			Dictionary<string, string> attributes = new Dictionary<string, string>();
			if (styleClass != null)
				attributes["class"] = styleClass;
			return ContentTag(tag, content, attributes, innerNewlines);
		}

		public static string Text(string input)
		{
			return WebUtility.HtmlEncode(input);
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
			return ContentTag("title", Text(title), null, false);
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

		public static string Paragraph(string content, string styleClass = null)
		{
			return ClassContentTag("p", content, styleClass);
		}

		public static string Diverse(string content, string styleClass = null)
		{
			return ClassContentTag("div", content, styleClass);
		}

		public static string UnorderedList(string content, string styleClass = null)
		{
			return ClassContentTag("ul", content, styleClass);
		}

		public static string ListEntry(string content, string styleClass = null)
		{
			return ClassContentTag("li", content, styleClass, false);
		}

		public static string Link(string uri, string content, string styleClass = null)
		{
			Dictionary<string, string> attributes = new Dictionary<string, string>();
			attributes["href"] = uri;
			if (styleClass != null)
				attributes["class"] = styleClass;
			return ContentTag("a", content, attributes, false);
		}

		public static string Bold(string content, string styleClass = null)
		{
			return ClassContentTag("b", content, styleClass, false);
		}

		public static string Span(string content, string styleClass = null)
		{
			return ClassContentTag("span", content, styleClass);
		}

		public static string Image(string uri, string description, string styleClass = null)
		{
			Dictionary<string, string> attributes = new Dictionary<string, string>();
			attributes["href"] = uri;
			attributes["alt"] = description;
			return ClassTag("img", styleClass, attributes);
		}

		public static string Table(string content, string styleClass = null)
		{
			return ClassContentTag("table", content, styleClass);
		}

		public static string TableRow(string content, string styleClass = null)
		{
			return ClassContentTag("tr", content, styleClass);
		}

		public static string TableCell(string content, string styleClass = null)
		{
			return ClassContentTag("td", content, styleClass);
		}

		public static string TableHead(string content, string styleClass = null)
		{
			return ClassContentTag("th", content, styleClass);
		}
	}
}
