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

using Neo.Core.Serialization;
using System;
using System.Runtime.CompilerServices;

namespace Neo.Core.Extensions
{
    public static class MemoryExtensions
    {
        public static int GetSerializedSize<TSource>(this Memory<TSource> memory)
            where TSource : unmanaged =>
            memory.Length.GetCompactSize() +
            (memory.Length * Unsafe.SizeOf<TSource>());

        public static int GetSerializedSize<TSource>(this ReadOnlyMemory<TSource> readOnlyMemory)
            where TSource : unmanaged =>
            readOnlyMemory.Length.GetCompactSize() +
            (readOnlyMemory.Length * Unsafe.SizeOf<TSource>());

        /// <summary>
        /// Converts a byte array to an <see cref="INeoSerializable"/> object.
        /// </summary>
        /// <typeparam name="T">The type to convert to.</typeparam>
        /// <param name="memory">The byte array to be converted.</param>
        /// <param name="startIndex"></param>
        /// <returns>The converted <see cref="INeoSerializable"/> object.</returns>
        public static T? AsSerializable<T>(this ReadOnlyMemory<byte> memory, int startIndex = 0)
            where T : class?, INeoSerializable? =>
            memory.ToArray()
                .AsSerializable<T>(startIndex);

        public static TAccumulate Aggregate<TSource, TAccumulate>(this Memory<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
        {
            var val = seed;

            ReadOnlySpan<TSource> readOnlySpan = source.Span;

            for (var i = 0; i < readOnlySpan.Length; i++)
            {
                var arg = readOnlySpan[i];
                val = func(val, arg);
            }

            return val;
        }

        public static TAccumulate Aggregate<TSource, TAccumulate>(this ReadOnlyMemory<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
        {
            var val = seed;

            for (var i = 0; i < source.Span.Length; i++)
            {
                var arg = source.Span[i];
                val = func(val, arg);
            }

            return val;
        }

        public static int ToHashCode(this Memory<byte> data) =>
            data.Aggregate(data.Length,
                (hash, b) =>
                        (hash * 31) ^ b
                );

        public static int ToHashCode(this ReadOnlyMemory<byte> data) =>
            data.Aggregate(data.Length,
                (hash, b) =>
                        (hash * 31) ^ b
                );
    }
}
