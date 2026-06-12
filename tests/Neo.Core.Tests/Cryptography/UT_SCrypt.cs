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

using Neo.Core.Cryptography;
using System;

namespace Neo.Core.Tests.Cryptography
{
    [TestClass]
    public class UT_SCrypt
    {
        [TestMethod]
        public void TestGenerate()
        {
            var expectedKeyString = "b6274d3a81892c24335ab46a08ec16d040ac00c5943b212099a44b76a9b8102631ab988fa07fb35357cee7b0e3910098c0774c0e97399997676d890b2bf2bb25";
            int n = 32, r = 2, p = 2;

            var actualKeyBytes = SCrypt.Generate([0x01, 0x02, 0x03], [0x04, 0x05, 0x06], n, r, p, 64);
            var actualKeyString = Convert.ToHexStringLower(actualKeyBytes);

            Assert.AreEqual(expectedKeyString, actualKeyString);
        }
    }
}
