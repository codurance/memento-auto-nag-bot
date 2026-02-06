using System.Diagnostics.CodeAnalysis;
using MementoNagBot.Clients.Slack;
using MementoNagBot.Models.Options;
using MementoNagBot.Services.Messaging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

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
				ISlackClient slackClient = Substitute.For<ISlackClient>();
				slackClient.PostMessageAsync(Arg.Any<string>(), Arg.Any<string>())
					.Returns(new SlackPostMessageResponse { Ok = false, Error = "invalid_auth" });

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
				ISlackClient slackClient = Substitute.For<ISlackClient>();
				slackClient.GetUserByEmailAsync(Arg.Any<string>())
					.Returns(new SlackUserLookupResponse { Ok = false, Error = "users_not_found" });

				BotOptions botOptions = new() { BotChannel = "#botspam" };
				SlackMessageService messageService = new(slackClient, Options.Create(botOptions), NullLogger<SlackMessageService>.Instance);

				await Should.NotThrowAsync(messageService.SendDirectMessageToUser("fakeuser@fakedomain.com", "TestMessage"));
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
				SlackPostMessageResponse messageResponse = new()
				{
					Ok = true
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

				SlackPostMessageResponse messageResponse = new()
				{
					Ok = true
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
				SlackPostMessageResponse messageResponse = new()
				{
					Ok = true
				};
				ISlackClient slackClient = Substitute.For<ISlackClient>();
				slackClient.PostMessageAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(messageResponse);
				slackClient.GetUserByEmailAsync(Arg.Any<string>()).Returns(new SlackUserLookupResponse
				{
					Ok = true,
					User = new SlackUser { Id = "U123" }
				});

				BotOptions botOptions = new() { BotChannel = "#botspam" };
				SlackMessageService messageService = new(slackClient, Options.Create(botOptions), NullLogger<SlackMessageService>.Instance);

				await Should.NotThrowAsync(messageService.SendDirectMessageToUser("fake@user.com", "TestMessage"));
			}

			[Fact]
			public async Task ThenMessageIsPostedToThatUser()
			{
				const string testMessage = "TestMessage";
				const string userEmail = "fake@user.com";

				SlackPostMessageResponse messageResponse = new()
				{
					Ok = true
				};
				SlackUserLookupResponse userResponse = new()
				{
					Ok = true,
					User = new SlackUser
					{
						Id = userEmail
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
