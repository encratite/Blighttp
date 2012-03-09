namespace Blighttp
{
	public class TableRow : ContentTagElement
	{
		public TableRow()
			: base("tr")
		{
		}

		public void Add(TableHeadCell cell)
		{
			Children.Add(cell);
		}

		public void Add(TableCell cell)
		{
			Children.Add(cell);
		}
	}
}
