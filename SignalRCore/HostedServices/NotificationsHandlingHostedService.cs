using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using ServiceStack.Redis;
using SignalRCore.Hubs;
using SignalRCore.Models;

namespace SignalRCore.HostedServices
{
    public class NotificationsHandlingHostedService : IHostedService, IDisposable
    {
        private RedisClient _client;
        private IRedisSubscription _subscription;

        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IConfiguration _configuration;

        public NotificationsHandlingHostedService(IConfiguration configuration, IHubContext<ChatHub> hub)
        {
            _configuration = configuration;
            _hubContext = hub;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _client = new RedisClient(_configuration["ConnectionStrings:Redis"]);
            _subscription = _client.CreateSubscription();

            _subscription.OnSubscribe = channel =>
            {
                Trace.WriteLine($"Subscribed to '{channel}'");
            };
            _subscription.OnUnSubscribe = channel =>
            {
                Trace.WriteLine($"UnSubscribed from '{channel}'");
            };
            _subscription.OnMessage = (channel, msg) =>
            {
                Trace.WriteLine($"Received '{msg}' from channel '{channel}'");

                var message = JsonConvert.DeserializeObject<MessageModel>(msg);

                _hubContext.Clients.All.SendAsync("ReceiveMessage", message);
            };

            _subscription.SubscribeToChannels("SignalRCore.Hubs.ChatHub:all");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _subscription?.UnSubscribeFromAllChannels();
            Dispose();
        }

        public void Dispose()
        {
            _client?.Dispose();
            _subscription?.Dispose();
        }
    }
}
