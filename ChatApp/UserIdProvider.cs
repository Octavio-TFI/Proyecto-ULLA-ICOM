using Microsoft.AspNetCore.SignalR;

namespace ChatApp
{
    public class UserIdProvider
        : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return Guid.NewGuid().ToString();
        }
    }
}
