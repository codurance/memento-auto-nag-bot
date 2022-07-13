using System.Net.Http.Headers;
using MementoNagBot.Clients.Memento;
using MementoNagBot.Models.Memento;
using Shouldly;

namespace MementoNagBot.IntegrationTests;

public class MementoIntegrationTests
{
	private static readonly string MementoUrl = Environment.GetEnvironmentVariable("MEMENTO_URL") 
	                                            ?? throw new("You need to set the MEMENTO_URL in your env vars");
	
	public class GivenIHaveAnAuthenticationKey
	{
		private static readonly string MementoAuthToken = Environment.GetEnvironmentVariable("MEMENTO_AUTH_TOKEN")
		                                                   ?? throw new("You need to set the MEMENTO_AUTH_TOKEN in your env vars");
		
		public class WhenIAttemptToGetTheUserList
		{
			[Fact]
			public async Task ThenMementoReturnsAListOfUsers()
			{
				HttpClient innerClient = new();
				innerClient.BaseAddress = new(MementoUrl);
				innerClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", MementoAuthToken);
				IMementoClient client = new MementoClient(innerClient);
				
				List<MementoUser>? res = await client.GetActiveInternalUsers();

				res.ShouldNotBeNull();
				res.Count.ShouldBeGreaterThan(30);
			}

			[Fact]
			public async Task ThenAllUsersReturnedAreActive()
			{
				HttpClient innerClient = new();
				innerClient.BaseAddress = new(MementoUrl);
				innerClient.DefaultRequestHeaders.Authorization = new("", MementoAuthToken);
				IMementoClient client = new MementoClient(innerClient);
				
				List<MementoUser>? res = await client.GetActiveInternalUsers();

				res.ShouldNotBeNull();
				res.ShouldAllBe(m => m.Active);
			}
			
			[Fact]
			public async Task ThenAllUsersReturnedAreInternal()
			{
				HttpClient innerClient = new();
				innerClient.BaseAddress = new(MementoUrl);
				innerClient.DefaultRequestHeaders.Authorization = new("", MementoAuthToken);
				IMementoClient client = new MementoClient(innerClient);
				
				List<MementoUser>? res = await client.GetActiveInternalUsers();

				res.ShouldNotBeNull();
				res.ShouldAllBe(m => m.Role != MementoRole.External);
			}
		}
		
		public class WhenIAttemptToGetActivitiesForAUser
		{
			[Fact]
			public async Task ThenAListOfActivitiesIsReturned()
			{
				
			}
		}
	}
	
	public class GivenIDontHaveAnAuthenticationKey
	{
		public class WhenIAttemptToGetTheUserList
		{
			[Fact]
			public async Task ThenANotAuthorizedExceptionIsThrown()
			{
				
			}
		}
		
		public class WhenIAttemptToGetActivitiesForAUser
		{
			[Fact]
			public async Task ThenANotAuthorizedExceptionIsThrown()
			{
				
			}
		}
	}
}