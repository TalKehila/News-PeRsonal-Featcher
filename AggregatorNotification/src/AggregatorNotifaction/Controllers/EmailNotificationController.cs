

namespace AggregatorNotification.Controllers
{
	[ApiController]
	[Route("api/email")]
	public class EmailNotificationController : ControllerBase
	{
		private readonly DaprClient _daprClient;
		private readonly ILogger<EmailNotificationController> _logger;
		private readonly INotification _notificationService;
		private readonly IConfiguration _configuration;

		public EmailNotificationController(DaprClient daprClient, ILogger<EmailNotificationController> logger, INotification notificationService, IConfiguration configuration)
		{
			_daprClient = daprClient;
			_logger = logger;
			_notificationService = notificationService;
			_configuration = configuration;
		}

		[HttpPost("send")]
		public async Task<IActionResult> SendEmailNotifications()
		{
			try
			{
				_logger.LogInformation("Fetching all users from accessor db...");
				var users = await _daprClient.InvokeMethodAsync<IEnumerable<AggregatorNotifaction.Models.User>>(HttpMethod.Get, "userdbaccessor", "/api/user");

				if (users == null || !users.Any())
				{
					_logger.LogWarning("No users found in the accessor db.");
					return NotFound("No users found in the accessor db.");
				}

				_logger.LogInformation($"Fetched {users.Count()} users from the accessor db.");

				foreach (var user in users)
				{
					try
					{
						_logger.LogInformation($"Fetching personalized news for user {user.Id}...");
						var personalizedNews = await _daprClient.InvokeMethodAsync<List<PersonalizedNewsArticle>>(HttpMethod.Get, "newsfetchsummarizeservice", $"news/{user.Id}");

						if (personalizedNews == null || !personalizedNews.Any())
						{
							_logger.LogWarning($"No personalized news found for user {user.Id}.");
							continue;
						}

						if (user.CommunicationChannel == "Email")
						{
							_logger.LogInformation($"Sending email notification to {user.Email}...");
							var result = await _notificationService.SendNotificationAsync("News Summary", FormatEmailContent(personalizedNews), user.Email, "talkehila21@gmail.com");

							if (!result)
							{
								_logger.LogError($"Failed to send email notification to user {user.Id}.");
							}
						}
						else
						{
							_logger.LogWarning($"User {user.Id} prefers communication via {user.CommunicationChannel}, skipping email notification.");
						}
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, $"Failed to generate or send email notification for user {user.Id}");
					}
				}

				return Ok("Email notifications process completed.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to send email notifications");
				return StatusCode(500, $"Failed to send email notifications: {ex.Message}");
			}
		}

		[Topic("newsfetch", "newsDetails")]
		[HttpPost("sendone")]
		public async Task<IActionResult> SendRegisterEmailNotification([FromBody]NewsNotification news)
		{
			
			try
			{
				
				if (news == null)
				{
					_logger.LogWarning("Received null news notification.");
					return BadRequest("News notification is null");
				}

				var user = news.user;
				var articles = news.articles;

				if (user.CommunicationChannel == "Email")
				{
					_logger.LogInformation($"Sending email notification to {user.Email}...");
					var result = await _notificationService.SendNotificationAsync(
						"News Summary",
						FormatEmailContent(articles),
						user.Email,
						"no-reply@example.com");

					if (result)
					{
						return Ok($"Email sent successfully to {user.Email}");
					}
					else
					{
						_logger.LogError($"Failed to send email notification to user {user.Id}.");
						return StatusCode(500, "Failed to send email notification.");
					}
				}
				else
				{
					_logger.LogWarning($"User {user.Id} prefers communication via {user.CommunicationChannel}, skipping email notification.");
					return Ok($"User prefers communication via {user.CommunicationChannel}, email notification skipped.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to send email notification");
				return StatusCode(500, $"Failed to send email notification: {ex.Message}");
			}
		}

		private string FormatEmailContent(List<PersonalizedNewsArticle> news)
		{
			var htmlBuilder = new StringBuilder();

			// Start with the email header
			htmlBuilder.Append(@"
            <html>
            <head>
                <style>
                    body { font-family: Arial, sans-serif; }
                    .article { margin-bottom: 20px; border-bottom: 1px solid #ddd; padding-bottom: 20px; }
                    .article h2 { color: #333; }
                    .article img { max-width: 100%; height: auto; }
                    .article .meta { color: #666; font-size: 0.9em; }
                    .article .summary { font-style: italic; }
                </style>
            </head>
            <body>
            <h1>Your Personalized News Update</h1>");

			//Add each article to the email
			foreach (var article in news)
			{
				htmlBuilder.Append($@"
                <div class='article'>
                    <h2><a href='{article.Link}'>{article.Title}</a></h2>
                    <p class='meta'>
                        Published on {article.PubDate} | 
                        Source: <a href='{article.SourceUrl}'>{article.SourceId}</a> | 
                        Category: {article.Category}
                    </p>
                    {(string.IsNullOrEmpty(article.ImageUrl) ? "" : $"<img src='{article.ImageUrl}' alt='Article image'>")}
                    <p class='summary'>{article.Summary}</p>
                    <p>{article.Description}</p>
                    <a href='{article.Link}'>Read more</a>
                </div>");
			}

			// Close the HTML tags
			htmlBuilder.Append(@"
            </body>
            </html>");

			return htmlBuilder.ToString();
		}
	}
}
