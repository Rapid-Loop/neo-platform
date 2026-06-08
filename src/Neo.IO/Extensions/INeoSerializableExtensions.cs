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

using Neo.IO.Serialization;
using System.IO;

namespace Neo.IO.Extensions
{
    public static class INeoSerializableExtensions
    {
        /// <summary>
        /// Converts an <see cref="INeoSerializable"/> object to a byte array.
        /// </summary>
        /// <param name="obj">The <see cref="INeoSerializable"/> object to be converted.</param>
        /// <returns>The converted byte array.</returns>
        public static byte[] ToArray(this INeoSerializable obj)
        {
            using var ms = new MemoryStream();

            obj.Serialize(ms);
            return ms.ToArray();
        }
    }
}
