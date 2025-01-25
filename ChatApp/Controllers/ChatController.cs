using ChatApp.DTOs;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController(
        IHubContext<ChatHub> _chatHubContext,
        ILogger<ChatController> _logger)
        : ControllerBase
    {
        // Endpoint para recibir mensajes de la LLM API
        [HttpPost]
        public async Task<IActionResult> ReceiveMessage(
            [FromBody] MessageReceived message)
        {
            await _chatHubContext.Clients
                .User(message.ChatId.ToString())
                .SendAsync("ReceiveMessage", message.Texto);

            _logger.LogInformation(
                @"Chat: {}
Message received: {}",
                message.ChatId,
                message.Texto);

            return Ok();
        }
    }
}
