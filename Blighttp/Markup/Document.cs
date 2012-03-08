namespace Blighttp
{
	public class Document : ContentTagElement
	{
		public Document(Head head, Body body)
			: base("html")
		{
			Children.Add(head);
			Children.Add(body);
		}

		public override string Render()
		{
			return string.Format("<!DOCTYPE html>\n{0}", base.Render());
		}
	}
}
