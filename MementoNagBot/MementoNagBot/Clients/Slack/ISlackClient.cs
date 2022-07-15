using SlackAPI;
using SlackAPI.RPCMessages;

namespace MementoNagBot.Clients.Slack;

public interface ISlackClient
{
	public Task<UserEmailLookupResponse> GetUserByEmailAsync(string email);
	
	public Task<PostMessageResponse> PostMessageAsync(
		string channelId,
		string text,
		string botName = null!,
		string parse = null!,
		bool linkNames = false,
		IBlock[] blocks = null!,
		Attachment[] attachments = null!,
		bool? unfurlLinks = null,
		string iconUrl = null!,
		string iconEmoji = null!,
		bool asUser = false,
		string threadTs = null!);
}