using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using MementoNagBot.Models.Memento;
using MementoNagBot.Models.Misc;

namespace MementoNagBot.Clients.Memento;

public class MementoClient: IMementoClient
{
	private readonly HttpClient _client;

	public MementoClient(HttpClient client)
	{
		_client = client;
	}

	public async Task<List<MementoUser>> GetActiveInternalUsers()
	{
		List<MementoUser>? users = await _client.GetFromJsonAsync<List<MementoUser>>("users");
		
		if (users is null) return new();
		
		return users
			.Where(u => u.Active)
			.Where(u => u.Role != MementoRole.External)
			.ToList();
	}

	public async Task<MementoTimeSheet?> GetTimeSheetForUser(string userId, InclusiveDateRange dateRange)
	{
		NameValueCollection query = System.Web.HttpUtility.ParseQueryString(string.Empty);
		query.Add("start", dateRange.StartDate.ToString("yyyy-MM-dd"));
		query.Add("end", dateRange.EndDate.ToString("yyyy-MM-dd"));
		string queryString = query.ToString() ?? string.Empty;

		MementoTimeSheet? timeSheet = await _client.GetFromJsonAsync<MementoTimeSheet>($"user/{userId}/timeentries?{queryString}");

		return timeSheet;
	}
}