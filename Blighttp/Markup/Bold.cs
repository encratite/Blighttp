namespace Blighttp
{
	public class Bold : ContentTagElement
	{
		const string Tag = "b";

		public Bold()
			: base(Tag)
		{
			Initialise();
		}

		public Bold(string text)
			: base(Tag)
		{
			Initialise();
			Children.Add(new TextElement(text));
		}

		void Initialise()
		{
			InnerNewlines = false;
			ContentType = ElementType.PhrasingContent;
			ContentModel = ElementType.PhrasingContent;
		}
	}
}
