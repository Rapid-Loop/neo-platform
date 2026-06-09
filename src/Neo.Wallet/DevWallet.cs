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
using Neo.Configuration.Interfaces;
using Neo.Cryptography;
using Neo.Cryptography.ECC;
using Neo.Wallet.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Wallet
{
    public class DevWallet : IWallet<object, ProtocolSettings>, IMap<DevWalletModel>
    {
        private static readonly byte[] s_defaultSCryptBytes = SCrypt.Generate("neo", [1, 2, 3]);
        private static readonly Version s_walletVersion = new(1, 0);

        public Version Version => s_walletVersion;

        public string? Name { get; set; }

        public byte[] Scrypt => s_defaultSCryptBytes;

        public object? Extra => null;

        private readonly Dictionary<UInt160, DevWalletAccount> _walletAccounts = [];

        public DevWallet() { }

        public override string ToString() =>
            $"{ToObject()}";

        public bool Contains(UInt160 address) =>
            _walletAccounts.ContainsKey(address);

        public IWalletAccount<ProtocolSettings> CreateAccount(ProtocolSettings protocolSettings)
        {
            throw new NotImplementedException();
        }

        public IWalletAccount<ProtocolSettings> CreateAccount(byte[] privateKeyBytes, ProtocolSettings protocolSettings)
        {
            throw new NotImplementedException();
        }

        public IWalletAccount<ProtocolSettings> CreateAccount(string wifString, ProtocolSettings protocolSettings)
        {
            throw new NotImplementedException();
        }

        public IWalletAccount<ProtocolSettings> CreateAccount(UInt160 address, ProtocolSettings protocolSettings)
        {
            throw new NotImplementedException();
        }

        public IWalletAccount<ProtocolSettings> CreateMultiSigAccount(params ECPoint[] publicKeys)
        {
            throw new NotImplementedException();
        }

        public IWalletAccount<ProtocolSettings> CreateMultiSigAccount(int m, params ECPoint[] publicKeys)
        {
            throw new NotImplementedException();
        }

        public bool RemoveAccount(UInt160 address) =>
            _walletAccounts.Remove(address);

        public IWalletAccount<ProtocolSettings> GetAccount(UInt160 address) =>
            _walletAccounts[address];

        public IWalletAccount<ProtocolSettings> GetAccount(ECPoint publickKey)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IWalletAccount<ProtocolSettings>> GetAccounts() =>
            _walletAccounts.Values;

        public IEnumerable<IWalletAccount<ProtocolSettings>> GetNetworkAccounts(uint network)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IWalletAccount<ProtocolSettings>> GetContractAccounts()
        {
            throw new NotImplementedException();
        }

        public IWalletAccount<ProtocolSettings> GetDefaultAccount()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IWalletAccount<ProtocolSettings>> GetMultiSigAccounts()
        {
            throw new NotImplementedException();
        }

        public void SetDefaultAccount(UInt160 address)
        {
            throw new NotImplementedException();
        }

        public DevWalletModel ToObject() =>
            new()
            {
                Version = Version,
                Name = Name,
                SCrypt = SCryptModel.Default,
                Extra = Extra,
                Accounts = [.. _walletAccounts.Values.Select(static s => s.ToObject())],
            };
    }
}
