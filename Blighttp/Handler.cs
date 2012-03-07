using System;
using System.Collections.Generic;

namespace Blighttp
{
	public enum ArgumentType
	{
		String,
		Integer,
	};

	public class Handler
	{
		bool IsDefaultHandler;

		//Default handlers have no name
		string? Name;

		//Default handlers have no arguments
		List<ArgumentType> ArgumentTypes;

		List<Handler> Children;

		//Default handler constructor
		public Handler()
		{
			IsDefaultHandler = true;
			Name = null;
			ArgumentTypes = null;
			Children = new List<Handler>();
		}

		public Handler(String name, List<ArgumentType> argumentTypes = null)
		{
			IsDefaultHandler = false;
			Name = name;
			if (argumentTypes == null)
				ArgumentTypes = new List<ArgumentType>();
			else
				ArgumentTypes = argumentTypes;
			Children = new List<Handler>();
		}

		//Returns null if the handler does not match
		Reply ProcessRequest(Request request, List<string> remainingPath)
		{
			if (remainingPath.Count == 0)
			{
				if (IsDefaultHandler)
					return Handle(request);
				else
					return null;
			}
			if (!IsDefaultHandler)
			{
				string currentName = remainingPath[0];
				remainingPath = remainingPath.GetRange(1, remainingPath.Count - 1);
				if (currentName == Name.Value)
				{
					//The request must be handled by this object
					List<string> argumentStrings = remainingPath;
					if (argumentStrings.Count != ArgumentTypes.Count)
						throw new HandlerException("Invalid argument count");
					List<object> arguments = new List<object>();
					for (int i = 0; i < ArgumentTypes.Count; i++)
					{
						ArgumentType type = ArgumentTypes[i];
						string argumentString = argumentStrings[i];
						object argument;
						switch (type)
						{
							case ArgumentType.String:
								argument = argumentString;
								break;

							case ArgumentType.Integer:
								try
								{
									argument = Convert.ToInt32(argumentString);
								}
								catch (FormatException)
								{
									throw new HandlerException("Invalid integer specified");
								}
								break;

							default:
								throw new Exception("Unknown argument type encountered");
						}
						arguments.Add(argument);
					}
					request.Arguments = arguments;
					return Handle(request);
				}
			}
			//Check children
			foreach (var child in Children)
			{
				Reply reply = child.ProcessRequest(request, remainingPath);
				if (reply != null)
				{
					//A child of the handler found a match for the request
					return reply;
				}
			}
			return null;
		}

		abstract Reply Handle(Request request);
	}
}
