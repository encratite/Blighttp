namespace Blighttp
{
	public class Span : ContentTagElement
	{
		public Span()
			: base("span")
		{
			ContentType = ElementType.PhrasingContent;
			ContentModel = ElementType.PhrasingContent;
		}
	}
}
