using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace MementoNagBot.Triggers;

public static class AfternoonTrigger
{
	[FunctionName("AfternoonTrigger")]
	public static async Task RunAsync([TimerTrigger("0 0 16 * * MON-FRI")] TimerInfo myTimer, ILogger log)
	{
		log.LogInformation($"C# Timer trigger function executed at: {DateTime.UtcNow}");
		
	}
}