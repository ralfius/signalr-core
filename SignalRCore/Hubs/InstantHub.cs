using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalRCore.Models;
using System.Threading.Tasks;

namespace SignalRCore.Hubs
{
    public class InstantHub : Hub
    {
        //private string AccountId;

        //public override Task OnConnectedAsync()
        //{
        //    var httpContext = this.Context.GetHttpContext();
        //    this.Context.
        //    this.AccountId = httpContext.Request.Query["accountId"];
        //    return base.OnConnectedAsync();
        //}

        public async Task SendMessage(InstantMessage message)
        {            
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
