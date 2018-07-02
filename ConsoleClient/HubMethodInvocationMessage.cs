// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNetCore.SignalR.Protocol
{
    /// <summary>
    /// A base class for hub messages representing an invocation.
    /// </summary>
    public abstract class HubMethodInvocationMessage : HubInvocationMessage
    {
        /// <summary>
        /// Gets the target method name.
        /// </summary>
        public string Target { get; }

        /// <summary>
        /// Gets the target method arguments.
        /// </summary>
        public object[] Arguments { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HubMethodInvocationMessage"/> class.
        /// </summary>
        /// <param name="invocationId">The invocation ID.</param>
        /// <param name="target">The target method name.</param>
        /// <param name="arguments">The target method arguments.</param>
        protected HubMethodInvocationMessage(string invocationId, string target, object[] arguments)
            : base(invocationId)
        {
            if (string.IsNullOrEmpty(target))
            {
                throw new ArgumentNullException(nameof(target));
            }

            Target = target;
            Arguments = arguments;
        }
    }

    /// <summary>
    /// A hub message representing a non-streaming invocation.
    /// </summary>
    public class InvocationMessage : HubMethodInvocationMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvocationMessage"/> class.
        /// </summary>
        /// <param name="target">The target method name.</param>
        /// <param name="arguments">The target method arguments.</param>
        public InvocationMessage(string target, object[] arguments)
            : base(null, target, arguments)
        {
        }
    }
}
