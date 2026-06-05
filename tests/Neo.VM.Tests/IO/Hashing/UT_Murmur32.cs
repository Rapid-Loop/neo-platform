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

using Neo.VM.IO.Hashing;

namespace Neo.VM.Tests.IO.Hashing
{
    [TestClass]
    public class UT_Murmur32
    {
        private const uint DefaultTestSeed = 10u;

        [TestMethod]
        public void TestHashToUInt32()
        {
            var expectedHash = 1183556631u;
            var actualHash = Murmur32.HashToUInt32("NEO", DefaultTestSeed);

            Assert.AreEqual(expectedHash, actualHash);

            expectedHash = 378574820u;
            actualHash = Murmur32.HashToUInt32([1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1,], DefaultTestSeed);

            Assert.AreEqual(expectedHash, actualHash);
        }
    }
}
