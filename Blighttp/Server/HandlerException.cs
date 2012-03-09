using System;

namespace Blighttp
{
	public class HandlerException : Exception
	{
		public HandlerException(string message)
			: base(message)
		{
		}
	}
}
