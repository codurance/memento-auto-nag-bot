using MementoNagBot.Clients.Slack;
using MementoNagBot.Models.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SlackAPI;
using SlackAPI.RPCMessages;

namespace MementoNagBot.Services.Messaging;

/// <summary>
/// This service sends messages to Slack.
/// If this fails, it won't throw, as we don't want to prevent other messages from being sent.
/// A retry mechanism is planned: https://github.com/codurance/memento-auto-nag-bot/issues/22
/// </summary>
public class SlackMessageService
{
	private readonly ISlackClient _client;
	private readonly IOptions<BotOptions> _botOptions;
	private readonly ILogger<SlackMessageService> _logger;

	public SlackMessageService(ISlackClient client, IOptions<BotOptions> botOptions, ILogger<SlackMessageService> logger)
	{
		_client = client;
		_botOptions = botOptions;
		_logger = logger;
	}
	
	public virtual async Task SendMessageToBotChannel(string messageText)
	{
		_logger.LogDebug("Attempting to send {Message} to {Channel}", messageText, _botOptions.Value.BotChannel);
		PostMessageResponse res = await _client.PostMessageAsync(_botOptions.Value.BotChannel, messageText);
		if (res.ok)
		{
			_logger.LogDebug("Successfully sent message to bot channel!");
		}
		else
		{
			_logger.LogWarning("Failed to send message to bot channel with {error}! Attempting to continue...", res.error);
		}
	}

	public virtual async Task SendDirectMessageToUser(string userEmail, string message)
	{
		_logger.LogDebug("Attempting to send {Message} to {UserEmail}", message, userEmail);

		try
		{
			UserEmailLookupResponse lookupResponse = await _client.GetUserByEmailAsync(userEmail);
			lookupResponse.AssertOk();
			User user = lookupResponse.user;
			PostMessageResponse res = await _client.PostMessageAsync(user.id, message);
			res.AssertOk();
		}
		catch (Exception)
		{
			_logger.LogWarning("Failed to send message to {UserEmail}! Attempting to continue...", userEmail);
		}
	}
}
