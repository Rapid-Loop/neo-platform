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
using Neo.Cryptography.ECC;
using System;

namespace Neo.Core.Tests.Cryptography.ECC
{
    [TestClass]
    public class UT_ECPoint
    {
        private const string PrivateKeyString = "0000000000000000000000000000000000000000000000000000000000000001";

        private readonly static byte[] s_privateKeyBytes = Convert.FromHexString(PrivateKeyString);

        [TestMethod]
        public void TestGeneratePublicKeyFromPrivateKey()
        {
            var expectedUncompressedPublicKeyString =
                "04"
                + "6b17d1f2e12c4247f8bce6e563a440f277037d812deb33a0f4a13945d898c296"
                + "4fe342e2fe1a7f9b8ee7eb4a7c0f9e162bce33576b315ececbb6406837bf51f5";

            var actualPublicPoint = ECPoint.FromPrivateKey(s_privateKeyBytes, ECCurve.SecP256r1);
            var actualUncompressedPublicKeyString = $"{actualPublicPoint}";

            Assert.AreEqual(expectedUncompressedPublicKeyString, actualUncompressedPublicKeyString);
        }

        [TestMethod]
        public void TestParsePoint()
        {
            var expectedPoint = ECPoint.FromPrivateKey(s_privateKeyBytes, ECCurve.SecP256r1);
            var expectedCompressedPointHexString = Convert.ToHexStringLower(expectedPoint.Encode(true));
            var expectedUncompressedPointHexString = Convert.ToHexStringLower(expectedPoint.Encode(false));

            var actualCompressedPoint = ECPoint.Parse(expectedCompressedPointHexString, ECCurve.SecP256r1);
            var actualUncompressedPoint = ECPoint.Parse(expectedUncompressedPointHexString, ECCurve.SecP256r1);

            var actualCompressedPointHexString = Convert.ToHexStringLower(actualCompressedPoint.Encode(true));
            var actualUncompressedPointHexString = Convert.ToHexStringLower(actualUncompressedPoint.Encode(false));

            Assert.AreEqual(expectedCompressedPointHexString, actualCompressedPointHexString);
            Assert.AreEqual(expectedUncompressedPointHexString, actualUncompressedPointHexString);

            var actualCompressedSuccessful = ECPoint.TryParse(expectedCompressedPointHexString, ECCurve.SecP256r1, out actualCompressedPoint);
            var actualUncompressedSuccessful = ECPoint.TryParse(expectedUncompressedPointHexString, ECCurve.SecP256r1, out actualUncompressedPoint);

            Assert.IsTrue(actualCompressedSuccessful);
            Assert.IsNotNull(actualCompressedPoint);
            Assert.AreEqual(expectedPoint.X, actualCompressedPoint.X);
            Assert.AreEqual(expectedPoint.Y, actualCompressedPoint.Y);
            Assert.AreEqual(expectedPoint.Size, actualCompressedPoint.Size);
            Assert.AreEqual(ECPoint.UncompressedLength, actualCompressedPoint.Length);
            Assert.AreEqual(expectedPoint.Curve.Name, actualCompressedPoint.Curve.Name);

            Assert.IsTrue(actualUncompressedSuccessful);
            Assert.IsNotNull(actualUncompressedPoint);
            Assert.AreEqual(expectedPoint.X, actualUncompressedPoint.X);
            Assert.AreEqual(expectedPoint.Y, actualUncompressedPoint.Y);
            Assert.AreEqual(expectedPoint.Size, actualUncompressedPoint.Size);
            Assert.AreEqual(ECPoint.UncompressedLength, actualUncompressedPoint.Length);
            Assert.AreEqual(expectedPoint.Curve.Name, actualUncompressedPoint.Curve.Name);
        }

        [TestMethod]
        public void TestEquatable()
        {
            var expectedPoint = ECPoint.FromPrivateKey(s_privateKeyBytes, ECCurve.SecP256r1);
            var actualPoint = ECPoint.FromPrivateKey(s_privateKeyBytes, ECCurve.SecP256r1);

            Assert.AreEqual(expectedPoint, actualPoint);
            Assert.IsTrue(expectedPoint == actualPoint);
            Assert.IsTrue(expectedPoint.Equals(actualPoint));
            Assert.IsTrue(expectedPoint.Equals((object)actualPoint));
        }

        [TestMethod]
        public void TestGetHashCode()
        {
            var expectedPoint = ECPoint.FromPrivateKey(s_privateKeyBytes, ECCurve.SecP256r1);
            var actualPoint1 = ECPoint.FromPrivateKey(s_privateKeyBytes, ECCurve.SecP256r1);
            var actualPoint2 = ECPoint.Parse("03" + Random.Shared.GetHexString(ECPoint.UncompressedLength - 1, true), ECCurve.SecP256r1);

            Assert.AreEqual(expectedPoint.GetHashCode(), actualPoint1.GetHashCode());
            Assert.AreNotEqual(expectedPoint.GetHashCode(), actualPoint2.GetHashCode());
            Assert.AreNotEqual(actualPoint1.GetHashCode(), actualPoint2.GetHashCode());
        }

        [TestMethod]
        public void TestCompareTo()
        {
            var expectedPoint = ECPoint.FromPrivateKey(s_privateKeyBytes, ECCurve.SecP256r1);
            var actualPoint1 = ECPoint.FromPrivateKey(s_privateKeyBytes, ECCurve.SecP256r1);
            var actualPoint2 = ECPoint.Parse("03" + Random.Shared.GetHexString(ECPoint.UncompressedLength - 1, true), ECCurve.SecP256r1);

            Assert.AreEqual(0, expectedPoint.CompareTo(actualPoint1));
            Assert.AreNotEqual(0, expectedPoint.CompareTo(actualPoint2));
        }

        [TestMethod]
        public void TestSerializable()
        {
            var expectedPoint1 = ECPoint.FromPrivateKey(s_privateKeyBytes, ECCurve.SecP256r1);
            var expectedPoint2 = ECPoint.Parse("03" + Random.Shared.GetHexString(ECPoint.UncompressedLength - 1, true), ECCurve.SecP256r1);
            var expectedPointLength = ECPoint.UncompressedLength;
            var expectedPointSize = expectedPointLength + 1;

            var actualPointBytes1 = expectedPoint1.ToArray();
            var actualPoint1 = actualPointBytes1.AsSerializable<ECPoint>();

            var actualPointBytes2 = expectedPoint2.ToArray();
            var actualPoint2 = actualPointBytes2.AsSerializable<ECPoint>();

            Assert.IsNotNull(actualPoint1);
            Assert.IsNotNull(actualPoint2);

            Assert.AreEqual(expectedPoint1, actualPoint1);
            Assert.AreEqual(expectedPoint2, actualPoint2);

            Assert.AreNotEqual(expectedPoint1, actualPoint2);
            Assert.AreNotEqual(expectedPoint2, actualPoint1);

            Assert.AreEqual(expectedPointSize, actualPoint1.Size);
            Assert.AreEqual(expectedPointSize, actualPoint2.Size);

            Assert.HasCount(expectedPointSize, actualPointBytes1);
            Assert.HasCount(expectedPointSize, actualPointBytes2);
        }
    }
}
