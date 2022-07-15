using MementoNagBot.Models.Misc;
using MementoNagBot.Providers.DateTimes;
using MementoNagBot.Services.Gating;
using MementoNagBot.Tests.Stubs;

namespace MementoNagBot.Tests.Services;

// ReSharper disable once ClassNeverInstantiated.Global
public class StartGateServiceTests
{
	// ReSharper disable once ClassNeverInstantiated.Global
	public class GivenTheCronJobFires
	{
		// ReSharper disable once ClassNeverInstantiated.Global
		public class WhenItIsAFriday
		{
			public class AndNotMonthEnd
			{
				[Theory]
				[InlineData(2022, 07, 01)]
				[InlineData(2022, 07, 08)]
				[InlineData(2022, 07, 15)]
				[InlineData(2022, 07, 22)]
				[InlineData(2022, 02, 04)]
				[InlineData(2022, 02, 11)]
				[InlineData(2022, 02, 18)]
				[InlineData(2022, 02, 25)]
				public void ThenItCanRunAsIsFriday(int year, int month, int day)
				{
					IDateProvider dateProvider = new DateProviderStub(new(year, month, day));
					StartGateService service = new(dateProvider);

					service.CanRun().ShouldBe(CanRunResult.CanRunFriday);
				}
			}

			public class AndMonthEnd
			{
				[Theory]
				[InlineData(2021, 12, 31)]
				[InlineData(2022, 09, 30)]
				[InlineData(2023, 03, 31)]
				public void ThenItCantRunAsWeRemindedPeopleYesterday(int year, int month, int day)
				{
					IDateProvider dateProvider = new DateProviderStub(new(year, month, day));
					StartGateService service = new(dateProvider);

					service.CanRun().ShouldBe(CanRunResult.CantRun);
				}
			}

			public class WhenTomorrowIsTheLastWorkingDayOfTheMonth
			{
				[Theory]
				[InlineData(2021, 12, 30)]
				[InlineData(2021, 01, 28)]
				[InlineData(2021, 02, 25)]
				[InlineData(2021, 03, 30)]
				[InlineData(2021, 04, 29)]
				[InlineData(2021, 05, 30)]
				public void ThenItCanRunAsTomorrowLastDay(int year, int month, int day)
				{
					IDateProvider dateProvider = new DateProviderStub(new(year, month, day));
					StartGateService service = new(dateProvider);

					service.CanRun().ShouldBe(CanRunResult.CanRunTomorrowLastDay);
				}
			}
			
			public class WhenItIsNeitherFridayNorTomorrowMonthEnd
			{
				[Theory]
				[MemberData(nameof(NotRightDayGenerator))]
				public void ThenItCantRunAsNotTheRightDay(int year, int month, int day)
				{
					IDateProvider dateProvider = new DateProviderStub(new(year, month, day));
					StartGateService service = new(dateProvider);

					service.CanRun().ShouldBe(CanRunResult.CantRun);
				}

				public static IEnumerable<object[]> NotRightDayGenerator()
				{
					List<DateOnly> badDates = new();
					// Not exhaustive... But not far from it without essentially reimplementing the logic for the TDG
					for (int month = 1; month <= 12; month++)
					{
						for (int day = 1; day <= 21; day++)
						{
							DateOnly date = new(2022, month, day);
							if (date.DayOfWeek is DayOfWeek.Friday) continue;
							badDates.Add(date);
						}
					}

					return badDates.Select(bd => new object[] {bd.Year, bd.Month, bd.Day});
				}
			}
		}
	}
}