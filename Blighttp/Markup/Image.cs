namespace Blighttp
{
	public class Image : TagElement
	{
		public Image(string path, string description)
			: base("img")
		{
			ContentType = ElementType.PhrasingContent;
			SetAttribute("href", path);
			SetAttribute("alt", description);
		}
	}
}
