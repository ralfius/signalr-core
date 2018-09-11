using System;
using System.Diagnostics;
using System.Linq;
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
        private readonly PooledRedisClientManager _clientManager;

        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IConfiguration _configuration;

        public NotificationsHandlingHostedService(IConfiguration configuration, IHubContext<ChatHub> hub)
        {
            _configuration = configuration;
            _hubContext = hub;

            var masters = _configuration.GetSection("ConnectionStrings:Redis:Masters").Get<string[]>();
            var slaves = _configuration.GetSection("ConnectionStrings:Redis:Slaves").Get<string[]>();

            _clientManager = new PooledRedisClientManager(masters, slaves);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var redisPubSub = new RedisPubSubServer(_clientManager, "SignalRCore.Hubs.ChatHub:all")
            {
                OnStart = () =>
                {
                    Trace.WriteLine("Connection started");
                },
                OnMessage = (channel, msg) =>
                {
                    var message = JsonConvert.DeserializeObject<MessageModel>(msg);

                    _hubContext.Clients.All.SendAsync("ReceiveMessage", message);
                },
                OnError = (error) =>
                {
                    Trace.WriteLine($"ProcessingHub -> OnError throws {error}");
                },
                OnFailover = (error) =>
                {
                    Trace.WriteLine("Failover initiated");
                }
            };

            redisPubSub.IsSentinelSubscription = true;

            redisPubSub.Start();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Dispose();
        }

        public void Dispose()
        {
            _clientManager?.Dispose();
        }
    }
}
