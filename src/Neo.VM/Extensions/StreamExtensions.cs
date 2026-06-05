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
using System.Runtime.InteropServices;

namespace Neo.VM.Extensions
{
    public static class StreamExtensions
    {
        public static void Write<T>(this Stream stream, T value)
            where T : unmanaged
        {
            var tSpan = MemoryMarshal.CreateSpan(ref value, 1);
            var span = MemoryMarshal.AsBytes(tSpan);

            stream.Write(span);
        }

        public static void Write<T>(this Stream stream, ref T value)
            where T : unmanaged
        {
            var tSpan = MemoryMarshal.CreateSpan(ref value, 1);
            var span = MemoryMarshal.AsBytes(tSpan);

            stream.Write(span);
        }

        public static void Write<T>(this Stream stream, T[] values)
            where T : unmanaged
        {
            var tSpan = values.AsSpan();
            var span = MemoryMarshal.AsBytes(tSpan);

            stream.Write(values.Length);
            stream.Write(span);
        }

        public static void Write<T>(this Stream stream, Span<T> values)
            where T : unmanaged
        {
            var span = MemoryMarshal.AsBytes(values);

            stream.Write(values.Length);
            stream.Write(span);
        }

        public static void Write(this Stream stream, string value)
        {
            var encoding = VMUility.StrictUtf8Encoding;
            var valueSpan = value.AsSpan();
            var encodedLength = encoding.GetByteCount(valueSpan);

            Span<byte> byteSpan = stackalloc byte[encodedLength];
            var byteLength = encoding.GetBytes(valueSpan, byteSpan);

            stream.Write(byteLength);
            stream.Write(byteSpan);
        }

        public static void Write(this Stream stream, char[] value)
        {
            var encoding = VMUility.StrictUtf8Encoding;
            var valueSpan = value.AsSpan();
            var encodedLength = encoding.GetByteCount(valueSpan);

            Span<byte> byteSpan = stackalloc byte[encodedLength];
            var byteLength = encoding.GetBytes(valueSpan, byteSpan);

            stream.Write(byteLength);
            stream.Write(value.Length);
            stream.Write(byteSpan);
        }

        public static ref T Read<T>(this Stream stream, ref T result)
            where T : unmanaged
        {
            var tSpan = MemoryMarshal.CreateSpan(ref result, 1);
            var span = MemoryMarshal.AsBytes(tSpan);

            stream.ReadExactly(span);

            return ref result;
        }

        public static T Read<T>(this Stream stream)
            where T : unmanaged
        {
            var result = default(T);
            var tSpan = MemoryMarshal.CreateSpan(ref result, 1);
            var span = MemoryMarshal.AsBytes(tSpan);

            stream.ReadExactly(span);

            return result;
        }

        public static char[] ReadCharArray(this Stream stream)
        {
            var byteLength = stream.Read<int>();
            var charLength = stream.Read<int>();

            Span<byte> span = stackalloc byte[byteLength];
            stream.ReadExactly(span);

            var results = new char[charLength];
            var charSpan = results.AsSpan();
            var encoding = VMUility.StrictUtf8Encoding;
            _ = encoding.GetChars(span, charSpan);

            return results;
        }

        public static string ReadString(this Stream stream)
        {
            var encoding = VMUility.StrictUtf8Encoding;
            var byteLength = stream.Read<int>();

            Span<byte> bytes = stackalloc byte[byteLength];
            stream.ReadExactly(bytes);

            return encoding.GetString(bytes);
        }

        public static T[] ReadArray<T>(this Stream stream)
            where T : unmanaged
        {
            if (typeof(T) == typeof(char))
                throw new NotImplementedException($"\'{typeof(T).FullName}\' type is not supported.");

            var length = stream.Read<int>();
            var results = new T[length];

            var tSpan = results.AsSpan();
            var span = MemoryMarshal.AsBytes(tSpan);
            stream.ReadExactly(span);

            return results;
        }
    }
}
