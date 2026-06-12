// BSD 2-Clause License
//
// Copyright (c) 2026, Rapid Loop
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//
// 1. Redistributions of source code must retain the above copyright notice, this
//    list of conditions and the following disclaimer.
//
// 2. Redistributions in binary form must reproduce the above copyright notice,
//    this list of conditions and the following disclaimer in the documentation
//    and/or other materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES

using System;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Neo.Core.Extensions
{
    public static class StreamExtensions
    {
        public static void WriteCompact<T>(this Stream stream, T value)
            where T : unmanaged, IBinaryInteger<T>
        {
            if (stream.CanWrite == false)
                throw new NotSupportedException("Stream does not support writing.");

            var v = Convert.ToUInt64(value);

            switch (v)
            {
                case < 0xfd:
                    stream.Write((byte)v);
                    break;
                case <= ushort.MaxValue:
                    stream.Write((byte)0xfd);
                    stream.Write((ushort)v);
                    break;
                case <= uint.MaxValue:
                    stream.Write((byte)0xfe);
                    stream.Write((uint)v);
                    break;
                default:
                    stream.Write((byte)0xff);
                    stream.Write(v);
                    break;
            }
        }

        public static void Write<T>(this Stream stream, T value)
            where T : unmanaged
        {
            if (stream.CanWrite == false)
                throw new NotSupportedException("Stream does not support writing.");

            var tSpan = MemoryMarshal.CreateSpan(ref value, 1);
            var span = MemoryMarshal.AsBytes(tSpan);

            stream.Write(span);
        }

        public static void Write<T>(this Stream stream, ref T value)
            where T : unmanaged
        {
            if (stream.CanWrite == false)
                throw new NotSupportedException("Stream does not support writing.");

            var tSpan = MemoryMarshal.CreateSpan(ref value, 1);
            var span = MemoryMarshal.AsBytes(tSpan);

            stream.Write(span);
        }

        public static void Write<T>(this Stream stream, T[] values)
            where T : unmanaged
        {
            if (stream.CanWrite == false)
                throw new NotSupportedException("Stream does not support writing.");

            var tSpan = values.AsSpan();
            var span = MemoryMarshal.AsBytes(tSpan);

            stream.WriteCompact(values.Length);
            stream.Write(span);
        }

        public static void Write<T>(this Stream stream, Span<T> values)
            where T : unmanaged
        {
            if (stream.CanWrite == false)
                throw new NotSupportedException("Stream does not support writing.");

            var span = MemoryMarshal.AsBytes(values);

            stream.WriteCompact(values.Length);
            stream.Write(span);
        }

        public static void Write(this Stream stream, string value)
        {
            if (stream.CanWrite == false)
                throw new NotSupportedException("Stream does not support writing.");

            var encoding = CoreUtilities.StrictUtf8Encoding;
            var valueSpan = value.AsSpan();
            var encodedLength = encoding.GetByteCount(valueSpan);

            Span<byte> byteSpan = stackalloc byte[encodedLength];
            var byteLength = encoding.GetBytes(valueSpan, byteSpan);

            stream.WriteCompact(byteLength);
            stream.Write(byteSpan);
        }

        public static void Write(this Stream stream, char[] value)
        {
            if (stream.CanWrite == false)
                throw new NotSupportedException("Stream does not support writing.");

            var encoding = CoreUtilities.StrictUtf8Encoding;
            var valueSpan = value.AsSpan();
            var encodedLength = encoding.GetByteCount(valueSpan);

            Span<byte> byteSpan = stackalloc byte[encodedLength];
            var byteLength = encoding.GetBytes(valueSpan, byteSpan);

            stream.WriteCompact(byteLength);
            stream.WriteCompact(value.Length);
            stream.Write(byteSpan);
        }

        public static T ReadCompact<T>(this Stream stream) where T : unmanaged, IBinaryInteger<T>
        {
            if (stream.CanRead == false)
                throw new NotSupportedException("Stream does not support reading.");

            var prefixByte = stream.ReadByte();

            if (prefixByte == -1)
                throw new EndOfStreamException("Unexpected end of stream while reading CompactSize.");

            var flag = (byte)prefixByte;

            var value = flag switch
            {
                < 0xfd => flag,
                0xfd => stream.Read<ushort>(),
                0xfe => stream.Read<uint>(),
                0xff => stream.Read<ulong>(),
            };

            return T.CreateChecked(value); // Throws on overflow for the target type
        }

        public static ref T Read<T>(this Stream stream, ref T result)
            where T : unmanaged
        {
            if (stream.CanRead == false)
                throw new NotSupportedException("Stream does not support reading.");

            var tSpan = MemoryMarshal.CreateSpan(ref result, 1);
            var span = MemoryMarshal.AsBytes(tSpan);

            stream.ReadExactly(span);

            return ref result;
        }

        public static T Read<T>(this Stream stream)
            where T : unmanaged
        {
            if (stream.CanRead == false)
                throw new NotSupportedException("Stream does not support reading.");

            var result = default(T);
            var tSpan = MemoryMarshal.CreateSpan(ref result, 1);
            var span = MemoryMarshal.AsBytes(tSpan);

            stream.ReadExactly(span);

            return result;
        }

        public static char[] ReadCharArray(this Stream stream)
        {
            if (stream.CanRead == false)
                throw new NotSupportedException("Stream does not support reading.");

            var byteLength = stream.ReadCompact<int>();
            var charLength = stream.ReadCompact<int>();

            Span<byte> span = stackalloc byte[byteLength];
            stream.ReadExactly(span);

            var results = new char[charLength];
            var charSpan = results.AsSpan();
            var encoding = CoreUtilities.StrictUtf8Encoding;
            _ = encoding.GetChars(span, charSpan);

            return results;
        }

        public static string ReadString(this Stream stream)
        {
            if (stream.CanRead == false)
                throw new NotSupportedException("Stream does not support reading.");

            var encoding = CoreUtilities.StrictUtf8Encoding;
            var byteLength = stream.ReadCompact<int>();

            Span<byte> bytes = stackalloc byte[byteLength];
            stream.ReadExactly(bytes);

            return encoding.GetString(bytes);
        }

        public static T[] ReadArray<T>(this Stream stream)
            where T : unmanaged
        {
            if (stream.CanRead == false)
                throw new NotSupportedException("Stream does not support reading.");

            if (typeof(T) == typeof(char))
                throw new NotImplementedException($"\'{typeof(T).FullName}\' type is not supported.");

            var length = stream.ReadCompact<int>();
            var results = new T[length];

            var tSpan = results.AsSpan();
            var span = MemoryMarshal.AsBytes(tSpan);
            stream.ReadExactly(span);

            return results;
        }
    }
}
