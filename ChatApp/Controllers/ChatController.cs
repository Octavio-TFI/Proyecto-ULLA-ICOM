using ChatApp.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Controllers
{
    public class ChatController(ChatHub chatHub)
        : ControllerBase
    {
        // Endpoint para recibir mensajes de la LLM API
        [HttpPost("ReceiveMessage")]
        public async Task<IActionResult> ReceiveMessage(
            [FromBody] MessageReceived message)
        {
            await chatHub.SendMessageAsync(message.ChatId, message.Text);

            return Ok();
        }
    }
}
