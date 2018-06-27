using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using ServiceStack.Redis;

namespace ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            IConfigurationRoot configuration = builder.Build();

            Console.WriteLine("Hello. Type anything and press enter.");

            while (true)
            {
                Console.Write("Message: ");

                var message = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(message))
                {
                    break;
                }

                var redisManager = new RedisManagerPool(configuration["ConnectionStrings:Redis"]);
                using (var client = redisManager.GetClient())
                {
                    client.PublishMessage("SignalRCore.Hubs.ChatHub:all", "{\"type\":1,\"target\":\"ReceiveMessage\",\"arguments\":[{\"user\":\"FromConsole\",\"message\":\"Message\"}]");
                }
            }
        }
    }
}


