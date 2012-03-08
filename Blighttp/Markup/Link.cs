namespace Blighttp
{
	public class Link : ContentTagElement
	{
		public Link(string reference)
			: base("a")
		{
			ContentType = ElementType.PhrasingContent;
			ContentModel = ElementType.PhrasingContent;
			SetAttribute("href", reference);
		}
	}
}
