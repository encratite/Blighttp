namespace Blighttp
{
	public class Body : ContentTagElement
	{
		public Body()
			: base("body")
		{
			ContentModel = ElementType.FlowContent;
		}
	}
}
