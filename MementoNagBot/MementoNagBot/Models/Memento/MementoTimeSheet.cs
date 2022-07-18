using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using MementoNagBot.Models.Misc;

namespace MementoNagBot.Models.Memento;

public class MementoTimeSheet: ICollection<MementoTimeEntry>
{
	private readonly List<MementoTimeEntry> _innerList;
	private readonly InclusiveDateRange _dateRange;
	private readonly int _hoursInDay;

	[UsedImplicitly] // Used by .GetFromJsonAsync() in MementoClient
	public MementoTimeSheet()
	{
		_dateRange = new(DateOnly.MinValue, DateOnly.MinValue);
		_innerList = new();
		_hoursInDay = 8;
	}

	public MementoTimeSheet(InclusiveDateRange dateRange, int hoursInDay = 8)
	{
		_innerList = new();
		_dateRange = dateRange;
		_hoursInDay = hoursInDay;
	}
	
	public int Count => _innerList.Count;
	
	public bool IsReadOnly => false;

	public bool IsComplete()
	{
		Dictionary<DateOnly, List<MementoTimeEntry>> timeEntriesByDay = _innerList
			.GroupBy(te => te.ActivityDate)
			.ToDictionary(k => k.Key, v => v.ToList());
		
		foreach (DateOnly date in _dateRange.Where(d => d.DayOfWeek is not DayOfWeek.Saturday or DayOfWeek.Sunday))
		{
			if (!timeEntriesByDay.TryGetValue(date, out List<MementoTimeEntry>? timeEntries)) return false;
			if (timeEntries.Sum(te => te.Hours) < _hoursInDay) return false;
		}

		return true;
	} 

	public IEnumerator<MementoTimeEntry> GetEnumerator() => _innerList.OrderBy(te => te.ActivityDate).GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public void Add(MementoTimeEntry item) => _innerList.Add(item);

	public void Clear() => _innerList.Clear();

	public bool Contains(MementoTimeEntry item) => _innerList.Contains(item);

	public void CopyTo(MementoTimeEntry[] array, int arrayIndex) => _innerList.CopyTo(array, arrayIndex);

	public bool Remove(MementoTimeEntry item) => _innerList.Remove(item);
}