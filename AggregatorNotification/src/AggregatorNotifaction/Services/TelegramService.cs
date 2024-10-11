/*
namespace AggregatorNotifaction.Services
{
	public class TelegramService : INotification
	{
		private readonly TelegramBotClient _botClient;

		public TelegramService(string botToken)
		{
			_botClient = new TelegramBotClient(botToken);
		}

		public async Task<bool> SendNotificationAsync(string messageTitle, string messageContent, string sendTo, string from)
		{
			try
			{
				// to fix the metrhod down this file
				Console.WriteLine(sendTo);
				var chatId = await GetChatIdByPhoneNumberAsync(sendTo); 

				// Format message
				string telegramMessage = $"*{messageTitle}*\n\n{messageContent}";

				// Send message
				var message = await _botClient.SendTextMessageAsync(chatId, telegramMessage, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

				return message != null;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Failed to send Telegram message: {ex.Message}");
				return false;
			}
		}

		private async Task<ChatId> GetChatIdByPhoneNumberAsync(string sendTo)
		{
			throw new NotImplementedException();
		}
	}
}
*/

public class TelegramService : INotification
{
	private readonly TelegramBotClient _botClient;

	public TelegramService(string botToken)
	{
		_botClient = new TelegramBotClient(botToken);
	}

	public async Task<bool> SendNotificationAsync(string messageTitle, string messageContent, string sendTo, string from)
	{
		try
		{
			// Format message
			string telegramMessage = $"*{messageTitle}*\n\n{messageContent}";

			// Send message to the phone number
			var message = await _botClient.SendTextMessageAsync(
				chatId: sendTo,  // Use the phone number directly
				text: telegramMessage,
				parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
			);

			return message != null;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Failed to send Telegram message: {ex.Message}");
			return false;
		}
	}
}

