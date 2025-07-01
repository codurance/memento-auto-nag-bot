using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using MementoNagBot.Models.Memento;
using MementoNagBot.Models.Misc;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Registry;
using Polly.Retry;

namespace MementoNagBot.Clients.Memento;

public class MementoClient: IMementoClient
{
	private readonly HttpClient _client;
	private readonly ILogger<MementoClient> _logger;
	private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;


	public MementoClient(HttpClient client, IReadOnlyPolicyRegistry<string> policyRegistry, ILogger<MementoClient> logger)
	{
		_client = client;
		_logger = logger;

		_retryPolicy = policyRegistry.Get<AsyncRetryPolicy<HttpResponseMessage>>(nameof(IMementoClient));
	}

	public async Task<List<MementoUser>> GetActiveInternalUsers()
	{
		_logger.LogDebug("Fetching users from Memento...");

		HttpResponseMessage? res = await _retryPolicy.ExecuteAsync(_ => _client.GetAsync("users"), GetFreshContext());

		var rawContent = await res.Content.ReadAsStringAsync();
		var asJson = JsonNode.Parse(rawContent);
		_logger.LogInformation("15th value: ", asJson[14].ToString());
		_logger.LogInformation("Raw content from Memento: {RawContent}", rawContent);

		List<MementoUser>? users = JsonSerializer.Deserialize<List<MementoUser>>(rawContent);
		
		if (users is null)
		{
			_logger.LogError("Failed to fetch users from Memento!");
			return new();
		}
		
		_logger.LogDebug("Successfully retrieved {NumUsers} from Memento", users.Count);
		
		return users
			.Where(u => u.Active)
			.Where(u => u.Role != MementoRole.External)
			.ToList();
	}

	public async Task<MementoTimeSheet> GetTimeSheetForUser(string userId, InclusiveDateRange dateRange)
	{
		_logger.LogDebug("Fetching timesheet for {UserEmail} from {StartDate} to {EndDate}...", userId, dateRange.StartDate, dateRange.EndDate);
		NameValueCollection query = System.Web.HttpUtility.ParseQueryString(string.Empty);
		query.Add("start", dateRange.StartDate.ToString("yyyy-MM-dd"));
		query.Add("end", dateRange.EndDate.ToString("yyyy-MM-dd"));
		string queryString = query.ToString() ?? string.Empty;

		HttpResponseMessage? res = await _retryPolicy.ExecuteAsync(_ => _client.GetAsync($"user/{userId}/timeentries?{queryString}"), GetFreshContext());
		
		List<MementoTimeEntry>? entries = await res.Content.ReadFromJsonAsync<List<MementoTimeEntry>>();

		if (entries is null)
		{
			_logger.LogWarning("Failed to fetch timesheet for {UserEmail} from {StartDate} to {EndDate}... Attempting to continue...", userId, dateRange.StartDate, dateRange.EndDate);
		}
		else
		{
			_logger.LogDebug("Successfully fetched {NumEntries} time-entries for {UserEmail} from {StartDate} to {EndDate}", entries.Count, userId, dateRange.StartDate, dateRange.EndDate);
		}
		
		MementoTimeSheet timeSheet = new(dateRange, entries);

		return timeSheet;
	}
	
	private Context GetFreshContext()
	{
		return new(Guid.NewGuid().ToString(), new Dictionary<string, object>
		{
			{ "logger", _logger }
		});
	}
}
