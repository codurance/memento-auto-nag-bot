using System.Diagnostics.CodeAnalysis;
using MementoNagBot.Clients.Slack;
using MementoNagBot.Models.Options;
using MementoNagBot.Services.Messaging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using SlackAPI;
using SlackAPI.RPCMessages;

namespace MementoNagBot.Tests.Services;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class SlackMessageServiceTests
{
	public class GivenICannotConnectToSlack
	{
		public class WhenIAttemptToSendAMessageToTheBotChannel
		{
			[Fact]
			public async Task ThenItDoesNotThrow()
			{
				SlackOptions slackOptions = new() { SlackApiToken = "123123" };
				ISlackClient slackClient = new SlackClientWrapper(Options.Create(slackOptions), NullLogger<SlackClientWrapper>.Instance);

				BotOptions botOptions = new() { BotChannel = "#botspam" };
				SlackMessageService messageService = new(slackClient, Options.Create(botOptions), NullLogger<SlackMessageService>.Instance);

				await Should.NotThrowAsync(messageService.SendMessageToBotChannel("TestMessage"));
			}
		}
		
		public class WhenIAttemptToSendAMessageToAnIndividualUser
		{
			[Fact]
			public async Task ThenItDoesNotThrow()
			{
				SlackOptions slackOptions = new() { SlackApiToken = "123123" };
				ISlackClient slackClient = new SlackClientWrapper(Options.Create(slackOptions), NullLogger<SlackClientWrapper>.Instance);

				BotOptions botOptions = new() { BotChannel = "#botspam" };
				SlackMessageService messageService = new(slackClient, Options.Create(botOptions), NullLogger<SlackMessageService>.Instance);

				await Should.NotThrowAsync(messageService.SendDirectMessageToUser("fakeuser@fakedomain.com","TestMessage"));
			}
		}
	}
	
	public class GivenICanConnectToSlack
	{
		public class WhenIAttemptToSendAMessageToTheBotChannel
		{
			[Fact]
			public async Task ThenItDoesNotThrow()
			{
				PostMessageResponse messageResponse = new()
				{
					ok = true
				};
				ISlackClient slackClient = Substitute.For<ISlackClient>();
				slackClient.PostMessageAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(messageResponse);

				BotOptions botOptions = new() { BotChannel = "#botspam" };
				SlackMessageService messageService = new(slackClient, Options.Create(botOptions), NullLogger<SlackMessageService>.Instance);

				await Should.NotThrowAsync(messageService.SendMessageToBotChannel("TestMessage"));
			}

			[Fact]
			public async Task ThenMessageIsPostedToSlack()
			{
				const string testMessage = "TestMessage";
				
				PostMessageResponse messageResponse = new()
				{
					ok = true
				};
				ISlackClient slackClient = Substitute.For<ISlackClient>();
				slackClient.PostMessageAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(messageResponse);

				BotOptions botOptions = new() { BotChannel = "#botspam" };
				SlackMessageService messageService = new(slackClient, Options.Create(botOptions), NullLogger<SlackMessageService>.Instance);

				await messageService.SendMessageToBotChannel(testMessage);

				await slackClient.Received(1).PostMessageAsync(botOptions.BotChannel, testMessage);
			}
		}
		
		public class WhenIAttemptToSendAMessageToAnIndividualUser
		{
			[Fact]
			public async Task ThenItDoesNotThrow()
			{
				PostMessageResponse messageResponse = new()
				{
					ok = true
				};
				ISlackClient slackClient = Substitute.For<ISlackClient>();
				slackClient.PostMessageAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(messageResponse);

				BotOptions botOptions = new() { BotChannel = "#botspam" };
				SlackMessageService messageService = new(slackClient, Options.Create(botOptions), NullLogger<SlackMessageService>.Instance);

				await Should.NotThrowAsync(messageService.SendDirectMessageToUser("fake@user.com","TestMessage"));
			}

			[Fact]
			public async Task ThenMessageIsPostedToThatUser()
			{
				const string testMessage = "TestMessage";
				const string userEmail = "fake@user.com";
				
				PostMessageResponse messageResponse = new()
				{
					ok = true
				};
				UserEmailLookupResponse userResponse = new()
				{
					ok = true,
					user = new()
					{
						id = userEmail
					}
				};
				ISlackClient slackClient = Substitute.For<ISlackClient>();
				slackClient.PostMessageAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(messageResponse);
				slackClient.GetUserByEmailAsync(Arg.Any<string>()).Returns(userResponse);

				BotOptions botOptions = new() { BotChannel = "#botspam" };
				SlackMessageService messageService = new(slackClient, Options.Create(botOptions), NullLogger<SlackMessageService>.Instance);

				await messageService.SendDirectMessageToUser(userEmail, testMessage);

				await slackClient.Received(1).PostMessageAsync(userEmail, testMessage);
			}
		}
	}
}