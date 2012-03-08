using System.Collections.Generic;
using System.Linq;

namespace Blighttp
{
	public class TagElement : Element
	{
		protected string Name;
		Dictionary<string, string> Attributes;

		public TagElement(string name)
			: base()
		{
			Name = name;
			Attributes = new Dictionary<string, string>();
		}

		public void SetAttribute(string name, string value)
		{
			Attributes[name] = value;
		}

		protected string GetAttributeString()
		{
			string output = "";
			foreach (var entry in Attributes)
				output += string.Format(" {0}=\"{1}\"", entry.Key, entry.Value);
			if (output.Length > 0 && output.Last() == '\n')
				output.Remove(output.Length - 1);
			return output;
		}

		public override string Render()
		{
			return string.Format("<{0}{1}>\n", Name, GetAttributeString());
		}
	}
}
