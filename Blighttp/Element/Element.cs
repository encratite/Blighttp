namespace Blighttp
{
	public enum ElementType
	{
		None,
		FlowContent,
		PhrasingContent,
		MetaData,
	};

	public abstract class Element
	{
		public ElementType ContentType;

		public Element()
		{
			ContentType = ElementType.None;
		}

		public abstract string Render();
	}
}
