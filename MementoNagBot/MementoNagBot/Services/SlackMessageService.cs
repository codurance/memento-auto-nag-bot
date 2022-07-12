using System;
using System.Threading.Tasks;
using MementoNagBot.Wrappers;
using SlackAPI;

namespace MementoNagBot.Services;

public class SlackMessageService
{
	private readonly ISlackClient _client;
	private const string BotChannel = "#botspam"; // TODO - Move into config?

	public SlackMessageService(ISlackClient client)
	{
		_client = client;
	}
	
	public async Task SendMessageToBotChannel(string messageText)
	{
		PostMessageResponse res = await _client.PostMessageAsync(BotChannel, messageText);
		res.AssertOk();
	}

	public async Task SendDirectMessageToUser(string userEmail, string message)
	{
		throw new NotImplementedException();
	}
}
