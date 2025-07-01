using Microsoft.Extensions.Logging;

namespace MementoNagBot.IntegrationTests;

public static class TestConsoleLoggerFactory
{
    public static ILogger<T> CreateLogger<T>(LogLevel logLevel = LogLevel.Error, string namespaceFilter = "")
    {
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            if (!string.IsNullOrWhiteSpace(namespaceFilter))
            {
                builder.AddFilter(namespaceFilter, LogLevel.Debug);
            }
            builder
                .AddFilter("Microsoft", LogLevel.Warning)
                .AddFilter("System", LogLevel.Warning)
                .AddConsole()
                .SetMinimumLevel(logLevel);
        });

        return loggerFactory.CreateLogger<T>();
    }
}
