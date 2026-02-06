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
		var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("email", email) });
		var response = await _httpClient.PostAsync("users.lookupByEmail", content);
		var json = await response.Content.ReadAsStringAsync();
		return JsonSerializer.Deserialize<SlackUserLookupResponse>(json) ?? new SlackUserLookupResponse();
	}

	private async Task<SlackPostMessageResponse> SendMessageAsync(string channelId, string text)
	{
		var payload = new { channel = channelId, text };
		var content = new StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");
		var response = await _httpClient.PostAsync("chat.postMessage", content);
		var json = await response.Content.ReadAsStringAsync();
		return JsonSerializer.Deserialize<SlackPostMessageResponse>(json) ?? new SlackPostMessageResponse();
	}

	private Context GetFreshContext()
	{
		return new(Guid.NewGuid().ToString(), new Dictionary<string, object>
		{
			{ "logger", _logger }
		});
	}
}
