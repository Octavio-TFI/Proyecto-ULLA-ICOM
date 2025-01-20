using ChatApp.DTOs;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController(IHubContext<ChatHub> _chatHubContext)
        : ControllerBase
    {
        // Endpoint para recibir mensajes de la LLM API
        [HttpPost]
        public async Task<IActionResult> ReceiveMessage(
            [FromBody] MessageReceived message)
        {
            await _chatHubContext.Clients.All
                .SendAsync("ReceiveMessage", message.ChatId, message.Texto);

            return Ok();
        }
    }
}
