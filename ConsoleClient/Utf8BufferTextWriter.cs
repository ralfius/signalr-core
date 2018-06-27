// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.AspNetCore.Internal
{
    internal sealed class Utf8BufferTextWriter : TextWriter
    {
        private static readonly UTF8Encoding _utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        [ThreadStatic]
        private static Utf8BufferTextWriter _cachedInstance;

        private readonly Encoder _encoder;
        private IBufferWriter<byte> _bufferWriter;
        private Memory<byte> _memory;
        private int _memoryUsed;

#if DEBUG
        private bool _inUse;
#endif

        public override Encoding Encoding => _utf8NoBom;

        public static Utf8BufferTextWriter Get(IBufferWriter<byte> bufferWriter)
        {
            var writer = _cachedInstance;
            if (writer == null)
            {
                writer = new Utf8BufferTextWriter();
            }

            // Taken off the the thread static
            _cachedInstance = null;
#if DEBUG
            if (writer._inUse)
            {
                throw new InvalidOperationException("The writer wasn't returned!");
            }

            writer._inUse = true;
#endif
            writer.SetWriter(bufferWriter);
            return writer;
        }

        public static void Return(Utf8BufferTextWriter writer)
        {
            _cachedInstance = writer;

            writer._encoder.Reset();
            writer._memory = Memory<byte>.Empty;
            writer._memoryUsed = 0;
            writer._bufferWriter = null;

#if DEBUG
            writer._inUse = false;
#endif
        }

        public void SetWriter(IBufferWriter<byte> bufferWriter)
        {
            _bufferWriter = bufferWriter;
        }
    }
}
