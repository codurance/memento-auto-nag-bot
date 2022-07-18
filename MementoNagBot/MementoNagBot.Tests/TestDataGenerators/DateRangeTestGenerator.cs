using AutoFixture;
using MementoNagBot.Models.Misc;

namespace MementoNagBot.Tests.TestDataGenerators;

public static class DateRangeTestGenerator
{
	public static IEnumerable<object[]> GenerateDateRanges(int numRanges)
	{
		Fixture fixture = new();
		fixture.Customize<DateOnly>(composer => composer.FromFactory<DateTime>(DateOnly.FromDateTime));

		for (int i = 0; i < numRanges; i++)
		{
			DateOnly[] dates = fixture.CreateMany<DateTime>(2).Select(DateOnly.FromDateTime).ToArray();
			InclusiveDateRange range = new(dates.Min(), dates.Max());
			yield return new object[] {range};
		}
	}
}