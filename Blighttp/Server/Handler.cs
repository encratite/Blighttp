﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Blighttp
{
	public enum ArgumentType
	{
		String,
		Integer,
	};

	public delegate Reply HandlerCallbackType(Request request);
	public delegate string ChunkedHandlerCallbackType(Request request);

	public class Handler
	{
		//A container is a handler that has no HandlerDelegate and merely contains other handlers
		bool IsContainer;

		//A default handler is a handler that will match an empty routing list (see remainingPath in RouteRequest)
		bool IsDefaultHandler;

		//A chunked handler provides small portions of the reply in repeated calls to the handler
		bool IsChunkedHandler;

		//Default handlers have no name
		public string Name { get; private set; }

		//This is the function that is called when the handler matches the request
		HandlerCallbackType HandlerCallback;

		//This is a partial content provider that is repeatedly called until it returns null, for chunked transfers
		ChunkedHandlerCallbackType ChunkedCallbackDelegate;

		ReplyCode ChunkedReplyCode;
		ContentType ChunkedContentType;

		//Default handlers have no arguments
		ArgumentType[] ArgumentTypes;

		//Sub handlers of this handler
		List<Handler> Children;

		//If this handler has no parent, the parent property is set to null
		//This property is required to retrieve the absolute path of a handler
		Handler Parent;

		//Empty default constructor for internal use only
		Handler()
		{
		}

		//Container constructor
		public Handler(string name)
		{
			IsContainer = true;
			IsDefaultHandler = false;
			IsChunkedHandler = false;

			Name = name;
			HandlerCallback = null;
			ArgumentTypes = null;
			Children = new List<Handler>();
			Parent = null;
		}

		//Default handler constructor
		public Handler(HandlerCallbackType handlerDelegate)
		{
			IsContainer = false;
			IsDefaultHandler = true;
			IsChunkedHandler = false;

			Name = null;
			HandlerCallback = handlerDelegate;
			//Default handlers can't have arguments as they must match an empty routing list
			ArgumentTypes = null;
			Children = new List<Handler>();
			Parent = null;
		}

		//Constructor for regular handlers
		public Handler(string name, HandlerCallbackType handlerDelegate, params ArgumentType[] arguments)
		{
			IsContainer = false;
			IsDefaultHandler = false;
			IsChunkedHandler = false;

			Name = name;
			HandlerCallback = handlerDelegate;
			ChunkedCallbackDelegate = null;
			ArgumentTypes = arguments;
			Children = new List<Handler>();
			Parent = null;
		}

		//Constructor for chunked handlers
		void ChunkedHandlerInitialisation(string name, ChunkedHandlerCallbackType chunkedHandlerDelegate, ContentType contentType = ContentType.Plain, params ArgumentType[] arguments)
		{
			IsContainer = false;
			IsDefaultHandler = false;
			IsChunkedHandler = true;

			//The reply code is currently not customisable because it's not a common use case for chunked handlers
			ChunkedReplyCode = ReplyCode.Ok;
			ChunkedContentType = contentType;

			Name = name;
			HandlerCallback = null;
			ChunkedCallbackDelegate = chunkedHandlerDelegate;
			ArgumentTypes = arguments;
			Children = new List<Handler>();
			Parent = null;
		}

		public static Handler ChunkedHandler(string name, ChunkedHandlerCallbackType chunkedHandlerDelegate, ContentType contentType = ContentType.Plain, params ArgumentType[] arguments)
		{
			Handler handler = new Handler();
			handler.ChunkedHandlerInitialisation(name, chunkedHandlerDelegate, contentType, arguments);
			return handler;
		}

		Reply ProcessRequest(Request request)
		{
			request.RequestHandler = this;
			if (IsChunkedHandler)
				return new Reply(ChunkedReplyCode, ChunkedContentType, ChunkedCallbackDelegate);
			else
				return HandlerCallback(request);
		}

		//Returns null if the handler does not match
		public Reply RouteRequest(Request request, List<string> remainingPath)
		{
			if (remainingPath.Count == 0)
			{
				if (IsDefaultHandler)
				{
					//Execute handler
					return ProcessRequest(request);
				}
				else
					return null;
			}
			if (IsDefaultHandler)
			{
				//Default handlers can only match empty remaining paths
				return null;
			}
			else
			{
				string currentName = remainingPath[0];
				remainingPath = remainingPath.GetRange(1, remainingPath.Count - 1);
				if (currentName == Name)
				{
					//We have a match for the request
					if (IsContainer)
					{
						//We have a hit for the container but the container does not handle the request itself
						//The remaining path needs to be passed on to the children so don't do anything at this point and just continue with the children
					}
					else
					{
						//The request must be handled by this object
						List<string> argumentStrings = remainingPath;
						if (argumentStrings.Count != ArgumentTypes.Length)
							throw new HandlerException("Invalid argument count");
						List<object> arguments = new List<object>();
						for (int i = 0; i < ArgumentTypes.Length; i++)
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
						return ProcessRequest(request);
					}
				}
				else
				{
					//The handler does not match the request
					return null;
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

			//None of the children matched
			return null;
		}

		public void SetParent(Handler parent)
		{
			Parent = parent;
		}

		public void Add(Handler child)
		{
			child.SetParent(this);
			Children.Add(child);
		}

		public string GetPath(params object[] arguments)
		{
			string output = IsDefaultHandler ? "" : string.Format("/{0}", Name);
			foreach (var argument in arguments)
				output += string.Format("/{0}", argument);
			if (Parent != null)
				output = Parent.GetPath() + output;
			if (output.Length == 0)
				output = "/";
			return output;
		}
	}
}
