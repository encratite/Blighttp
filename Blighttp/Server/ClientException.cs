using System;

namespace Blighttp
{
	class ClientException : Exception
	{
		public ClientException(string message)
			: base(message)
		{
		}
	}
}
