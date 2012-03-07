using System;
using System.Collections.Generic;

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
