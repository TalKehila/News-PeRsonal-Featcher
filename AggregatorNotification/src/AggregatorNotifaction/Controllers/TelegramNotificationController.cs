namespace AggregatorNotifaction.Controllers
{
	[ApiController]
	[Route("api/notification")]
	public class NotificationController : ControllerBase
	{
		private readonly DaprClient _daprClient;
		private readonly ILogger<NotificationController> _logger;
		private readonly INotification _notificationService;
		public NotificationController(DaprClient daprClient, ILogger<NotificationController> logger, INotification notificationService)
		{
			_daprClient = daprClient;
			_logger = logger;
			_notificationService = notificationService;
		}

	[HttpPost("send")]
	public async Task<IActionResult> SendNotifications()
	{
		try
		{
			_logger.LogInformation("Fetching all users from dbaccessor...");
			var users = await _daprClient.InvokeMethodAsync<IEnumerable<Models.User>>(HttpMethod.Get, "userdbaccessor", "api/user");
				// i removed the /api before the user
			if (users == null || !users.Any())
			{
				_logger.LogWarning("No users found.");
				return NotFound("No users found.");
			}

			_logger.LogInformation($"Fetched {users.Count()} users.");

			foreach (var user in users)
			{
				if (string.IsNullOrEmpty(user.Phone))
				{
					_logger.LogWarning($"User {user.Id} does not have a phone number.");
					continue;
				}

				try
				{
					_logger.LogInformation($"Fetching personalized news for user {user.Id}...");
               	var personalizedNews = await _daprClient.InvokeMethodAsync<List<PersonalizedNewsArticle>>(
                    HttpMethod.Get, 
                    "newsfetchsummarizeservice", 
                    $"news/{user.Id}");

					_logger.LogInformation($"Sending notification to {user.Phone}...");
					var result = await _notificationService.SendNotificationAsync("Notification Title", personalizedNews.ToString(), user.Phone, "From Address");

					if (!result)
					{
						_logger.LogError($"Failed to send notification to user {user.Id}.");
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, $"Failed to generate or send notification for user {user.Id}");
				}
			}

			return Ok("Notifications process completed.");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to send notifications");
			return StatusCode(500, $"Failed to send notifications: {ex.Message}");
		}
	}

	}
	

}