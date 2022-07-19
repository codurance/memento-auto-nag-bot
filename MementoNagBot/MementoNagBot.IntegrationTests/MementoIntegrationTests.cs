using System.Diagnostics.CodeAnalysis;
using MementoNagBot.Clients.Memento;
using MementoNagBot.Models.Memento;
using MementoNagBot.Models.Misc;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;

namespace MementoNagBot.IntegrationTests;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class MementoIntegrationTests
{
	private static readonly string MementoUrl = Environment.GetEnvironmentVariable("MEMENTO_URL") 
	                                            ?? throw new("You need to set the MEMENTO_URL in your env vars");
	
	public class GivenIHaveAnAuthenticationKey
	{
		private static readonly string MementoAuthToken = Environment.GetEnvironmentVariable("MEMENTO_AUTH_TOKEN")
		                                                   ?? throw new("You need to set the MEMENTO_AUTH_TOKEN in your env vars");


		private static IMementoClient GetMementoClient()
		{
			HttpClient innerClient = new();
			innerClient.BaseAddress = new(MementoUrl);
			innerClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", MementoAuthToken);
			return new MementoClient(innerClient, NullLogger<MementoClient>.Instance);
		}
		
		public class WhenIAttemptToGetTheUserList
		{
			[Fact]
			public async Task ThenMementoReturnsAListOfUsers()
			{
				IMementoClient client = GetMementoClient();
				
				List<MementoUser> res = await client.GetActiveInternalUsers();

				res.ShouldNotBeNull();
				res.Count.ShouldBeGreaterThan(30);
			}

			[Fact]
			public async Task ThenAllUsersReturnedAreActive()
			{
				IMementoClient client = GetMementoClient();
				
				List<MementoUser> res = await client.GetActiveInternalUsers();

				res.ShouldNotBeNull();
				res.ShouldAllBe(m => m.Active);
			}
			
			[Fact]
			public async Task ThenAllUsersReturnedAreInternal()
			{
				IMementoClient client = GetMementoClient();
				
				List<MementoUser> res = await client.GetActiveInternalUsers();

				res.ShouldNotBeNull();
				res.ShouldAllBe(m => m.Role != MementoRole.External);
			}
		}
		
		public class WhenIAttemptToGetActivitiesForAUser
		{
			// This isn't ideal, but for the sake of an internal tool, testing with known data
			// That is unlikely to change is fine. In an ideal world, we'd either have a mock API 
			// Or a fixed test target, but that's overkill for this.
			
			[Fact]
			public async Task ThenAListOfActivitiesIsReturned()
			{
				const string testUserEmail = "james.hughes@codurance.com";
				InclusiveDateRange dateRange = new(new(2022, 06, 27), new(2022, 07, 01));
				
				IMementoClient client = GetMementoClient();
				MementoTimeSheet res = await client.GetTimeSheetForUser(testUserEmail, dateRange);

				res.ShouldNotBeNull();
				res.Count.ShouldBe(5);
			}
			
			[Fact]
			public async Task ThenEachActivityHasADateInRange()
			{
				const string testUserEmail = "james.hughes@codurance.com";
				DateOnly startDate = new(2022, 06, 27);
				DateOnly endDate = new(2022, 07, 01);
				InclusiveDateRange dateRange = new(startDate, endDate);
				
				IMementoClient client = GetMementoClient();
				MementoTimeSheet res = await client.GetTimeSheetForUser(testUserEmail, dateRange);

				res.ShouldNotBeNull();
				res.First().ActivityDate.ShouldBe(startDate);
				res.Last().ActivityDate.ShouldBe(endDate);
				res.Skip(1).SkipLast(1).Select(te => te.ActivityDate).ShouldAllBe(ad => ad > startDate && ad < endDate);
			}
			
			[Fact]
			public async Task ThenTheTotalHoursAreForty()
			{
				const string testUserEmail = "james.hughes@codurance.com";
				InclusiveDateRange dateRange = new(new(2022, 06, 27), new(2022, 07, 01));
				
				IMementoClient client = GetMementoClient();
				MementoTimeSheet res = await client.GetTimeSheetForUser(testUserEmail, dateRange);

				res.ShouldNotBeNull();
				res.Sum(r => r.Hours).ShouldBe(40);
			}

			[Fact]
			public async Task IfThereAreNoEntriesThenReturnAnEmptyCollection()
			{
				const string testUserEmail = "james.hughes@codurance.com";
				DateOnly startDate = new(2000, 01, 1);
				DateOnly endDate = new(2000, 01, 01);
				InclusiveDateRange dateRange = new(startDate, endDate);
				
				IMementoClient client = GetMementoClient();
				MementoTimeSheet res = await client.GetTimeSheetForUser(testUserEmail, dateRange);

				res.ShouldNotBeNull();
				res.ShouldBeEmpty();
			}
			
			[Fact]
			public async Task ThenDateRangeIsSetCorrectly()
			{
				const string testUserEmail = "james.hughes@codurance.com";
				InclusiveDateRange dateRange = new(new(2022, 06, 27), new(2022, 07, 01));
				
				IMementoClient client = GetMementoClient();
				MementoTimeSheet res = await client.GetTimeSheetForUser(testUserEmail, dateRange);

				res.ShouldNotBeNull();
				res.DateRange.ShouldBe(dateRange);
			}
		}
	}
}