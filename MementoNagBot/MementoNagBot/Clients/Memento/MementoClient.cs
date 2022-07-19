using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using MementoNagBot.Models.Memento;
using MementoNagBot.Models.Misc;
using Microsoft.Extensions.Logging;

namespace MementoNagBot.Clients.Memento;

public class MementoClient: IMementoClient
{
	private readonly HttpClient _client;
	private readonly ILogger<MementoClient> _logger;

	public MementoClient(HttpClient client, ILogger<MementoClient> logger)
	{
		_client = client;
		_logger = logger;
	}

	public async Task<List<MementoUser>> GetActiveInternalUsers()
	{
		_logger.LogDebug("Fetching users from Memento...");
		List<MementoUser>? users = await _client.GetFromJsonAsync<List<MementoUser>>("users");
		
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

		List<MementoTimeEntry>? entries = await _client.GetFromJsonAsync<List<MementoTimeEntry>>($"user/{userId}/timeentries?{queryString}");

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
}