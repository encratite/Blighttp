using System.Collections.Generic;

namespace Blighttp
{
	public class ContentTagElement : TagElement
	{
		protected List<Element> Children;

		protected ElementType ContentModel;

		protected bool InnerNewlines;

		public ContentTagElement(string name)
			: base(name)
		{
			Children = new List<Element>();

			ContentModel = ElementType.None;

			InnerNewlines = true;
		}

		string GetChildString()
		{
			string output = "";
			foreach (var child in Children)
				output += child.Render();
			if (output.Length > 0 && output[output.Length - 1] == '\n')
				output = output.Remove(output.Length - 1);
			return output;
		}

		public override string Render()
		{
			string newlineString;
			if (InnerNewlines)
				newlineString = "\n";
			else
				newlineString = "";
			return string.Format("<{0}{1}>{3}{2}{3}</{0}>\n", Name, GetAttributeString(), GetChildString(), newlineString);
		}

		public void Add(Element child)
		{
			if (ContentModel == ElementType.None)
				throw new MarkupException("This tag cannot have children");

			if (
				(ContentModel == child.ContentType) ||
				//All phrasing content is flow content
				(ContentModel == ElementType.FlowContent && child.ContentType == ElementType.PhrasingContent)
				)
			{
				Children.Add(child);
			}
		}
	}
}
