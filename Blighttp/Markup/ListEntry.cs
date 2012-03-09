namespace Blighttp
{
	public class ListEntry : ContentTagElement
	{
		public ListEntry()
			: base("li")
		{
			ContentModel = ElementType.FlowContent;
		}
	}
}
