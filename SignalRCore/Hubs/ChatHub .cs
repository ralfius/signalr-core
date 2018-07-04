using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SignalRCore.Models;

namespace SignalRCore.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(MessageModel message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
