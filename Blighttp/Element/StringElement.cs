namespace Blighttp
{
	public class StringElement : Element
	{
		string Text;

		public StringElement(string text)
		{
			Text = text;
		}

		public override string Render()
		{
			return Text;
		}
	}
}
