using System.Diagnostics.CodeAnalysis;
using AutoFixture;
using MementoNagBot.Clients.Memento;
using MementoNagBot.Models.Memento;
using MementoNagBot.Models.Misc;
using MementoNagBot.Models.Options;
using MementoNagBot.Services.Reminders;
using MementoNagBot.Tests.Stubs;
using Microsoft.Extensions.Options;

namespace MementoNagBot.Tests.Services;

// Some of these tests are doing a bit too much... There's a planned refactor: https://github.com/codurance/memento-auto-nag-bot/issues/49

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class MementoReminderServiceTests
{
	private static readonly IOptions<MementoOptions> Options = Microsoft.Extensions.Options.Options.Create(new MementoOptions { WhiteList = "WhiteList0, WhiteList1,WhiteList2" }); // Odd spacing is deliberate
	public class GivenTomorrowIsMonthEnd
	{
		private static readonly DateProviderStub DateStub = new(new(2022, 06, 29)); // Wednesday with month-end on Thursday
		private const int DaysInWeek = 4;
		private const int HoursInDay = 8;
		private const int HoursInWeek = DaysInWeek * HoursInDay;
		public class WhenISendAGeneralReminder
		{
			[Fact]
			public async Task ThenTheMonthEndMessageIsSent()
			{
				SlackMessageServiceStub messageStub = new(null!, null!);
				DateProviderStub dateStub = new(new(2022, 07, 15));
				MementoReminderService service = new(messageStub, null!, dateStub, Options);

				await service.SendGeneralReminder(true);

				messageStub.LastBotChannelMessage.ShouldBe(MementoReminderService.MonthEndReminderText);
			}
		}

		public class WhenISendIndividualReminders
		{
			// Not using a TDG here as the method will be dealing with the data in bulk rather than individual
			// cases
			
			[Fact]
			public async Task ThenTheIndividualMessageMonthEndTemplateIsUsed()
			{
				Dictionary<MementoUser, MementoTimeSheet> testData = GetTestData(DaysInWeek, HoursInDay);
				List<string> reminderMessages = testData
					.Where(td => !td.Key.Email.Contains("WhiteList"))
					.Where(td => !td.Value.IsComplete())
					.Select(td => string.Format(MementoReminderService.MonthEndIndividualReminderTemplate, td.Key.Name.Split(' ')[0]))
					.ToList();

				SlackMessageServiceStub messageStub = new(null!, null!);
				
				IMementoClient client = Substitute.For<IMementoClient>();
				client.GetActiveInternalUsers().Returns(testData.Keys.ToList());
				foreach (KeyValuePair<MementoUser, MementoTimeSheet> userTimeEntryPair in testData)
				{
					client.GetTimeSheetForUser(userTimeEntryPair.Key.Email, Arg.Any<InclusiveDateRange>())
						.Returns(userTimeEntryPair.Value);
				}
				MementoReminderService service = new(messageStub, client, DateStub, Options);
				
				await service.SendIndividualReminders(true);
				
				
				messageStub.DirectMessagesSent
					.Select(dm => dm.Message)
					.ToList()
					.ShouldBeEquivalentTo(reminderMessages);
			}
			
			[Fact]
			public async Task ThenAllUsersWithoutFullTimesheetsShouldBeMessaged()
			{
				Dictionary<MementoUser, MementoTimeSheet> testData = GetTestData(DaysInWeek, HoursInDay);
				List<string> emailsExpectedToHaveReceivedAMessage = testData
					.Where(td => !td.Key.Email.Contains("WhiteList"))
					.Where(td => !td.Value.IsComplete())
					.Select(td => td.Key.Email)
					.ToList();
				
				SlackMessageServiceStub messageStub = new(null!, null!);
				IMementoClient client = Substitute.For<IMementoClient>();
				client.GetActiveInternalUsers().Returns(testData.Keys.ToList());
				foreach (KeyValuePair<MementoUser, MementoTimeSheet> userTimeEntryPair in testData)
				{
					client.GetTimeSheetForUser(userTimeEntryPair.Key.Email, Arg.Any<InclusiveDateRange>())
						.Returns(userTimeEntryPair.Value);
				}
				MementoReminderService service = new(messageStub, client, DateStub, Options);
				
				await service.SendIndividualReminders(true);


				messageStub.DirectMessagesSent
					.Select(dm => dm.Email)
					.ToList()
					.ShouldBeEquivalentTo(emailsExpectedToHaveReceivedAMessage);
			}

			[Fact]
			public async Task ThenNoWhitelistedUsersShouldBeMessaged()
			{
				Dictionary<MementoUser, MementoTimeSheet> testData = GetTestData(DaysInWeek, HoursInDay);
				List<string> whitelistedUsers = testData.Keys.Select(u => u.Email).Where(u => u.Contains("WhiteList")).ToList();
				
				SlackMessageServiceStub messageStub = new(null!, null!);
				IMementoClient client = Substitute.For<IMementoClient>();
				client.GetActiveInternalUsers().Returns(testData.Keys.ToList());
				foreach (KeyValuePair<MementoUser, MementoTimeSheet> userTimeEntryPair in testData)
				{
					client.GetTimeSheetForUser(userTimeEntryPair.Key.Email, Arg.Any<InclusiveDateRange>())
						.Returns(userTimeEntryPair.Value);
				}
				MementoReminderService service = new(messageStub, client, DateStub, Options);
				
				await service.SendIndividualReminders(true);
				
				messageStub.DirectMessagesSent.Select(dm => dm.Email).ToList().ForEach(e => whitelistedUsers.ShouldNotContain(e));
			}
		}
	}

	public class GivenTomorrowIsNotMonthEnd
	{
		private static readonly DateProviderStub DateStub = new(new(2022, 07, 15));
		private const int DaysInWeek = 5;
		private const int HoursInDay = 8;
		private const int HoursInWeek = DaysInWeek * HoursInDay;
		public class WhenISendAGeneralReminder
		{
			[Fact]
			public async Task ThenTheGeneralReminderMessageIsSent()
			{
				SlackMessageServiceStub messageStub = new(null!, null!);
				MementoReminderService service = new(messageStub, null!, DateStub, Options);

				await service.SendGeneralReminder(false);

				messageStub.LastBotChannelMessage.ShouldBe(MementoReminderService.GeneralReminderText);
			}
			
		}

		public class WhenISendAnIndividualReminder
		{
			// Not using a TDG here as the method will be dealing with the data in bulk rather than individual
			// cases
			
			[Fact]
			public async Task ThenTheIndividualGeneralTemplateIsUsed()
			{
				Dictionary<MementoUser, MementoTimeSheet> testData = GetTestData(DaysInWeek, HoursInDay);
				List<string> reminderMessages = testData
					.Where(td => !td.Key.Email.Contains("WhiteList"))
					.Where(td => !td.Value.IsComplete())
					.Select(td => string.Format(MementoReminderService.IndividualReminderTemplate, td.Key.Name.Split(' ')[0]))
					.ToList();

				SlackMessageServiceStub messageStub = new(null!, null!);
				
				IMementoClient client = Substitute.For<IMementoClient>();
				client.GetActiveInternalUsers().Returns(testData.Keys.ToList());
				foreach (KeyValuePair<MementoUser, MementoTimeSheet> userTimeEntryPair in testData)
				{
					client.GetTimeSheetForUser(userTimeEntryPair.Key.Email, Arg.Any<InclusiveDateRange>())
						.Returns(userTimeEntryPair.Value);
				}
				MementoReminderService service = new(messageStub, client, DateStub, Options);
				
				await service.SendIndividualReminders(false);
				
				
				messageStub.DirectMessagesSent
					.Select(dm => dm.Message)
					.ToList()
					.ShouldBeEquivalentTo(reminderMessages);
			}
			
			[Fact]
			public async Task ThenAllUsersWithoutFullTimesheetsShouldBeMessaged()
			{
				Dictionary<MementoUser, MementoTimeSheet> testData = GetTestData(DaysInWeek, HoursInDay);
				List<string> emailsExpectedToHaveReceivedAMessage = testData
					.Where(td => !td.Key.Email.Contains("WhiteList"))
					.Where(td => !td.Value.IsComplete())
					.Select(td => td.Key.Email)
					.ToList();
				
				SlackMessageServiceStub messageStub = new(null!, null!);
				IMementoClient client = Substitute.For<IMementoClient>();
				client.GetActiveInternalUsers().Returns(testData.Keys.ToList());
				foreach (KeyValuePair<MementoUser, MementoTimeSheet> userTimeEntryPair in testData)
				{
					client.GetTimeSheetForUser(userTimeEntryPair.Key.Email, Arg.Any<InclusiveDateRange>())
						.Returns(userTimeEntryPair.Value);
				}
				MementoReminderService service = new(messageStub, client, DateStub, Options);
				
				await service.SendIndividualReminders(false);


				messageStub.DirectMessagesSent
					.Select(dm => dm.Email)
					.ToList()
					.ShouldBeEquivalentTo(emailsExpectedToHaveReceivedAMessage);
			}
			
			[Fact]
			public async Task ThenNoWhitelistedUsersShouldBeMessaged()
			{
				Dictionary<MementoUser, MementoTimeSheet> testData = GetTestData(DaysInWeek, HoursInDay);
				List<string> whitelistedUsers = testData.Keys.Select(u => u.Email).Where(u => u.Contains("WhiteList")).ToList();
				
				SlackMessageServiceStub messageStub = new(null!, null!);
				IMementoClient client = Substitute.For<IMementoClient>();
				client.GetActiveInternalUsers().Returns(testData.Keys.ToList());
				foreach (KeyValuePair<MementoUser, MementoTimeSheet> userTimeEntryPair in testData)
				{
					client.GetTimeSheetForUser(userTimeEntryPair.Key.Email, Arg.Any<InclusiveDateRange>())
						.Returns(userTimeEntryPair.Value);
				}
				MementoReminderService service = new(messageStub, client, DateStub, Options);
				
				await service.SendIndividualReminders(false);
				
				messageStub.DirectMessagesSent.Select(dm => dm.Email).ToList().ForEach(e => whitelistedUsers.ShouldNotContain(e));
			}
		}
	}
	
	private static Dictionary<MementoUser, MementoTimeSheet> GetTestData(int daysInWeek, int hoursInDay)
	{
		Dictionary<MementoUser, MementoTimeSheet> usertimeSheet = new();
		InclusiveDateRange range = new(new(2022,01,04), new(2022, 01, 08));
				
		Fixture fixture = new();
		fixture.Customize<DateOnly>(composer => composer.FromFactory<DateTime>(DateOnly.FromDateTime));

		// Generate three people with full timesheets

		for (int i = 0; i < 3; i++)
		{
			MementoUser user = fixture.Create<MementoUser>();
			MementoTimeSheet timeSheet = new(range);
			for (int j = 0; j < daysInWeek; j++)
			{
				MementoTimeEntry entry = fixture.Create<MementoTimeEntry>();
				entry = entry with { Hours = hoursInDay, ActivityDate = new(2022, 01, 04 + j) }; // First week that started with a Monday
				timeSheet.Add(entry);
			}
			usertimeSheet.Add(user, timeSheet);
		}

		// Generate three people with half-timesheets
				
		for (int i = 0; i < 3; i++)
		{
			MementoUser user = fixture.Create<MementoUser>();
			MementoTimeSheet timeSheet = new(range);
			for (int j = 0; j < daysInWeek; j++)
			{
				MementoTimeEntry entry = fixture.Create<MementoTimeEntry>();
				entry = entry with { Hours = hoursInDay / 2, ActivityDate = new(2022, 01, 04 + j) }; // First week that started with a Monday
				timeSheet.Add(entry);
			}
			usertimeSheet.Add(user, timeSheet);
		}
				
		// Generate three people with no-timesheet
				
		for (int i = 0; i < 3; i++)
		{
			MementoUser user = fixture.Create<MementoUser>();
			MementoTimeSheet timeSheet = new(range);
			usertimeSheet.Add(user, timeSheet);
		}
		
		// Generate three whitelisted people with no-timesheet
		
		for (int i = 0; i < 3; i++)
		{
			MementoUser user = fixture.Create<MementoUser>();
			user = user with { Email = $"WhiteList{i}" };
			MementoTimeSheet timeSheet = new(range);
			usertimeSheet.Add(user, timeSheet);
		}

		return usertimeSheet;
	}
}
