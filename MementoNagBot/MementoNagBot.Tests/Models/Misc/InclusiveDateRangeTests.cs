using System.Diagnostics.CodeAnalysis;
using MementoNagBot.Models.Misc;
using MementoNagBot.Tests.TestDataGenerators;

namespace MementoNagBot.Tests.Models.Misc;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class InclusiveDateRangeTests
{
	public class GivenADateRange
	{
		public class WhenIIterateOverIt
		{
			[Theory]
			[MemberData(nameof(DateRangeTestGenerator.GenerateDateRanges), 10, MemberType = typeof(DateRangeTestGenerator))]
			public void ThenEachIterationIsTheDayAfterTheLast(InclusiveDateRange testRange)
			{
				testRange.First().ShouldBe(testRange.StartDate);
				testRange.Last().ShouldBe(testRange.EndDate);

				DateOnly lastDate = testRange.StartDate.AddDays(-1);
				foreach (DateOnly date in testRange)
				{
					date.ShouldBe(lastDate.AddDays(1));
					lastDate = date;
				}
			}
			
			[Theory]
			[MemberData(nameof(DateRangeTestGenerator.GenerateDateRanges), 10, MemberType = typeof(DateRangeTestGenerator))]
			public void ThenTheStartDateIsIncluded(InclusiveDateRange testRange)
			{
				testRange.ShouldContain(testRange.StartDate);
			}

			[Theory]
			[MemberData(nameof(DateRangeTestGenerator.GenerateDateRanges), 10, MemberType = typeof(DateRangeTestGenerator))]
			public void ThenTheEndDateIsIncluded(InclusiveDateRange testRange)
			{
				testRange.ShouldContain(testRange.EndDate);
			}

			[Theory]
			[MemberData(nameof(DateRangeTestGenerator.GenerateDateRanges), 10, MemberType = typeof(DateRangeTestGenerator))]
			public void ThenEveryDayBetweenIsIncluded(InclusiveDateRange testRange)
			{
				int expectedDays = (int)(testRange.EndDate.ToDateTime(TimeOnly.MinValue) -
				                         testRange.StartDate.ToDateTime(TimeOnly.MinValue)).TotalDays + 1;

				testRange.Distinct().Count().ShouldBe(expectedDays);
			}
		}
	}
}