using Microsoft.AspNetCore.SignalR;
using SignalRCore.Models;
using System.Threading.Tasks;

namespace SignalRCore.Hubs
{
    public class ProcessingHub : Hub
    {
        public async Task SendMessage(ProcessState message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
