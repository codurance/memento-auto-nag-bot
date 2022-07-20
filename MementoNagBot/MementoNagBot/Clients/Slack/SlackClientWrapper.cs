using MementoNagBot.Models.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
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

	private readonly TimeSpan[] _retryDurations =
	{
		TimeSpan.FromMilliseconds(500),
		TimeSpan.FromSeconds(1),
		TimeSpan.FromSeconds(2),
		TimeSpan.FromSeconds(5)
	};
	
	
	public SlackClientWrapper(IOptions<SlackOptions> slackOptions, ILogger<SlackClientWrapper> logger)
	{
		_logger = logger;
		_client = new(slackOptions.Value.SlackApiToken);
		
		_lookupRetryPolicy = Policy.HandleResult<UserEmailLookupResponse>(r => !r.ok)
			.WaitAndRetryAsync(_retryDurations, (_, _, i, _) =>
			{
				if (i < _retryDurations.Length)
				{
					_logger.LogWarning("Failed to lookup Slack User ID from Email, will retry!\nRetry count: {RetryCount}", i);
				}
				else
				{
					_logger.LogError("Can't find Slack User ID from Email... Giving up!");
				}
			});
		
		_messageRetryPolicy = Policy.HandleResult<PostMessageResponse>(r => !r.ok)
			.WaitAndRetryAsync(_retryDurations, (_, _, i, _) =>
			{
				if (i < _retryDurations.Length)
				{
					_logger.LogWarning("Failed to send message to slack, will retry!\nRetry count: {RetryCount}", i);
				}
				else
				{
					_logger.LogError("Can't send message to Slack... Giving up!");
				}
			});
	}


	public Task<UserEmailLookupResponse> GetUserByEmailAsync(string email) 
		=> _lookupRetryPolicy.ExecuteAsync(() => _client.GetUserByEmailAsync(email));

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
		=> _messageRetryPolicy.ExecuteAsync(() => 
			_client.PostMessageAsync(channelId, text, botName, parse, linkNames, blocks, attachments, unfurlLinks, iconUrl, iconEmoji, asUser, threadTs));
}