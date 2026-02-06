using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using MementoNagBot.Clients.Slack;
using MementoNagBot.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace MementoNagBot.Tests.Clients;

/// <summary>
/// Tests for <see cref="SlackClientWrapper"/>, which is a thin HTTP client over the Slack Web API.
///
/// The wrapper talks to two Slack endpoints:
///
/// 1. <b>chat.postMessage</b> — sends a message to a channel or DM.
///    - POST https://slack.com/api/chat.postMessage
///    - Content-Type: application/json
///    - Body: { "channel": "C12345", "text": "Hello" }
///    - Auth: Bearer token in the Authorization header (configured on HttpClient at DI registration)
///    - Returns: { "ok": true } on success, { "ok": false, "error": "..." } on failure
///
/// 2. <b>users.lookupByEmail</b> — resolves a user's email to their Slack user ID.
///    - POST https://slack.com/api/users.lookupByEmail
///    - Content-Type: application/x-www-form-urlencoded
///    - Body: email=user@example.com
///    - Auth: Bearer token in the Authorization header (configured on HttpClient at DI registration)
///    - Returns: { "ok": true, "user": { "id": "U12345" } } on success,
///              { "ok": false, "error": "users_not_found" } on failure
///
/// Both methods are wrapped in Polly retry policies that retry up to 4 times
/// when the response has "ok": false.
/// </summary>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class SlackClientWrapperTests
{
	private static SlackClientWrapper CreateClient(FakeSlackHandler handler)
	{
		HttpClient httpClient = new(handler) { BaseAddress = new("https://slack.com/api/") };
		var policyRegistry = PollySetupExtensions.GetPolicyRegistry();
		return new SlackClientWrapper(httpClient, policyRegistry, NullLogger<SlackClientWrapper>.Instance);
	}

	public class PostMessageAsync
	{
		public class WhenSlackReturnsOk
		{
			[Fact]
			public async Task ThenTheResponseIsSuccessful()
			{
				using var handler = FakeSlackHandler.WithResponse(new { ok = true });
				var client = CreateClient(handler);

				var result = await client.PostMessageAsync("C12345", "Hello, world!");

				result.Ok.ShouldBeTrue();
			}

			[Fact]
			public async Task ThenItPostsJsonToChatPostMessage()
			{
				using var handler = FakeSlackHandler.WithResponse(new { ok = true });
				var client = CreateClient(handler);

				await client.PostMessageAsync("C12345", "Hello, world!");

				handler.LastRequestUri!.AbsolutePath.ShouldBe("/api/chat.postMessage");
				handler.LastRequestMethod.ShouldBe(HttpMethod.Post);
			}

			[Fact]
			public async Task ThenTheRequestBodyContainsChannelAndText()
			{
				using var handler = FakeSlackHandler.WithResponse(new { ok = true });
				var client = CreateClient(handler);

				await client.PostMessageAsync("C12345", "Hello, world!");

				handler.LastRequestContentType!.ShouldContain("application/json");
				var body = JsonDocument.Parse(handler.LastRequestBody!);
				body.RootElement.GetProperty("channel").GetString().ShouldBe("C12345");
				body.RootElement.GetProperty("text").GetString().ShouldBe("Hello, world!");
			}
		}

		public class WhenSlackReturnsAnError
		{
			[Fact]
			public async Task ThenTheResponseContainsTheError()
			{
				using var handler = FakeSlackHandler.WithResponse(new { ok = false, error = "channel_not_found" });
				var client = CreateClient(handler);

				var result = await client.PostMessageAsync("C00000", "Hello");

				result.Ok.ShouldBeFalse();
				result.Error.ShouldBe("channel_not_found");
			}

			[Fact]
			public async Task ThenTheRetryPolicyRetriesBeforeReturning()
			{
				using var handler = FakeSlackHandler.WithResponse(new { ok = false, error = "channel_not_found" });
				var client = CreateClient(handler);

				await client.PostMessageAsync("C00000", "Hello");

				// Polly is configured with 4 retry durations, so 1 initial + 4 retries = 5 total
				handler.RequestCount.ShouldBe(5);
			}
		}
	}

	public class GetUserByEmailAsync
	{
		public class WhenSlackFindsTheUser
		{
			[Fact]
			public async Task ThenTheResponseContainsTheUserId()
			{
				using var handler = FakeSlackHandler.WithResponse(new { ok = true, user = new { id = "U98765" } });
				var client = CreateClient(handler);

				var result = await client.GetUserByEmailAsync("alice@example.com");

				result.Ok.ShouldBeTrue();
				result.User.ShouldNotBeNull();
				result.User.Id.ShouldBe("U98765");
			}

			[Fact]
			public async Task ThenItPostsFormEncodedToUsersLookupByEmail()
			{
				using var handler = FakeSlackHandler.WithResponse(new { ok = true, user = new { id = "U98765" } });
				var client = CreateClient(handler);

				await client.GetUserByEmailAsync("alice@example.com");

				handler.LastRequestUri!.AbsolutePath.ShouldBe("/api/users.lookupByEmail");
				handler.LastRequestMethod.ShouldBe(HttpMethod.Post);
			}

			[Fact]
			public async Task ThenTheRequestBodyContainsTheEmail()
			{
				using var handler = FakeSlackHandler.WithResponse(new { ok = true, user = new { id = "U98765" } });
				var client = CreateClient(handler);

				await client.GetUserByEmailAsync("alice@example.com");

				handler.LastRequestContentType!.ShouldContain("application/x-www-form-urlencoded");
				handler.LastRequestBody!.ShouldContain("email=alice%40example.com");
			}
		}

		public class WhenSlackCannotFindTheUser
		{
			[Fact]
			public async Task ThenTheResponseContainsTheError()
			{
				using var handler = FakeSlackHandler.WithResponse(new { ok = false, error = "users_not_found" });
				var client = CreateClient(handler);

				var result = await client.GetUserByEmailAsync("nobody@example.com");

				result.Ok.ShouldBeFalse();
				result.Error.ShouldBe("users_not_found");
			}

			[Fact]
			public async Task ThenTheRetryPolicyRetriesBeforeReturning()
			{
				using var handler = FakeSlackHandler.WithResponse(new { ok = false, error = "users_not_found" });
				var client = CreateClient(handler);

				await client.GetUserByEmailAsync("nobody@example.com");

				// Polly is configured with 4 retry durations, so 1 initial + 4 retries = 5 total
				handler.RequestCount.ShouldBe(5);
			}
		}
	}

	/// <summary>
	/// A fake <see cref="HttpMessageHandler"/> that captures outgoing requests and returns
	/// a canned JSON response. Used instead of mocking HttpClient (which is not easily mockable)
	/// or calling the real Slack API.
	/// </summary>
	private class FakeSlackHandler : HttpMessageHandler
	{
		private readonly string _responseJson;

		public int RequestCount { get; private set; }
		public Uri? LastRequestUri { get; private set; }
		public HttpMethod? LastRequestMethod { get; private set; }
		public string? LastRequestBody { get; private set; }
		public string? LastRequestContentType { get; private set; }

		private FakeSlackHandler(string responseJson) => _responseJson = responseJson;

		public static FakeSlackHandler WithResponse(object body) =>
			new(JsonSerializer.Serialize(body));

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
		{
			RequestCount++;
			LastRequestUri = request.RequestUri;
			LastRequestMethod = request.Method;
			if (request.Content != null)
			{
				LastRequestBody = await request.Content.ReadAsStringAsync(cancellationToken);
				LastRequestContentType = request.Content.Headers.ContentType?.ToString();
			}

			return new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new StringContent(_responseJson, System.Text.Encoding.UTF8, "application/json")
			};
		}
	}
}
