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
using Neo.IO;
using Neo.Wallet.Json;
using System;
using System.IO;

namespace Neo.Wallet
{
    public static class ChainWallet
    {
        /// <summary>
        /// Decodes a private key from the specified WIF string.
        /// </summary>
        /// <param name="wif">The WIF string to be decoded.</param>
        /// <returns>The decoded private key.</returns>
        public static byte[] GetKeyFromWifString(string wif)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(wif);

            var data = Base58.DecodeCheck(wif);

            if (data.Length != 34 || data[0] != 0x80 || data[33] != 0x01)
                throw new FormatException("Invalid WIF key");

            var privateKey = new byte[32];

            Buffer.BlockCopy(data, 1, privateKey, 0, privateKey.Length);
            Array.Clear(data, 0, data.Length);

            return privateKey;
        }

        public static DevWallet OpenDevFile(FileInfo file) =>
            JsonModel.FromJson<DevWalletModel>(file)?.ToObject() ?? new();
    }
}
