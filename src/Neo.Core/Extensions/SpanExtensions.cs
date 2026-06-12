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
    public static class SpanExtensions
    {
        public static int GetByteCount<T>(this Span<T> span)
            where T : unmanaged =>
            span.Length.GetCompactSize() +
            (span.Length * Unsafe.SizeOf<T>());

        public static int GetByteCount<T>(this ReadOnlySpan<T> readOnlySpan)
            where T : unmanaged =>
            readOnlySpan.Length.GetCompactSize() +
            (readOnlySpan.Length * Unsafe.SizeOf<T>());

        /// <summary>
        /// Converts a byte array to an <see cref="INeoSerializable"/> object.
        /// </summary>
        /// <typeparam name="T">The type to convert to.</typeparam>
        /// <param name="span">The byte array to be converted.</param>
        /// <param name="startIndex"></param>
        /// <returns>The converted <see cref="INeoSerializable"/> object.</returns>
        public static T? AsSerializable<T>(this ReadOnlySpan<byte> span, int startIndex = 0)
            where T : class?, INeoSerializable? =>
            span.ToArray()
                .AsSerializable<T>(startIndex);
    }
}
