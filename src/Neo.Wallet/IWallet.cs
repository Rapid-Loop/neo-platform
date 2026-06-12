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

using Neo.Configuration;
using Neo.Core;
using Neo.Cryptography.ECC;
using Neo.Wallet.Cryptography;
using System;
using System.Collections.Generic;

namespace Neo.Wallet
{
    public interface IWallet<TExtras, TAccountExtras>
        where TExtras : class?, new()
        where TAccountExtras : class?, new()
    {
        Version Version { get; }

        string? Name { get; }

        ScryptParameters SCrypt { get; }

        TExtras? Extra { get; }

        bool Contains(UInt160 scriptHash);

        IWalletAccount<TAccountExtras> CreateAccount(ProtocolSettings protocolSettings);

        IWalletAccount<TAccountExtras> CreateAccount(byte[] privateKeyBytes, ProtocolSettings protocolSettings);

        IWalletAccount<TAccountExtras> CreateAccount(string wifString, ProtocolSettings protocolSettings);

        IWalletAccount<TAccountExtras> CreateAccount(UInt160 contractHash, ProtocolSettings protocolSettings);

        bool RemoveAccount(UInt160 scriptHash);

        void SetDefaultAccount(UInt160 scriptHash);

        IWalletAccount<TAccountExtras> GetDefaultAccount();

        IWalletAccount<TAccountExtras> GetAccount(UInt160 scriptHash);

        IWalletAccount<TAccountExtras> GetAccount(ECPoint publicKey);

        IEnumerable<IWalletAccount<TAccountExtras>> GetAccounts();

        IEnumerable<IWalletAccount<TAccountExtras>> GetNetworkAccounts(uint network);

        IEnumerable<IWalletAccount<TAccountExtras>> GetContractAccounts();

        IEnumerable<IWalletAccount<TAccountExtras>> GetMultiSigAccounts();

        IWalletAccount<TAccountExtras> CreateMultiSigAccount(ProtocolSettings protocolSettings, params ECPoint[] publicKeys);

        IWalletAccount<TAccountExtras> CreateMultiSigAccount(ProtocolSettings protocolSettings, int m, params ECPoint[] publicKeys);
    }
}
