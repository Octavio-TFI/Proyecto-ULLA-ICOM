using ChatApp.DTOs;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp
{
    public class ChatHub(
        ILogger<ChatHub> _logger,
        IConfiguration _configuration)
        : Hub
    {
        public async Task SendMessageAsync(string text)
        {
            HttpClient client = new()
            {
                BaseAddress =
                    new Uri(
                        _configuration.GetValue<string>("ApiURL") ??
                            throw new Exception(
                                "Se debe configurar la URL de la api en ApiURL"))
            };

            if (!Guid.TryParse(Context.UserIdentifier, out Guid chatId))
            {
                _logger.LogError("Invalid chat ID: {}", chatId);

                return;
            }

            var messageToSend = new MessageToSend
            {
                ChatId = chatId,
                Texto = text,
                DateTime = DateTime.Now
            };

            _logger.LogInformation(
                @"Chat: {}
Sending message to API: {}",
                chatId,
                text);

            try
            {
                var response = await client.PostAsJsonAsync(
                    "/Test/texto",
                    messageToSend);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Message sent successfully");
                }
                else
                {
                    _logger.LogError(
                        "Failed to send message: {}",
                        response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to send message: {}", e.Message);
            }
        }
    }
}
