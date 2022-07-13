using System.Collections.Generic;
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
		List<MementoUser> users = await _client.GetFromJsonAsync<List<MementoUser>>("users");
		
		if (users is null) return new();
		
		return users
			.Where(u => u.Active)
			.Where(u => u.Role != "External")
			.ToList();
	}

	public Task<List<MementoTimeEntry>> GetTimeEntriesForUser(string userId, InclusiveDateRange dateRange)
	{
		throw new NotImplementedException();
	}
}