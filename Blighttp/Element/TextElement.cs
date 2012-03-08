using System.Net;

namespace Blighttp
{
	public class TextElement : Element
	{
		string Text;

		public TextElement(string text)
		{
			ContentType = ElementType.PhrasingContent;

			Text = text;
		}

		public override string Render()
		{
			return WebUtility.HtmlEncode(Text) ;
		}
	}
}
