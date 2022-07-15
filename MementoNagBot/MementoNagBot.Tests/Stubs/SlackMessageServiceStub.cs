using MementoNagBot.Clients.Slack;
using MementoNagBot.Models.Options;
using MementoNagBot.Services.Messaging;
using Microsoft.Extensions.Options;

namespace MementoNagBot.Tests.Stubs;

public class SlackMessageServiceStub : SlackMessageService
{
	public string LastBotChannelMessage { get; private set; } = string.Empty;

	public readonly List<(string Email, string Message)> DirectMessagesSent = new(); 

	public SlackMessageServiceStub(ISlackClient client, IOptions<BotOptions> botOptions) : base(client, botOptions)
	{
	}

	public override Task SendMessageToBotChannel(string messageText)
	{
		LastBotChannelMessage = messageText;
		return Task.CompletedTask;
	}

	public override Task SendDirectMessageToUser(string userEmail, string message)
	{
		DirectMessagesSent.Add(new(userEmail, message));
		return Task.CompletedTask;
	}
}