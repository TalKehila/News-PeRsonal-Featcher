
namespace AggregatorNotification.Services
{
    public class EmailService : INotification
    {
        private readonly ILogger<EmailService> _logger;
        private readonly string _gmailUsername;
        private readonly string _gmailPassword;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
            _gmailUsername = Environment.GetEnvironmentVariable("GMAIL_USERNAME");
            _gmailPassword = Environment.GetEnvironmentVariable("GMAIL_PASSWORD");
        }

        public async Task<bool> SendNotificationAsync(string messageTitle, string messageContent, string sendTo, string from)
        {
            try
            {
                
                var fromAddress = new MailAddress(_gmailUsername, "From Fetcher");
                var toAddress = new MailAddress(sendTo);
                using (var client = new System.Net.Mail.SmtpClient())
                {
                    client.Host = "smtp.gmail.com";
                    client.Port = 587;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(_gmailUsername, _gmailPassword);
                    using (var message = new MailMessage(
                        from: new MailAddress(_gmailUsername, "News Aggregator"),
                        to: new MailAddress(sendTo, "")
                        ))
                    {

                        message.Subject = messageTitle;
                        message.Body = messageContent;
                        message.IsBodyHtml = true;

                        client.Send(message);
                    }

                }
                _logger.LogInformation($"Email sent successfully to {sendTo}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {sendTo}: {ex.Message}");
                return false;
            }
        }
    }
}