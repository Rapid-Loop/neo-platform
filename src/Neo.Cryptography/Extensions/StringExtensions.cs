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

using Neo.IO;
using System;

namespace Neo.Cryptography.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Converts the specified address to a script hash.
        /// </summary>
        /// <param name="address">The address to convert.</param>
        /// <param name="version">The address version.</param>
        /// <returns>The converted script hash.</returns>
        public static UInt160 ToScriptHash(this string address, byte version)
        {
            var data = Base58.DecodeCheck(address);

            if (data.Length != 21)
                throw new FormatException($"Invalid address format: expected 21 bytes after Base58Check decoding, but got {data.Length} bytes. The address may be corrupted or in an invalid format.");

            if (data[0] != version)
                throw new FormatException($"Invalid address version: expected version {version}, but got {data[0]}. The address may be for a different network.");

            return new(data.AsSpan(1));
        }
    }
}
