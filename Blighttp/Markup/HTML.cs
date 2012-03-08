namespace Blighttp
{
	public class HTML : TagElement
	{
		public HTML(Head head, Body body)
			: base("html")
		{
			base.Add(head);
			base.Add(body);
		}

		public override string Render()
		{
			return string.Format("<!DOCTYPE html>\n{0}", base.Render());
		}

		public override void Add(Element child)
		{
			throw new MarkupException("You can't add child elements to the root element");
		}
	}
}
