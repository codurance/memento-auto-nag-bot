using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace MementoNagBot.Triggers;

public static class NoonTrigger
{
	[FunctionName("NoonTrigger")]
	public static async Task RunAsync([TimerTrigger("0 0 12 * * MON-FRI")] TimerInfo myTimer, ILogger log)
	{
		log.LogInformation($"C# Timer trigger function executed at: {DateTime.UtcNow}");
		
	}
}