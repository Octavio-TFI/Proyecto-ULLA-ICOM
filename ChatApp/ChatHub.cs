using ChatApp.DTOs;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp
{
    public class ChatHub(ILogger<ChatHub> _logger)
        : Hub
    {
        public async Task ReceiveMessageAsync(Guid chatId, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", chatId, message);
        }

        public async Task SendMessageAsync(Guid chatId, string message)
        {
            HttpClient client = new()
            {
                BaseAddress = new Uri("https://localhost:7160")
            };

            var messageToSend = new MessageToSend
            {
                ChatId = chatId,
                Texto = message,
                DateTime = DateTime.Now
            };

            _logger.LogInformation("Sending message to API: {0}", message);

            try
            {
                var response = await client.PostAsJsonAsync(
                    "/Test",
                    messageToSend);

                if(response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Message sent successfully");
                } else
                {
                    _logger.LogError(
                        "Failed to send message: {0}",
                        response.ReasonPhrase);
                }
            } catch(Exception e)
            {
                _logger.LogError("Failed to send message: {0}", e.Message);
            }
        }
    }
}
