using System.Threading.Tasks;
using MementoNagBot.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace MementoNagBot.Triggers;

public class ManualReminderTrigger
{
	private readonly SlackMessageService _messageService;

	public ManualReminderTrigger(SlackMessageService messageService)
	{
		_messageService = messageService;
	}
	
	[FunctionName("ReminderTrigger")]
	public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
	{
		await _messageService.SendMessageToBotChannel("What is the answer to life, the universe, and everything?");
		return new OkResult();
	}
	
}