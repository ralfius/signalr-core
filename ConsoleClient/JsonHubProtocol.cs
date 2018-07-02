// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Buffers;
using Microsoft.AspNetCore.Internal;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Microsoft.AspNetCore.SignalR.Protocol
{
    /// <summary>
    /// Implements the SignalR Hub Protocol using JSON.
    /// </summary>
    public class JsonHubProtocol : IHubProtocol
    {
        private const string InvocationIdPropertyName = "invocationId";
        private const string TypePropertyName = "type";
        private const string TargetPropertyName = "target";
        private const string ArgumentsPropertyName = "arguments";
        private const string HeadersPropertyName = "headers";

        private static readonly string ProtocolName = "json";

        /// <summary>
        /// Gets the serializer used to serialize invocation arguments and return values.
        /// </summary>
        public JsonSerializer PayloadSerializer { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonHubProtocol"/> class.
        /// </summary>
        public JsonHubProtocol() : this(Options.Create(new JsonHubProtocolOptions()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonHubProtocol"/> class.
        /// </summary>
        /// <param name="options">The options used to initialize the protocol.</param>
        public JsonHubProtocol(IOptions<JsonHubProtocolOptions> options)
        {
            PayloadSerializer = JsonSerializer.Create(options.Value.PayloadSerializerSettings);
        }

        /// <inheritdoc />
        public string Name => ProtocolName;

        /// <inheritdoc />
        public void WriteMessage(HubMessage message, IBufferWriter<byte> output)
        {
            WriteMessageCore(message, output);
            TextMessageFormatter.WriteRecordSeparator(output);
        }

        private void WriteMessageCore(HubMessage message, IBufferWriter<byte> stream)
        {
            var textWriter = Utf8BufferTextWriter.Get(stream);
            try
            {
                using (var writer = JsonUtils.CreateJsonTextWriter(textWriter))
                {
                    writer.WriteStartObject();
                    switch (message)
                    {
                        case InvocationMessage m:
                            WriteMessageType(writer, HubProtocolConstants.InvocationMessageType);
                            WriteHeaders(writer, m);
                            WriteInvocationMessage(m, writer);
                            break;
                        default:
                            throw new InvalidOperationException($"Unsupported message type: {message.GetType().FullName}");
                    }
                    writer.WriteEndObject();
                    writer.Flush();
                }
            }
            finally
            {
                Utf8BufferTextWriter.Return(textWriter);
            }
        }

        private void WriteHeaders(JsonTextWriter writer, HubInvocationMessage message)
        {
            if (message.Headers != null && message.Headers.Count > 0)
            {
                writer.WritePropertyName(HeadersPropertyName);
                writer.WriteStartObject();
                foreach (var value in message.Headers)
                {
                    writer.WritePropertyName(value.Key);
                    writer.WriteValue(value.Value);
                }
                writer.WriteEndObject();
            }
        }

        private void WriteInvocationMessage(InvocationMessage message, JsonTextWriter writer)
        {
            WriteInvocationId(message, writer);
            writer.WritePropertyName(TargetPropertyName);
            writer.WriteValue(message.Target);

            WriteArguments(message.Arguments, writer);
        }

        private void WriteArguments(object[] arguments, JsonTextWriter writer)
        {
            writer.WritePropertyName(ArgumentsPropertyName);
            writer.WriteStartArray();
            foreach (var argument in arguments)
            {
                PayloadSerializer.Serialize(writer, argument);
            }
            writer.WriteEndArray();
        }

        private static void WriteInvocationId(HubInvocationMessage message, JsonTextWriter writer)
        {
            if (!string.IsNullOrEmpty(message.InvocationId))
            {
                writer.WritePropertyName(InvocationIdPropertyName);
                writer.WriteValue(message.InvocationId);
            }
        }

        private static void WriteMessageType(JsonTextWriter writer, int type)
        {
            writer.WritePropertyName(TypePropertyName);
            writer.WriteValue(type);
        }

        internal static JsonSerializerSettings CreateDefaultSerializerSettings()
        {
            return new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
        }
    }
}
