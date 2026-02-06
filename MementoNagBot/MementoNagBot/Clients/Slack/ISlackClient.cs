namespace MementoNagBot.Clients.Slack;

public interface ISlackClient
{
	public Task<SlackUserLookupResponse> GetUserByEmailAsync(string email);

	public Task<SlackPostMessageResponse> PostMessageAsync(string channelId, string text);
}
