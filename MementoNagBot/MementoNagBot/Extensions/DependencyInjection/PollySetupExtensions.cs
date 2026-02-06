using System.Net.Http;
using MementoNagBot.Clients.Memento;
using MementoNagBot.Clients.Slack;
using MementoNagBot.Extensions.Polly;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Registry;
using Polly.Retry;

namespace MementoNagBot.Extensions.DependencyInjection;

public static class PollySetupExtensions
{
	private static readonly TimeSpan[] RetryDurations =
	{
		TimeSpan.FromMilliseconds(500),
		TimeSpan.FromSeconds(1),
		TimeSpan.FromSeconds(2),
		TimeSpan.FromSeconds(5)
	};

	public static IReadOnlyPolicyRegistry<string> GetPolicyRegistry()
	{
		AsyncRetryPolicy<SlackUserLookupResponse>? lookupRetryPolicy = Policy.HandleResult<SlackUserLookupResponse>(r => !r.Ok)
			.WaitAndRetryAsync(RetryDurations, (_, _, i, context) =>
			{
				if (!context.TryGetLogger(out ILogger? logger)) return;
				if (i < RetryDurations.Length)
				{
					logger?.LogWarning("Failed to lookup Slack User ID from Email, will retry!\nRetry count: {RetryCount}", i);
				}
				else
				{
					logger?.LogError("Can't find Slack User ID from Email... Giving up!");
				}
			});

		AsyncRetryPolicy<SlackPostMessageResponse>? messageRetryPolicy = Policy.HandleResult<SlackPostMessageResponse>(r => !r.Ok)
			.WaitAndRetryAsync(RetryDurations, (_, _, i, context) =>
			{
				if (!context.TryGetLogger(out ILogger? logger)) return;
				if (i < RetryDurations.Length)
				{
					logger?.LogWarning("Failed to send message to slack, will retry!\nRetry count: {RetryCount}", i);
				}
				else
				{
					logger?.LogError("Can't send message to Slack... Giving up!");
				}
			});

		AsyncRetryPolicy<HttpResponseMessage>? mementoRetryPolicy = Policy.Handle<HttpRequestException>()
			.OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
			.WaitAndRetryAsync(RetryDurations, (_, _, i, context) =>
			{
				if (!context.TryGetLogger(out ILogger? logger)) return;
				if (i < RetryDurations.Length)
				{
					logger?.LogWarning("Memento connection failed\nRetry count: {RetryCount}", i);
				}
				else
				{
					logger?.LogError("Can't reach Memento... Giving up!");
				}
			});

		IPolicyRegistry<string> policyRegistry = new PolicyRegistry();
		policyRegistry.Add(nameof(ISlackClient.GetUserByEmailAsync), lookupRetryPolicy);
		policyRegistry.Add(nameof(ISlackClient.PostMessageAsync), messageRetryPolicy);
		policyRegistry.Add(nameof(IMementoClient), mementoRetryPolicy);

		return policyRegistry;
	}

	public static IServiceCollection RegisterPollyPolicies(this IServiceCollection collection)
	{
		collection.AddSingleton(GetPolicyRegistry());

		return collection;
	}
}
