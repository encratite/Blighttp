namespace Blighttp
{
	public class Head : ContentTagElement
	{
		public Head(string title)
			: base("head")
		{
			ContentModel = ElementType.MetaData;
			Meta unicode = new Meta();
			unicode.SetAttribute("charset", "utf-8");
			Children.Add(unicode);
			Children.Add(new Title(title));
		}
	}
}
