namespace AggregatorNotifaction.Intefaces
{
	public interface INotification
	{
		Task <bool>SendNotificationAsync(string messageTitle,string messageContent, string sendTo, string from);

	}
}
