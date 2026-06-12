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
using Neo.IO;
using Neo.Wallet.Cryptography;
using Neo.Wallet.Json;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

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

        public static string ToNep2String(byte[] privateKeyBytes, string password, ScryptParameters? scryptParameters = default, byte addressVersion = 53)
        {
            const int SCryptKeyLengthInBytes = 64;
            const int SaltLengthInBytes = 4;

            scryptParameters ??= ScryptParameters.Default;

            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var salt = RandomNumberGenerator.GetBytes(SaltLengthInBytes);

            var scrypt = SCrypt.Generate(passwordBytes, salt, scryptParameters.N, scryptParameters.R, scryptParameters.P, SCryptKeyLengthInBytes);
            var derivedHalf1 = scrypt[..32];
            var derivedHalf2 = scrypt[32..];

            var ciphertextBytes = AesEncryptECB(privateKeyBytes.Xor(derivedHalf1), derivedHalf2);

            byte[] nep2StringBytes = [
                0x01, 0x42, 0xe0,
                .. salt,
                .. ciphertextBytes,
            ];

            return Base58.EncodeCheck(nep2StringBytes);
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
            const int SaltLengthInBytes = 4;

            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var salt = decodedSpan.Slice(3, SaltLengthInBytes);

            var scrypt = SCrypt.Generate(passwordBytes, [.. salt], scryptParameters.N, scryptParameters.R, scryptParameters.P, SCryptKeyLengthInBytes);
            var derivedHalf1 = scrypt[..32];
            var derivedHalf2 = scrypt[32..];

            byte[] encryptedKeyBytes = [.. decodedSpan.Slice(7, 32)];
            var decryptedBytes = AesDecryptECB(encryptedKeyBytes, derivedHalf2);
            var privateKeyBytes = decryptedBytes.Xor(derivedHalf1);

            return privateKeyBytes;
        }

        public static byte[] AesDecryptECB(byte[] ciphertextBytes, byte[] keyBytes)
        {
            using var aes = Aes.Create();
            aes.Key = keyBytes;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.None;

            using var decryptor = aes.CreateDecryptor();
            return decryptor.TransformFinalBlock(ciphertextBytes, 0, ciphertextBytes.Length);
        }

        public static byte[] AesEncryptECB(byte[] plaintextBytes, byte[] keyBytes)
        {
            using var aes = Aes.Create();
            aes.Key = keyBytes;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.None;

            using var encryptor = aes.CreateEncryptor();
            return encryptor.TransformFinalBlock(plaintextBytes, 0, plaintextBytes.Length);
        }

        public static DevWallet OpenDevWalletFile(FileInfo file) =>
            JsonModel.FromJson<DevWalletModel>(file)!.ToObject();

        public static Nep6Wallet? OpenNep6WalletFie(FileInfo file) =>
            JsonModel.FromJson<Nep6WalletModel>(file)!.ToObject();
    }
}
