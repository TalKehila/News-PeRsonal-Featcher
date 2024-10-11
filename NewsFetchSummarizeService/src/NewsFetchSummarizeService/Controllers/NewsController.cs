namespace NewsFetchSummarizeService.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class NewsController : ControllerBase
	{
		private readonly DaprClient _daprClient;
		private readonly NewsFetcher _newsFetcher;
		private readonly ILogger<NewsController>_newsLogger;

		public NewsController(DaprClient daprClient, NewsFetcher newsFetcher, ILogger<NewsController> newslogger)
		{
			_daprClient = daprClient;
			_newsFetcher = newsFetcher;
			_newsLogger = newslogger;
		}

		[Topic("pubsub", "userRegisterDetails")]
		[HttpPost("details")]
		public async Task<IActionResult> HandleUserRegisterDetails([FromBody] User user)
		{
			try
			{
				if (user == null)
				{
					_newsLogger.LogWarning("Received null User");
					return BadRequest("CloudEvent is null.");
				}
				if (string.IsNullOrWhiteSpace(user.Preferences))
				{
					_newsLogger.LogWarning($"User preferences are missing for user: {user.Id}");
					return BadRequest("User preferences are missing.");
				}

				// Fetch news articles
				var newsArticlesResponse = await _newsFetcher.GetNewsArticlesAsync(user);
				if (newsArticlesResponse == null || newsArticlesResponse.results == null)
				{
					_newsLogger.LogWarning($"No news articles found for user: {user.Id}");
					return NotFound("No news articles found.");
				}

				_newsLogger.LogInformation($"Retrieved {newsArticlesResponse.totalResults} articles for user: {user.Id}");

				// Process articles
				var processedArticles = new List<PersonalizedNewsArticle>();
				foreach (var article in newsArticlesResponse.results)
				{
					try
					{
						string content = await _newsFetcher.FetchAndStripHtmlAsync(article.link.ToString());
						string summary = await _newsFetcher.SummarizeContentAsync(content);

                            var newsArticle = new PersonalizedNewsArticle
                            {
								ArticleId = article.article_id,
								Title = article.title ?? "No Title", 
								Link = article.link,
								VideoUrl = article.video_url ?? "No Video Url",
								Description = article.description ?? "No Description", 
								Content = content,
								PubDate = article.pubDate,
								ImageUrl = article.image_url ?? "No Image", 
								SourceId = article.source_id,
								SourcePriority = article.source_priority,
								SourceUrl = article.source_url ?? "No Source URL", 
								SourceIcon = article.source_icon ?? "No Source Icon", 
								Language = article.language ?? "English", 
								Category = user.Preferences,
								Summary = summary ?? "Content couldnt be summarize"
                            };

						processedArticles.Add(newsArticle);
						_newsLogger.LogInformation($"Processed article: {article.articleId} for user: {user.Id}");
					}
					catch (Exception ex)
					{
						_newsLogger.LogError(ex, $"Error processing article {article.articleId} for user {user.Id}");
					}
				}

				// Create NewsNotification
				NewsNotification newsNotification = new NewsNotification(user, processedArticles);

				// Create metadata
				var metadata = new Dictionary<string, string>
				{
					{ "cloudevent.datacontenttype", "application/*+json" }
				};

				// Publish event
				try
				{
					await _daprClient.PublishEventAsync("newsfetch", "newsDetails", newsNotification);
					_newsLogger.LogInformation($"Event published for user: {user.Id}");
				}
				catch (Exception ex)
				{
					_newsLogger.LogError(ex, $"Failed to publish event for user: {user.Id}");
					return StatusCode(500, "Failed to publish event.");
				}

				return Ok("User registration details and preferences processed.");
			}
			catch (Exception ex)
			{
				_newsLogger.LogError(ex, $"An error occurred while processing user registration details");
				return StatusCode(500, $"An error occurred: {ex.Message}");
			}
		}



		[HttpGet("{userId}")]
		public async Task<IActionResult> GetPersonalizedNews(int userId)
		{
			try
			{
				var user = await _daprClient.InvokeMethodAsync<User>(
					HttpMethod.Get,
					"userdbaccessor",
					$"api/user/{userId}");
				Console.WriteLine(userId);
				if (user == null)
				{
					return NotFound("User not found");
				}
				Console.WriteLine(user.Preferences);
				var personalizedNews = new List<PersonalizedNewsArticle>();

				var news = await _newsFetcher.GetNewsArticlesAsync(user);
				if (news != null && news.results != null)
				{
					foreach (var article in news.results)
					{
						if (article.language == "english")
						{
							Console.WriteLine(article.title);
							Console.WriteLine(article.pubDate);
							string content = await _newsFetcher.FetchAndStripHtmlAsync(article.link.ToString());
							string summary = await _newsFetcher.SummarizeContentAsync(content);

							var newsArticle = new PersonalizedNewsArticle
							{
								ArticleId = article.article_id,
								Title = article.title,
								Link = article.link,
								VideoUrl = article.video_url,
								Description = article.description,
								Content = content,
								PubDate = article.pubDate,
								ImageUrl = article.image_url,
								SourceId = article.source_id,
								SourcePriority = article.source_priority,
								SourceUrl = article.source_url,
								SourceIcon = article.source_icon,
								Language = article.language,
								Category = user.Preferences,
								Summary = summary
							};

							personalizedNews.Add(newsArticle);
						}
					}
				}

				return Ok(personalizedNews);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"An error occurred: {ex.Message}");
			}
		}

	}
}