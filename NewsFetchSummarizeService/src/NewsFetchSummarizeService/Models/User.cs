using System.Text.Json.Serialization;

namespace NewsFetchSummarizeService.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Preferences { get; set; }
        public string? CommunicationChannel { get; set; }
        public override string ToString()
        {
            return $"Id: {Id}, FullName: {FullName}, Email: {Email}, Phone: {Phone}, Preferences: {Preferences}, CommunicationChannel: {CommunicationChannel}";
        }
    }

	[Serializable]
	public class NewsNotification
	{
        public NewsNotification(User user, List<PersonalizedNewsArticle> articles )
        {
            this.user = user;
            this.articles = articles;
        }
        [JsonPropertyName("user")]
		public User user { get; set; }

		[JsonPropertyName("articles")]
		public List<PersonalizedNewsArticle> articles { get; set; }
                public override string ToString()
        {
            var articlesString = string.Join("\n\n", articles.ConvertAll(article => article.ToString()));
            return $"User:\n{user}\n\nArticles:\n{articlesString}";
        }
	}

}