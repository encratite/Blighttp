using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Blighttp;

namespace Test
{
	class Program
	{
		static void Main(string[] arguments)
		{
			Server server = new Server("127.0.0.1", 9000);
			server.Run();
		}
	}
}
