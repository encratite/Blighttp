namespace Blighttp
{
	public class TableHeadCell : ContentTagElement
	{
		public TableHeadCell()
			: base("th")
		{
			ContentType = ElementType.FlowContent;
		}
	}
}
