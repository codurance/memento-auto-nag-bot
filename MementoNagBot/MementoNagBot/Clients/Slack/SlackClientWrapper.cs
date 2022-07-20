using System.Collections.Generic;
using MementoNagBot.Models.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Registry;
using Polly.Retry;
using SlackAPI;
using SlackAPI.RPCMessages;

namespace MementoNagBot.Clients.Slack;

public class SlackClientWrapper: ISlackClient
{
	private readonly ILogger<SlackClientWrapper> _logger;
	private readonly SlackTaskClient _client;
	private readonly AsyncRetryPolicy<UserEmailLookupResponse> _lookupRetryPolicy;
	private readonly AsyncRetryPolicy<PostMessageResponse> _messageRetryPolicy;


	public SlackClientWrapper(IReadOnlyPolicyRegistry<string> policyRegistry, IOptions<SlackOptions> slackOptions, ILogger<SlackClientWrapper> logger)
	{
		_logger = logger;
		_client = new(slackOptions.Value.SlackApiToken);

		_lookupRetryPolicy = policyRegistry.Get<AsyncRetryPolicy<UserEmailLookupResponse>>(nameof(ISlackClient.GetUserByEmailAsync));
		_messageRetryPolicy = policyRegistry.Get<AsyncRetryPolicy<PostMessageResponse>>(nameof(ISlackClient.PostMessageAsync));
	}


	public Task<UserEmailLookupResponse> GetUserByEmailAsync(string email)
	{

		return _lookupRetryPolicy.ExecuteAsync(_ => _client.GetUserByEmailAsync(email), GetFreshContext());
	}

	public Task<PostMessageResponse> PostMessageAsync(
		string channelId,
		string text,
		string botName = null!,
		string parse = null!,
		bool linkNames = false,
		IBlock[] blocks = null!,
		Attachment[] attachments = null!,
		bool? unfurlLinks = null,
		string iconUrl = null!,
		string iconEmoji = null!,
		bool asUser = false,
		string threadTs = null!)
		=> _messageRetryPolicy.ExecuteAsync(_ => 
			_client.PostMessageAsync(channelId, text, botName, parse, linkNames, blocks, attachments, unfurlLinks, iconUrl, iconEmoji, asUser, threadTs), GetFreshContext());

	private Context GetFreshContext()
	{
		return new(Guid.NewGuid().ToString(), new Dictionary<string, object>
		{
			{ "logger", _logger }
		});
	}
}