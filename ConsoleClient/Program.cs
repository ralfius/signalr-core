using System;
using System.IO;
using System.Runtime.InteropServices;
using MessagePack;
using Microsoft.AspNetCore.Internal;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
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

            using (var redisClient = new RedisClient(configuration["ConnectionStrings:Redis"]))
            {
                while (true)
                {
                    Console.Write("Message: ");

                    var message = Console.ReadLine()?.Trim();

                    if (string.IsNullOrEmpty(message))
                    {
                        break;
                    }

                    // SignalR Redis contract is used here
                    //var messageObject = GetMessage("ReceiveMessage", new[] { new { user = "Console", message }});
                    //sub.Publish("SignalRCore.Hubs.ChatHub:all", messageObject);

                    // custom contract is used here
                    redisClient.PublishMessage("SignalRCore.Hubs.ChatHub:all", JsonConvert.SerializeObject(new { user = "Console", message }));
                }
            }
        }

        private static byte[] GetMessage(string methodName, object[] args)
        {
            // Written as a MessagePack 'arr' containing at least these items:
            // * A MessagePack 'arr' of 'str's representing the excluded ids
            // * [The output of WriteSerializedHubMessage, which is an 'arr']
            // Any additional items are discarded.

            var writer = MemoryBufferWriter.Get();

            try
            {
                MessagePackBinary.WriteArrayHeader(writer, 2);
                MessagePackBinary.WriteArrayHeader(writer, 0);

                WriteSerializedHubMessage(writer, new SerializedHubMessage(new InvocationMessage(methodName, args)));

                return writer.ToArray();
            }
            finally
            {
                MemoryBufferWriter.Return(writer);
            }
        }

        private static void WriteSerializedHubMessage(Stream stream, SerializedHubMessage message)
        {
            // Written as a MessagePack 'map' where the keys are the name of the protocol (as a MessagePack 'str')
            // and the values are the serialized blob (as a MessagePack 'bin').

            var protocol = new JsonHubProtocol();

            MessagePackBinary.WriteMapHeader(stream, 1);
            MessagePackBinary.WriteString(stream, protocol.Name);

            var serialized = message.GetSerializedMessage(protocol);
            MemoryMarshal.TryGetArray(serialized, out var array);
            MessagePackBinary.WriteBytes(stream, array.Array, array.Offset, array.Count);
        }
    }
}


