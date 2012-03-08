using System.Collections.Generic;
using System.Linq;

namespace Blighttp
{
	public class TagElement : Element
	{
		string Name;
		bool HasContent;
		Dictionary<string, string> Attributes;
		List<Element> Children;

		public TagElement(string name, bool hasContent = true)
		{
			Name = name;
			HasContent = hasContent;
			Attributes = new Dictionary<string, string>();
			if (HasContent)
				Children = new List<Element>();
			else
				Children = null;
		}

		protected void SetAttribute(string name, string value)
		{
			Attributes[name] = value;
		}

		string GetAttributeString()
		{
			string output = "";
			foreach (var entry in Attributes)
				output += string.Format(" {0}=\"{1}\"", entry.Key, entry.Value);
			if (output.Length > 0 && output.Last() == '\n')
				output.Remove(output.Length - 1);
			return output;
		}

		string GetChildString()
		{
			string output = "";
			foreach (var child in Children)
				output += child.Render();
			return output;
		}

		public override string Render()
		{
			if (HasContent)
				return string.Format("<{0}{1}>\n{2}\n</{0}>\n", Name, GetAttributeString(), GetChildString());
			else
				return string.Format("<{0}{1}>\n", Name, GetAttributeString());
		}

		public void Add(Element child)
		{
			Children.Add(child);
		}
	}
}
