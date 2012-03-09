namespace Blighttp
{
	public class TableCell : ContentTagElement
	{
		public TableCell()
			: base("td")
		{
			ContentType = ElementType.FlowContent;
		}
	}
}
