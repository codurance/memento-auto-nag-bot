using System.Linq;
using System.Threading.Tasks;
using MementoNagBot.Options;
using Microsoft.Extensions.Options;
using SlackAPI;
using SlackAPI.RPCMessages;

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


	public Task<UserEmailLookupResponse> GetUserByEmailAsync(string email) => _client.GetUserByEmailAsync(email);

	public Task<PostMessageResponse> PostMessageAsync(
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
		string threadTs = null) =>
		_client.PostMessageAsync(channelId, text, botName, parse, linkNames, blocks, attachments,
			unfurlLinks, iconUrl, iconEmoji, asUser, threadTs);
}