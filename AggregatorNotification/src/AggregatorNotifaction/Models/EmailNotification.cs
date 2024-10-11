namespace AggregatorNotifaction.Models
{
	public class EmailNotification
	{
        public string? MessageTitle { get; set; }
        public string? MessageContent { get; set; }		
		public string? SendTo { get; set; }
		public string? From { get; set; }

    }
	
}
