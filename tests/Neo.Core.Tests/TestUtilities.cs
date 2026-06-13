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

using Neo.Core.Blockchain;
using Neo.Core.VM;
using System;

namespace Neo.Core.Tests
{
    internal class TestUtilities
    {
        public static BlockHeader CreateBlockHeader(uint index, UInt256 prevHash) =>
            new()
            {
                PrevHash = prevHash,
                MerkleRoot = UInt256.Parse("0x6226416a0e5aca42b5566f5a19ab467692688ba9d47986f6981a7f747bba2772"),
                Timestamp = (ulong)((DateTimeOffset)new DateTime(2024, 06, 05, 0, 33, 1, 001, DateTimeKind.Utc)).ToUnixTimeMilliseconds(),
                Index = index,
                Nonce = 0,
                NextConsensus = UInt160.Zero,
                Witness = new Witness
                {
                    InvocationScript = [],
                    VerificationScript = [(byte)OpCode.PUSH1],
                }
            };
    }
}
