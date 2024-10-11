namespace AggregatorNotifaction.Models
{
	public class TelegramNotification
	{
		public string? MessageTitle { get; set; }
		public string? MessageContent { get; set; }
		public string? SendTo { get; set; }
		public string? From { get; set; }
	}
}
