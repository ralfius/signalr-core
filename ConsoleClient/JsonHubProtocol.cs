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
        private const string ResultPropertyName = "result";
        private const string ItemPropertyName = "item";
        private const string InvocationIdPropertyName = "invocationId";
        private const string TypePropertyName = "type";
        private const string ErrorPropertyName = "error";
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
                        case StreamInvocationMessage m:
                            WriteMessageType(writer, HubProtocolConstants.StreamInvocationMessageType);
                            WriteHeaders(writer, m);
                            WriteStreamInvocationMessage(m, writer);
                            break;
                        case StreamItemMessage m:
                            WriteMessageType(writer, HubProtocolConstants.StreamItemMessageType);
                            WriteHeaders(writer, m);
                            WriteStreamItemMessage(m, writer);
                            break;
                        case CompletionMessage m:
                            WriteMessageType(writer, HubProtocolConstants.CompletionMessageType);
                            WriteHeaders(writer, m);
                            WriteCompletionMessage(m, writer);
                            break;
                        case CancelInvocationMessage m:
                            WriteMessageType(writer, HubProtocolConstants.CancelInvocationMessageType);
                            WriteHeaders(writer, m);
                            WriteCancelInvocationMessage(m, writer);
                            break;
                        case PingMessage _:
                            WriteMessageType(writer, HubProtocolConstants.PingMessageType);
                            break;
                        case CloseMessage m:
                            WriteMessageType(writer, HubProtocolConstants.CloseMessageType);
                            WriteCloseMessage(m, writer);
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

        private void WriteCompletionMessage(CompletionMessage message, JsonTextWriter writer)
        {
            WriteInvocationId(message, writer);
            if (!string.IsNullOrEmpty(message.Error))
            {
                writer.WritePropertyName(ErrorPropertyName);
                writer.WriteValue(message.Error);
            }
            else if (message.HasResult)
            {
                writer.WritePropertyName(ResultPropertyName);
                PayloadSerializer.Serialize(writer, message.Result);
            }
        }

        private void WriteCancelInvocationMessage(CancelInvocationMessage message, JsonTextWriter writer)
        {
            WriteInvocationId(message, writer);
        }

        private void WriteStreamItemMessage(StreamItemMessage message, JsonTextWriter writer)
        {
            WriteInvocationId(message, writer);
            writer.WritePropertyName(ItemPropertyName);
            PayloadSerializer.Serialize(writer, message.Item);
        }

        private void WriteInvocationMessage(InvocationMessage message, JsonTextWriter writer)
        {
            WriteInvocationId(message, writer);
            writer.WritePropertyName(TargetPropertyName);
            writer.WriteValue(message.Target);

            WriteArguments(message.Arguments, writer);
        }

        private void WriteStreamInvocationMessage(StreamInvocationMessage message, JsonTextWriter writer)
        {
            WriteInvocationId(message, writer);
            writer.WritePropertyName(TargetPropertyName);
            writer.WriteValue(message.Target);

            WriteArguments(message.Arguments, writer);
        }

        private void WriteCloseMessage(CloseMessage message, JsonTextWriter writer)
        {
            if (message.Error != null)
            {
                writer.WritePropertyName(ErrorPropertyName);
                writer.WriteValue(message.Error);
            }
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
