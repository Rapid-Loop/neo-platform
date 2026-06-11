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

using Neo.Configuration.Json;
using Neo.Core.Extensions;
using Neo.Cryptography;
using Neo.Cryptography.Extensions;
using Neo.IO;
using Neo.SmartContract;
using Neo.Wallet.Cryptography;
using Neo.Wallet.Json;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using ECCurve = Neo.Cryptography.ECC.ECCurve;
using ECPoint = Neo.Cryptography.ECC.ECPoint;

namespace Neo.Wallet
{
    public static class ChainWallet
    {
        public const int MaxPrivateKeySizeInBytes = 32;

        /// <summary>
        /// Decodes a private key from the specified WIF string.
        /// </summary>
        /// <param name="wifString">The WIF string to be decoded.</param>
        /// <returns>The decoded private key.</returns>
        public static byte[] GetKeyFromWifString(string wifString)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(wifString);

            var decodedWifBytes = Base58.DecodeCheck(wifString);

            if (decodedWifBytes.Length != 34 ||
                decodedWifBytes[0] != 0x80 ||
                decodedWifBytes[33] != 0x01)
                throw new FormatException("Invalid WIF key");

            return decodedWifBytes[1..^1];
        }

        public static byte[] GetKeyFromNep2String(string nep2String, string password, ScryptParameters scryptParameters, byte addressVersion = 53)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(nep2String, nameof(nep2String));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(password, nameof(password));

            var decodedBytes = Base58.DecodeCheck(nep2String);
            var decodedSpan = decodedBytes.AsSpan();

            if (decodedSpan.Length != 39 ||
                decodedSpan[0] != 0x01 ||
                decodedSpan[1] != 0x42 ||
                decodedSpan[2] != 0xe0)
                throw new FormatException("Invalid NEP-2 key");

            const int SCryptKeyLengthInBytes = 64;

            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var salt = decodedSpan.Slice(3, 4);

            var scrypt = SCrypt.Generate(passwordBytes, [.. salt], scryptParameters.N, scryptParameters.R, scryptParameters.P, SCryptKeyLengthInBytes);
            var derivedHalf1 = scrypt[..32];
            var derivedHalf2 = scrypt[32..];

            byte[] encryptedKeyBytes = [.. decodedSpan.Slice(7, 32)];
            var decryptedBytes = AesDecryptECB(encryptedKeyBytes, derivedHalf2);
            var privateKeyBytes = decryptedBytes.Xor(derivedHalf1);

            var publicKeyPoint = ECPoint.FromPrivateKey(privateKeyBytes, ECCurve.SecP256r1);
            var scriptHash = WitnessContract.CreateSignatureRedeemScript(publicKeyPoint).ToScriptHash();
            var address = scriptHash.ToAddress(addressVersion);

            var addressBytes = Encoding.UTF8.GetBytes(address);
            var addressHashBytes = SHA256.HashData(SHA256.HashData(addressBytes));

            if (addressHashBytes.AsSpan(0, 4).SequenceEqual(salt) == false)
                throw new InvalidDataException("NEP-2 key is not valid");

            return privateKeyBytes;
        }

        public static byte[] AesDecryptECB(byte[] inputBytes, byte[] keyBytes)
        {
            using var aes = Aes.Create();
            aes.Key = keyBytes;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.None;

            using var decryptor = aes.CreateDecryptor();
            return decryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
        }

        public static DevWallet OpenDevFile(FileInfo file) =>
            JsonModel.FromJson<DevWalletModel>(file)?.ToObject() ?? new();
    }
}
