using MementoNagBot.Clients.Slack;
using MementoNagBot.Models.Options;
using Microsoft.Extensions.Options;
using SlackAPI;
using SlackAPI.RPCMessages;

namespace MementoNagBot.Services.Messaging;

public class SlackMessageService
{
	private readonly ISlackClient _client;
	private readonly IOptions<BotOptions> _botOptions;

	public SlackMessageService(ISlackClient client, IOptions<BotOptions> botOptions)
	{
		_client = client;
		_botOptions = botOptions;
	}
	
	public virtual async Task SendMessageToBotChannel(string messageText)
	{
		PostMessageResponse res = await _client.PostMessageAsync(_botOptions.Value.BotChannel, messageText);
		res.AssertOk();
	}

	public virtual async Task SendDirectMessageToUser(string userEmail, string message)
	{
		UserEmailLookupResponse lookupResponse = await _client.GetUserByEmailAsync(userEmail);
		lookupResponse.AssertOk();
		User user = lookupResponse.user;
		PostMessageResponse res = await _client.PostMessageAsync(user.id, message);
		res.AssertOk();
	}
}
