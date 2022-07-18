using System.Diagnostics.CodeAnalysis;
using AutoFixture;
using MementoNagBot.Models.Misc;

namespace MementoNagBot.Tests.Models.Misc;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class InclusiveDateRangeTests
{
	public class GivenADateRange
	{
		public class WhenIGetAllDaysInRange
		{
			[Theory]
			[MemberData(nameof(DateRangeTestGenerator.GenerateDateRanges), 10, MemberType = typeof(DateRangeTestGenerator))]
			public void ThenTheStartDateIsIncluded(InclusiveDateRange testRange)
			{
				List<DateOnly> dates = testRange.GetDaysInRange();
				dates.ShouldContain(testRange.StartDate);
			}

			[Theory]
			[MemberData(nameof(DateRangeTestGenerator.GenerateDateRanges), 10, MemberType = typeof(DateRangeTestGenerator))]
			public void ThenTheEndDateIsIncluded(InclusiveDateRange testRange)
			{
				List<DateOnly> dates = testRange.GetDaysInRange();
				dates.ShouldContain(testRange.EndDate);
			}

			[Theory]
			[MemberData(nameof(DateRangeTestGenerator.GenerateDateRanges), 10, MemberType = typeof(DateRangeTestGenerator))]
			public void ThenEveryDayBetweenIsIncluded(InclusiveDateRange testRange)
			{
				int expectedDays = (int)(testRange.EndDate.ToDateTime(TimeOnly.MinValue) -
				                         testRange.StartDate.ToDateTime(TimeOnly.MinValue)).TotalDays + 1;

				List<DateOnly> dates = testRange.GetDaysInRange();

				dates.Distinct().Count().ShouldBe(expectedDays);
			}


		}
		
		public class WhenIGetTheIterator
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
		}
	}

	private static class DateRangeTestGenerator
	{
		public static IEnumerable<object[]> GenerateDateRanges(int numRanges)
		{
			Fixture fixture = new();
			fixture.Customize<DateOnly>(composer => composer.FromFactory<DateTime>(DateOnly.FromDateTime));

			List<InclusiveDateRange> dateRanges = new();

			for (int i = 0; i < numRanges; i++)
			{
				DateOnly[] dates = fixture.CreateMany<DateTime>(2).Select(DateOnly.FromDateTime).ToArray();
				dateRanges.Add(new(dates.Min(), dates.Max()));
			}

			return dateRanges.Select(d => new object[] { d });
		}
	}
}