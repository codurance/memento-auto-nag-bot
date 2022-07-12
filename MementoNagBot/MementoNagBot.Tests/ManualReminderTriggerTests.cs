using MementoNagBot.Exceptions;
using MementoNagBot.Services;
using MementoNagBot.Triggers;
using MementoNagBot.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using NSubstitute;


namespace MementoNagBot.Tests;

// ReSharper disable once ClassNeverInstantiated.Global
public class ManualReminderTriggerTests
{
	public class WhenTheManualReminderTriggerIsCalled
	{
		[Fact]
        public async Task ThenItSendsAMessageToSlackChannel()
        {
	        Environment.SetEnvironmentVariable("SLACK_API_TOKEN","12345"); // TODO - Replace with IOptions?
	        ISlackClient client = Substitute.For<ISlackClient>();
	        SlackMessageService service = new(client);
	        ManualReminderTrigger trigger = new(service);
	        HttpRequest req = new DefaultHttpRequest(new DefaultHttpContext());
	        await trigger.RunAsync(req, null);
	        await client.Received(1)
		        .PostMessageAsync("#botspam", "What is the answer to life, the universe, and everything?");
        }
	}
}