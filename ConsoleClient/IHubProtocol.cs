// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Buffers;

namespace Microsoft.AspNetCore.SignalR.Protocol
{
    /// <summary>
    /// A protocol abstraction for communicating with SignalR hubs.
    /// </summary>
    public interface IHubProtocol
    {
        /// <summary>
        /// Gets the name of the protocol. The name is used by SignalR to resolve the protocol between the client and server.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Writes the specified <see cref="HubMessage"/> to a writer.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="output">The output writer.</param>
        void WriteMessage(HubMessage message, IBufferWriter<byte> output);
    }
}
