using MementoNagBot.Triggers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;


namespace MementoNagBot.Tests;

public class ManualReminderTriggerTests
{
	public class WhenTheManualReminderTriggerIsCalled
	{
		[Fact]
		public async Task ThenItReturnsHelloWorld()
		{
			HttpRequest req = new DefaultHttpRequest(new DefaultHttpContext());
			string? res = await ManualReminderTrigger.RunAsync(req, null);
			res.ShouldBe("Hello World!");
		}
	}
}