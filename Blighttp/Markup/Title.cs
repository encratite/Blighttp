namespace Blighttp
{
	public class Title : ContentTagElement
	{
		public Title(string title)
			: base("title")
		{
			InnerNewlines = false;
			Children.Add(new TextElement(title));
		}
	}
}
