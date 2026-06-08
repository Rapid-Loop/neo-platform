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

namespace Neo.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Trims the specified prefix from the start of the <see cref="string"/>, ignoring case.
        /// </summary>
        /// <param name="value">The <see cref="string"/> to trim.</param>
        /// <param name="prefix">The prefix to trim.</param>
        /// <returns>
        /// The trimmed ReadOnlySpan without prefix. If no prefix is found, the input is returned unmodified.
        /// </returns>
        public static ReadOnlySpan<char> TrimStartIgnoreCase(this ReadOnlySpan<char> value, ReadOnlySpan<char> prefix)
        {
            if (value.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
                return value[prefix.Length..];
            return value;
        }
    }
}
