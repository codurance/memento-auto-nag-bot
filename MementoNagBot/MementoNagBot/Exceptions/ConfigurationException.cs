using System;

namespace MementoNagBot.Exceptions;

public class MissingConfigurationException : Exception
{
	public string MissingConfigName { get; }
	public MissingConfigurationException(string missingConfigName, string message): base(message)
	{
		MissingConfigName = missingConfigName;
	}
}