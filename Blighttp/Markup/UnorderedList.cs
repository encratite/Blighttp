namespace Blighttp
{
	public class UnorderedList : ContentTagElement
	{
		public UnorderedList()
			: base("ul")
		{
			ContentType = ElementType.FlowContent;
		}

		public void Add(ListEntry entry)
		{
			Children.Add(entry);
		}
	}
}
