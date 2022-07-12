using System.Threading.Tasks;
using SlackAPI;

namespace MementoNagBot.Wrappers;

public class SlackClientWrapper: ISlackClient
{
	private readonly SlackTaskClient _client;
	public SlackClientWrapper(SlackTaskClient client)
	{
		
		_client = client;
	}


	public async Task<PostMessageResponse> PostMessageAsync(
		string channelId,
		string text,
		string botName = null,
		string parse = null,
		bool linkNames = false,
		IBlock[] blocks = null,
		Attachment[] attachments = null,
		bool? unfurlLinks = null,
		string iconUrl = null,
		string iconEmoji = null,
		bool asUser = false,
		string threadTs = null)
	{
		return await _client.PostMessageAsync(channelId, text, botName, parse, linkNames, blocks, attachments,
			unfurlLinks, iconUrl, iconEmoji, asUser, threadTs);
	}
}