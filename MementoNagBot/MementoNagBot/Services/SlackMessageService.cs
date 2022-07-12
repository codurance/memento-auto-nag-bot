using System;
using System.Threading.Tasks;
using MementoNagBot.Options;
using MementoNagBot.Wrappers;
using Microsoft.Extensions.Options;
using SlackAPI;

namespace MementoNagBot.Services;

public class SlackMessageService
{
	private readonly ISlackClient _client;
	private readonly IOptions<BotOptions> _botOptions;

	public SlackMessageService(ISlackClient client, IOptions<BotOptions> botOptions)
	{
		_client = client;
		_botOptions = botOptions;
	}
	
	public async Task SendMessageToBotChannel(string messageText)
	{
		PostMessageResponse res = await _client.PostMessageAsync(_botOptions.Value.BotChannel, messageText);
		res.AssertOk();
	}

	public async Task SendDirectMessageToUser(string userEmail, string message)
	{
		throw new NotImplementedException();
	}
}
