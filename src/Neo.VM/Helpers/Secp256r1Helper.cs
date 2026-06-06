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
using System.Numerics;
using System.Security.Cryptography;
using static System.Security.Cryptography.ECCurve;

namespace Neo.VM.Helpers
{
    public static class Secp256r1Helper
    {
        private static readonly BigInteger s_p = BigInteger.Parse("115792089210356248762697446949407573530086143415290314195533631308867097853951");
        private static readonly BigInteger s_b = BigInteger.Parse("41058363725152142129326129780047268409114441015993725554835256314039467401291");

        public static byte[] GetPublicKey(byte[] privateKeyBytes, bool shouldCompress = false)
        {
            using var ecdsa = ECDsa.Create();

            var ecPrivateKeyParameters = new ECParameters
            {
                Curve = NamedCurves.nistP256,
                D = privateKeyBytes,
            };

            ecdsa.ImportParameters(ecPrivateKeyParameters);

            var publicKeyParameters = ecdsa.ExportParameters(false);

            byte[] uncompressedPublicKeyBytes = [
                0x04,
                .. publicKeyParameters.Q.X.AsSpan(),
                .. publicKeyParameters.Q.Y.AsSpan(),
            ];

            return shouldCompress ?
                CompressPublicKey(uncompressedPublicKeyBytes) :
                uncompressedPublicKeyBytes;
        }

        public static byte[] CompressPublicKey(byte[] uncompressedPublicKeyBytes)
        {
            var prefix = uncompressedPublicKeyBytes[^1] % 2 == 0 ?
                (byte)0x02 : (byte)0x03;

            return [
                prefix,
                .. uncompressedPublicKeyBytes[1..33]
            ];
        }

        public static byte[] DecompressPublicKey(byte[] compressedPublicKeyBytes)
        {
            if (compressedPublicKeyBytes.Length != 33 || (compressedPublicKeyBytes[0] != 0x02 && compressedPublicKeyBytes[0] != 0x03))
                throw new FormatException("Invalid compressed key format.");

            var xBytes = compressedPublicKeyBytes[1..33];

            var x = new BigInteger(xBytes, isUnsigned: true, isBigEndian: true);
            var xCubed = BigInteger.ModPow(x, 3, s_p);
            var z = (xCubed - (3 * x) + s_b) % s_p;

            if (z < 0) z += s_p;

            var y = BigInteger.ModPow(z, (s_p + 1) / 4, s_p);

            var isYEven = (y % 2 == 0);
            var shouldBeEven = compressedPublicKeyBytes[0] == 0x02;

            if (isYEven != shouldBeEven) y = s_p - y;

            var yBytes = y.ToByteArray(isUnsigned: true, isBigEndian: true);

            return [
                0x04,
                .. compressedPublicKeyBytes[1..33],
                .. yBytes,
            ];
        }
    }
}
