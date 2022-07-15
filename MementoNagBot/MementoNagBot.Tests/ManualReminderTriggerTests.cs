using MementoNagBot.Clients.Slack;
using MementoNagBot.Models.Options;
using MementoNagBot.Services.Messaging;
using MementoNagBot.Triggers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SlackAPI;
using SlackAPI.RPCMessages;


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
	        IOptions<BotOptions> options = Options.Create(new BotOptions{BotChannel = channel});
	        SlackMessageService service = new(client, options);
	        ManualReminderTrigger trigger = new(service);
	        HttpRequest req = new DefaultHttpRequest(new DefaultHttpContext());
	        req.QueryString = new("?path=channel");
	        IActionResult? res = await trigger.RunAsync(req, null);
	        res.ShouldBeAssignableTo<OkResult>();
	        await client.Received(1).PostMessageAsync(channel, "What is the answer to life, the universe, and everything?");
        }

        [Fact]
        public async Task ThenItSendsADirectMessageToJames()
        {
	        const string jamesEmail = "james.hughes@codurance.com";
	        const string channel = "#NoChannel";
	        UserEmailLookupResponse response = new()
	        {
		        ok = true,
		        user = new()
		        {
			        name = "James",
			        id = "42"
		        }
	        };
	        ISlackClient client = Substitute.For<ISlackClient>();
	        client.PostMessageAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult(new PostMessageResponse { ok = true }));
	        client.GetUserByEmailAsync(jamesEmail).Returns(Task.FromResult(response));
	        IOptions<BotOptions> options = Microsoft.Extensions.Options.Options.Create(new BotOptions{BotChannel = channel});
	        SlackMessageService service = new(client, options);
	        ManualReminderTrigger trigger = new(service);
	        HttpRequest req = new DefaultHttpRequest(new DefaultHttpContext());
	        req.QueryString = new("?path=direct");
	        IActionResult? res = await trigger.RunAsync(req, null);
	        res.ShouldBeAssignableTo<OkResult>();
	        await client.Received(1).PostMessageAsync(response.user.id, "If I'm 555 then you're 666");
        }
	}
}