namespace Blighttp
{
	public class Paragraph : ContentTagElement
	{
		public Paragraph()
			: base("p")
		{
			ContentType = ElementType.FlowContent;
			ContentModel = ElementType.PhrasingContent;
		}
	}
}
