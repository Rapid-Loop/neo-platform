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

using Neo.IO.Extensions;
using System.Linq;
using System.Security.Cryptography;

namespace Neo.Cryptography.Tests
{
    [TestClass]
    public class UT_MerkleTree
    {
        [TestMethod]
        public void TestComputeRoot()
        {
            byte[] array1 = [0x01];
            var hash1 = new UInt256(SHA256.HashData(array1));

            byte[] array2 = [0x02];
            var hash2 = new UInt256(SHA256.HashData(array2));

            byte[] array3 = [0x03];
            var hash3 = new UInt256(SHA256.HashData(array3));

            UInt256[] hashes = [hash1, hash2, hash3];
            var expectedRootHash = MerkleTree.ComputeRoot(hashes);

            var hash4 = SHA256.HashData([.. hash1.ToArray(), .. hash2.ToArray()]);
            var hash5 = SHA256.HashData([.. hash3.ToArray(), .. hash3.ToArray()]);
            var actualResult = new UInt256(SHA256.HashData([.. hash4.ToArray(), .. hash5.ToArray()]));

            Assert.AreEqual(expectedRootHash, actualResult);
        }
    }
}
