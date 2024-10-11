using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace AggregatorNotifaction.Models
{
	public class NotificationRequest
	{
		public string PhoneNumber { get; set; }
		public string MessageContent { get; set; }
	}

	public class Notification
	{
		public string SendTo { get; set; }
		public string MessageContent { get; set; }
	}

 public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Preferences { get; set; }
        public string CommunicationChannel { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}\n" +
                   $"FullName: {FullName}\n" +
                   $"Email: {Email}\n" +
                   $"Phone: {Phone}\n" +
                   $"Preferences: {Preferences}\n" +
                   $"CommunicationChannel: {CommunicationChannel}";
        }
    }

    public class UserPreferences
    {
        public string NotificationEndpoint { get; set; }

        public override string ToString()
        {
            return $"NotificationEndpoint: {NotificationEndpoint}";
        }
    }

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