// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Buffers;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.AspNetCore.Internal
{
    internal static class JsonUtils
    {
        internal static JsonTextWriter CreateJsonTextWriter(TextWriter textWriter)
        {
            var writer = new JsonTextWriter(textWriter);
            writer.ArrayPool = JsonArrayPool<char>.Shared;
            // Don't close the output, leave closing to the caller
            writer.CloseOutput = false;

            // SignalR will always write a complete JSON response
            // This setting will prevent an error during writing be hidden by another error writing on dispose
            writer.AutoCompleteOnClose = false;

            return writer;
        }

        private class JsonArrayPool<T> : IArrayPool<T>
        {
            private readonly ArrayPool<T> _inner;

            internal static readonly JsonArrayPool<T> Shared = new JsonArrayPool<T>(ArrayPool<T>.Shared);

            public JsonArrayPool(ArrayPool<T> inner)
            {
                _inner = inner;
            }

            public T[] Rent(int minimumLength)
            {
                return _inner.Rent(minimumLength);
            }

            public void Return(T[] array)
            {
                _inner.Return(array);
            }
        }
    }
}
