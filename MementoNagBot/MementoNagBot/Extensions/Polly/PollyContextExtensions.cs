using Microsoft.Extensions.Logging;
using Polly;

namespace MementoNagBot.Extensions.Polly;

public static class PollyContextExtensions
{
	public static bool TryGetLogger(this Context context, out ILogger? logger)
	{
		if (context.TryGetValue("logger", out object? loggerObject) && loggerObject is ILogger theLogger)
		{
			logger = theLogger;
			return true;
		}

		logger = null;
		return false;
	}
}