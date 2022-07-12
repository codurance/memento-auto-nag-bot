using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace MementoNagBot.Triggers;

public static class ManualReminderTrigger
{
	[FunctionName("ReminderTrigger")]
	public static async Task<string> RunAsync(
		[HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
	{
		return "Hello World!";
	}
}