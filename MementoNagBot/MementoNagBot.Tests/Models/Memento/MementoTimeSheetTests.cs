using System.Diagnostics.CodeAnalysis;
using MementoNagBot.Models.Memento;
using MementoNagBot.Tests.TestDataGenerators;

namespace MementoNagBot.Tests.Models.Memento;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class MementoTimeSheetTests
{
	public class GivenIHaveATimeSheet
	{
		public class WhenThatTimeSheetIsComplete
		{
			[Theory]
			[MemberData(nameof(TimeSheetTestGenerator.GetFullTimeSheets), 10, MemberType = typeof(TimeSheetTestGenerator))]
			public void ThenICanValidateThatItIsComplete(MementoTimeSheet timeSheet)
			{
				timeSheet.IsComplete().ShouldBeTrue();
			}
		}

		public class WhenThatTimeSheetIsIncomplete
		{
			[Theory]
			[MemberData(nameof(TimeSheetTestGenerator.GetIncompleteTimeSheets), 10, MemberType = typeof(TimeSheetTestGenerator))]
			public void ThenICanValidateThatItIsIncomplete(MementoTimeSheet timeSheet)
			{
				timeSheet.IsComplete().ShouldBeFalse();
			}
		}
	}
}