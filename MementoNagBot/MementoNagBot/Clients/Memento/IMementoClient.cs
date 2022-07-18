using System.Collections.Generic;
using MementoNagBot.Models.Memento;
using MementoNagBot.Models.Misc;

namespace MementoNagBot.Clients.Memento;

public interface IMementoClient
{
	public Task<List<MementoUser>> GetActiveInternalUsers();
	public Task<List<MementoTimeEntry>> GetTimeEntriesForUser(string userId, InclusiveDateRange dateRange);
	public Task<MementoTimeSheet?> GetTimeSheetForUser(string userId, InclusiveDateRange dateRange);
}