// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.SignalR.Protocol
{
    public class CompletionMessage : HubInvocationMessage
    {
        public string Error { get; }
        public object Result { get; }
        public bool HasResult { get; }

        public CompletionMessage(string invocationId, string error, object result, bool hasResult)
            : base(invocationId)
        {
            if (error != null && result != null)
            {
                throw new ArgumentException($"Expected either '{nameof(error)}' or '{nameof(result)}' to be provided, but not both");
            }

            Error = error;
            Result = result;
            HasResult = hasResult;
        }
    }
}
