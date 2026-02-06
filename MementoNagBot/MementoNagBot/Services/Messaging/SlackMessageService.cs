using MementoNagBot.Clients.Slack;
using MementoNagBot.Models.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
		_logger.LogInformation("Attempting to send {Message} to {Channel}", messageText, _botOptions.Value.BotChannel);
		SlackPostMessageResponse res = await _client.PostMessageAsync(_botOptions.Value.BotChannel, messageText);
		if (res.Ok)
		{
			_logger.LogInformation("Successfully sent message to bot channel!");
		}
		else
		{
			_logger.LogWarning("Failed to send message to bot channel with {error}! Attempting to continue...", res.Error);
		}
	}

	public virtual async Task SendDirectMessageToUser(string userEmail, string message)
	{
		_logger.LogDebug("Attempting to send {Message} to {UserEmail}", message, userEmail);

		try
		{
			SlackUserLookupResponse lookupResponse = await _client.GetUserByEmailAsync(userEmail);
			if (!lookupResponse.Ok)
				throw new InvalidOperationException($"Slack user lookup failed: {lookupResponse.Error}");
			SlackPostMessageResponse res = await _client.PostMessageAsync(lookupResponse.User!.Id, message);
			if (!res.Ok)
				throw new InvalidOperationException($"Slack post message failed: {res.Error}");
		}
		catch (Exception)
		{
			_logger.LogWarning("Failed to send message to {UserEmail}! Attempting to continue...", userEmail);
		}
	}
}
