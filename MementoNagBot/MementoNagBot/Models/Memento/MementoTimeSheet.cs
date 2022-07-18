using System.Collections;
using System.Collections.Generic;
using MementoNagBot.Models.Misc;

namespace MementoNagBot.Models.Memento;

public class MementoTimeSheet: ICollection<MementoTimeEntry>
{
	private readonly List<MementoTimeEntry> _innerList;
	private DateOnly _startDate;
	private DateOnly _endDate;
	private int _hoursInDay;

	public MementoTimeSheet(InclusiveDateRange dateRange, int hoursInDay = 8)
	{
		_innerList = new();
		_startDate = dateRange.StartDate;
		_endDate = dateRange.EndDate;
		_hoursInDay = hoursInDay;
	}
	
	public int Count => _innerList.Count;
	
	public bool IsReadOnly => false;

	public IEnumerator<MementoTimeEntry> GetEnumerator() => _innerList.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public void Add(MementoTimeEntry item) => _innerList.Add(item);

	public void Clear() => _innerList.Clear();

	public bool Contains(MementoTimeEntry item) => _innerList.Contains(item);

	public void CopyTo(MementoTimeEntry[] array, int arrayIndex) => _innerList.CopyTo(array, arrayIndex);

	public bool Remove(MementoTimeEntry item) => _innerList.Remove(item);


}