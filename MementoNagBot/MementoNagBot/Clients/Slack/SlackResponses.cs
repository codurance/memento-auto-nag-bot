using System.Text.Json.Serialization;

namespace MementoNagBot.Clients.Slack;

public class SlackPostMessageResponse
{
	[JsonPropertyName("ok")]
	public bool Ok { get; set; }

	[JsonPropertyName("error")]
	public string? Error { get; set; }
}

public class SlackUserLookupResponse
{
	[JsonPropertyName("ok")]
	public bool Ok { get; set; }

	[JsonPropertyName("error")]
	public string? Error { get; set; }

	[JsonPropertyName("user")]
	public SlackUser? User { get; set; }
}

public class SlackUser
{
	[JsonPropertyName("id")]
	public string Id { get; set; } = string.Empty;
}
