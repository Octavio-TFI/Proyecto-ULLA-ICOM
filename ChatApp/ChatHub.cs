using Microsoft.AspNetCore.SignalR;

namespace ChatApp
{
    public class ChatHub
        : Hub
    {
        public async Task SendMessageAsync(Guid chatId, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", chatId, message);
        }
    }
}
