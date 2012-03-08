using System;
using System.Collections.Generic;

namespace Blighttp
{
	public enum ArgumentType
	{
		String,
		Integer,
	};

	public delegate Reply HandlerDelegateType(Request request);

	public class Handler
	{
		//A container is a handler that has no HandlerDelegate and merely contains other handlers
		bool IsContainer;

		//A default handler is a handler that will match an empty routing list (see remainingPath in RouteRequest)
		bool IsDefaultHandler;

		//Default handlers have no name
		string Name;

		HandlerDelegateType HandlerDelegate;

		//Default handlers have no arguments
		List<ArgumentType> ArgumentTypes;

		List<Handler> Children;

		//Container constructor
		public Handler(string name)
		{
			IsContainer = true;
			IsDefaultHandler = false;
			Name = name;
			HandlerDelegate = null;
			ArgumentTypes = null;
			Children = new List<Handler>();
		}

		//Default handler constructor
		public Handler(HandlerDelegateType handlerDelegate)
		{
			IsContainer = false;
			IsDefaultHandler = true;
			Name = null;
			HandlerDelegate = handlerDelegate;
			//Default handlers can't have arguments as they must match an empty routing list
			ArgumentTypes = null;
			Children = new List<Handler>();
		}

		//Constructor for regular handlers
		public Handler(string name, HandlerDelegateType handlerDelegate, List<ArgumentType> argumentTypes = null)
		{
			IsContainer = false;
			IsDefaultHandler = false;
			Name = name;
			HandlerDelegate = handlerDelegate;
			if (argumentTypes == null)
				ArgumentTypes = new List<ArgumentType>();
			else
				ArgumentTypes = argumentTypes;
			Children = new List<Handler>();
		}

		//Returns null if the handler does not match
		public Reply RouteRequest(Request request, List<string> remainingPath)
		{
			if (remainingPath.Count == 0)
			{
				if (IsDefaultHandler)
				{
					//Execute handler
					return HandlerDelegate(request);
				}
				else
					return null;
			}
			if (!IsDefaultHandler)
			{
				string currentName = remainingPath[0];
				remainingPath = remainingPath.GetRange(1, remainingPath.Count - 1);
				if (currentName == Name && !IsContainer)
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
					//Execute handler
					return HandlerDelegate(request);
				}
			}
			//Check children
			foreach (var child in Children)
			{
				Reply reply = child.RouteRequest(request, remainingPath);
				if (reply != null)
				{
					//A child of the handler found a match for the request
					return reply;
				}
			}
			return null;
		}

		public void Add(Handler child)
		{
			Children.Add(child);
		}
	}
}
