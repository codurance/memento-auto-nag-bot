using System.Threading.Tasks;
using MementoNagBot.Options;
using Microsoft.Extensions.Options;
using SlackAPI;

namespace MementoNagBot.Wrappers;

public class SlackClientWrapper: ISlackClient
{
	private readonly IOptions<SlackOptions> _slackOptions;
	private readonly SlackTaskClient _client;
	public SlackClientWrapper(IOptions<SlackOptions> slackOptions)
	{
		_slackOptions = slackOptions;
		_client = new(_slackOptions.Value.SlackApiToken);
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