using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Registry;
using Polly.Retry;

namespace MementoNagBot.Clients.Slack;

public class SlackClientWrapper : ISlackClient
{
	private readonly ILogger<SlackClientWrapper> _logger;
	private readonly HttpClient _httpClient;
	private readonly AsyncRetryPolicy<SlackUserLookupResponse> _lookupRetryPolicy;
	private readonly AsyncRetryPolicy<SlackPostMessageResponse> _messageRetryPolicy;

	public SlackClientWrapper(HttpClient httpClient, IReadOnlyPolicyRegistry<string> policyRegistry, ILogger<SlackClientWrapper> logger)
	{
		_logger = logger;
		_httpClient = httpClient;

		_lookupRetryPolicy = policyRegistry.Get<AsyncRetryPolicy<SlackUserLookupResponse>>(nameof(ISlackClient.GetUserByEmailAsync));
		_messageRetryPolicy = policyRegistry.Get<AsyncRetryPolicy<SlackPostMessageResponse>>(nameof(ISlackClient.PostMessageAsync));
	}

	public Task<SlackUserLookupResponse> GetUserByEmailAsync(string email)
	{
		return _lookupRetryPolicy.ExecuteAsync(_ => LookupByEmailAsync(email), GetFreshContext());
	}

	public Task<SlackPostMessageResponse> PostMessageAsync(string channelId, string text)
	{
		return _messageRetryPolicy.ExecuteAsync(_ => SendMessageAsync(channelId, text), GetFreshContext());
	}

	private async Task<SlackUserLookupResponse> LookupByEmailAsync(string email)
	{
		_logger.LogInformation("Slack users.lookupByEmail: looking up {Email}", email);
		var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("email", email) });
		var response = await _httpClient.PostAsync("users.lookupByEmail", content);
		var json = await response.Content.ReadAsStringAsync();
		_logger.LogInformation("Slack users.lookupByEmail: HTTP {StatusCode}", (int)response.StatusCode);
		var result = JsonSerializer.Deserialize<SlackUserLookupResponse>(json) ?? new SlackUserLookupResponse();
		if (result.Ok)
			_logger.LogInformation("Slack users.lookupByEmail: resolved {Email} to user {UserId}", email, result.User?.Id);
		else
			_logger.LogInformation("Slack users.lookupByEmail: failed for {Email} with error {SlackError}", email, result.Error);
		return result;
	}

	private async Task<SlackPostMessageResponse> SendMessageAsync(string channelId, string text)
	{
		_logger.LogInformation("Slack chat.postMessage: sending to {Channel}", channelId);
		var payload = new { channel = channelId, text };
		var content = new StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");
		var response = await _httpClient.PostAsync("chat.postMessage", content);
		var json = await response.Content.ReadAsStringAsync();
		_logger.LogInformation("Slack chat.postMessage: HTTP {StatusCode}", (int)response.StatusCode);
		var result = JsonSerializer.Deserialize<SlackPostMessageResponse>(json) ?? new SlackPostMessageResponse();
		if (result.Ok)
			_logger.LogInformation("Slack chat.postMessage: sent to {Channel}", channelId);
		else
			_logger.LogInformation("Slack chat.postMessage: failed for {Channel} with error {SlackError}", channelId, result.Error);
		return result;
	}

	private Context GetFreshContext()
	{
		return new(Guid.NewGuid().ToString(), new Dictionary<string, object>
		{
			{ "logger", _logger }
		});
	}
}
