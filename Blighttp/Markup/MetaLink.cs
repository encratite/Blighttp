namespace Blighttp
{
	public class MetaLink : TagElement
	{
		public MetaLink(string relationship)
			: base("link")
		{
			ContentType = ElementType.MetaData;
			SetAttribute("rel", relationship);
		}

		public static MetaLink Stylesheet(string path)
		{
			MetaLink stylesheet = new MetaLink("stylesheet");
			stylesheet.SetAttribute("type", "text/css");
			stylesheet.SetAttribute("media", "screen");
			stylesheet.SetAttribute("href", path);
			return stylesheet;
		}

		public static MetaLink Icon(string path)
		{
			MetaLink icon = new MetaLink("icon");
			icon.SetAttribute("type", "image/ico");
			icon.SetAttribute("href", path);
			return icon;
		}
	}
}
