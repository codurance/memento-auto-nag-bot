using MementoNagBot.Options;
using MementoNagBot.Services;
using MementoNagBot.Triggers;
using MementoNagBot.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Options;
using NSubstitute;
using SlackAPI;


namespace MementoNagBot.Tests;

// ReSharper disable once ClassNeverInstantiated.Global
public class ManualReminderTriggerTests
{
	public class WhenTheManualReminderTriggerIsCalled
	{
		[Fact]
        public async Task ThenItSendsAMessageToSlackChannel()
        {
	        const string channel = "#NoChannel";
	        ISlackClient client = Substitute.For<ISlackClient>();
	        client.PostMessageAsync(Arg.Any<string>(), Arg.Any<string>())
		        .Returns(Task.FromResult(new PostMessageResponse { ok = true }));

	        IOptions<BotOptions> options = Microsoft.Extensions.Options.Options.Create(new BotOptions{BotChannel = channel});
	        SlackMessageService service = new(client, options);
	        ManualReminderTrigger trigger = new(service);
	        HttpRequest req = new DefaultHttpRequest(new DefaultHttpContext());
	        await trigger.RunAsync(req, null);
	        await client.Received(1).PostMessageAsync(channel, "What is the answer to life, the universe, and everything?");
        }
	}
}