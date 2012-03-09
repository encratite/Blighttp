namespace Blighttp
{
	public class Table : ContentTagElement
	{
		public Table()
			: base("table")
		{
			ContentType = ElementType.FlowContent;
		}

		public void Add(TableRow row)
		{
			Children.Add(row);
		}
	}
}
