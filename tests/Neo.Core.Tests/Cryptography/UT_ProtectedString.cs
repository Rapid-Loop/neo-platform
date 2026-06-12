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
using System.Linq;
using System.Text;

namespace Neo.Core.Tests.Cryptography
{
    [TestClass]
    public class UT_ProtectedString
    {
        [TestMethod]
        public void TestAll()
        {
            var expectedPasswordString = "password123";
            var expectedPasswordBytes = Encoding.UTF8.GetBytes(expectedPasswordString);

            var expectedPasswordHashCode = expectedPasswordBytes.Aggregate(
                expectedPasswordBytes.Length,
                (hash, b) => (hash * 31) ^ b
            );

            ProtectedString actualProtectedString = expectedPasswordString;

            Assert.IsFalse(string.IsNullOrWhiteSpace(actualProtectedString));

            Assert.AreNotEqual(expectedPasswordHashCode, actualProtectedString.GetHashCode());
            Assert.AreNotEqual(0, actualProtectedString.GetHashCode());

            Assert.AreEqual(actualProtectedString.GetHashCode(), actualProtectedString.GetHashCode());
            Assert.AreEqual<string>(expectedPasswordString, actualProtectedString);
            Assert.AreEqual<ProtectedString>(expectedPasswordString, actualProtectedString);

            Assert.AreEqual(0, string.Compare(expectedPasswordString, actualProtectedString));
            Assert.AreEqual(0, expectedPasswordString.CompareTo(actualProtectedString));

            Assert.IsTrue(actualProtectedString.Equals(expectedPasswordString));
            Assert.IsTrue(actualProtectedString == expectedPasswordString);
        }
    }
}
