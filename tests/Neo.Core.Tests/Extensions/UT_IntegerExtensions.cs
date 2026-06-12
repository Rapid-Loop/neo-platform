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

using Neo.Core.Extensions;

namespace Neo.Core.Tests.Extensions
{
    [TestClass]
    public class UT_IntegerExtensions
    {
        [TestMethod]
        public void TestGetCompactedSize()
        {
            byte expectedByte = 0xdd;
            ushort expectedUShort = 0xfffe;
            var expectedUInt = 0xfffffffeu;
            var expectedULong = 0xfffffffffffffffeu;

            ulong expectedULongLong = 0xdeadc0deu;

            var actualByte = expectedByte.GetCompactSize();
            var actualUShort = expectedUShort.GetCompactSize();
            var actualUInt = expectedUInt.GetCompactSize();
            var actualULong = expectedULong.GetCompactSize();
            var actualULongLong = expectedULongLong.GetCompactSize();

            Assert.AreEqual(1, actualByte);
            Assert.AreEqual(3, actualUShort);
            Assert.AreEqual(5, actualUInt);
            Assert.AreEqual(9, actualULong);
            Assert.AreEqual(5, actualULongLong);
        }
    }
}
