namespace Blighttp
{
	public class Meta : TagElement
	{
		public Meta()
			: base("meta")
		{
			ContentType = ElementType.MetaData;
		}
	}
}
