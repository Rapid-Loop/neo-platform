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

using Neo.Core;
using Neo.Core.Cryptography.ECC;
using Neo.Core.Extensions;
using Neo.Core.Interfaces;
using Neo.Core.SmartContract;
using Neo.Wallet.Cryptography;
using Neo.Wallet.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using RandomNumberGenerator = System.Security.Cryptography.RandomNumberGenerator;

namespace Neo.Wallet
{
    public class Nep6Wallet : IWallet<object, ProtocolSettings>, IMap<Nep6WalletModel>
    {
        private static readonly Version s_walletVersion = new(1, 0);

        public Version Version => s_walletVersion;

        public string? Name { get; set; }

        public ScryptParameters SCrypt => _scryptParameters;

        public object? Extra { get; set; }

        private readonly Dictionary<UInt160, Nep6WalletAccount> _walletAccounts = [];
        private readonly ScryptParameters _scryptParameters;

        public Nep6Wallet(ScryptParameters? scryptParameters = default)
        {
            _scryptParameters = scryptParameters ?? ScryptParameters.Default;
        }

        public Nep6Wallet(Nep6WalletModel nep6WalletModel)
        {
            _scryptParameters = nep6WalletModel.SCrypt?.ToObject() ?? ScryptParameters.Default;

            if (nep6WalletModel.Accounts is not null)
            {
                foreach (var accountModel in nep6WalletModel.Accounts)
                {
                    if (accountModel.Key is null) continue;
                    if (accountModel.Contract is null) continue;

                    var account = new Nep6WalletAccount(accountModel, _scryptParameters);

                    _walletAccounts[account.Address] = account;
                }
            }
        }

        public bool Contains(UInt160 scriptHash) =>
            _walletAccounts.ContainsKey(scriptHash);

        public IWalletAccount<ProtocolSettings> CreateAccount(Nep6WalletAccountModel walletAccountModel)
        {
            if (walletAccountModel.Address is null)
                throw new InvalidOperationException();

            var account = new Nep6WalletAccount(walletAccountModel, _scryptParameters);

            return _walletAccounts[walletAccountModel.Address] = account;
        }

        public IWalletAccount<ProtocolSettings> CreateAccount(ProtocolSettings protocolSettings) =>
            CreateAccount(RandomNumberGenerator.GetBytes(ChainWallet.MaxPrivateKeySizeInBytes), protocolSettings);

        public IWalletAccount<ProtocolSettings> CreateAccount(byte[] privateKeyBytes, ProtocolSettings protocolSettings)
        {
            var newWalletAccount = new Nep6WalletAccount(privateKeyBytes, protocolSettings, scryptParameters: _scryptParameters);

            if (Contains(newWalletAccount.ScriptHash))
                throw new InvalidOperationException();

            return _walletAccounts[newWalletAccount.ScriptHash] = newWalletAccount;
        }

        public IWalletAccount<ProtocolSettings> CreateAccount(string wifString, ProtocolSettings protocolSettings) =>
            CreateAccount(ChainWallet.GetKeyFromWifString(wifString), protocolSettings);

        public IWalletAccount<ProtocolSettings> CreateAccount(UInt160 contractHash, ProtocolSettings protocolSettings)
        {
            var newWalletAccount = new Nep6WalletAccount(contractHash, protocolSettings, scryptParameters: _scryptParameters);

            if (Contains(newWalletAccount.ScriptHash))
                throw new InvalidOperationException();

            return _walletAccounts[newWalletAccount.ScriptHash] = newWalletAccount;
        }

        public IWalletAccount<ProtocolSettings> CreateMultiSigAccount(ProtocolSettings protocolSettings, params ECPoint[] publicKeys) =>
            CreateMultiSigAccount(protocolSettings, (int)Math.Ceiling((2 * publicKeys.Length + 1) / 3m), publicKeys);

        public IWalletAccount<ProtocolSettings> CreateMultiSigAccount(ProtocolSettings protocolSettings, int m, params ECPoint[] publicKeys)
        {
            var newWalletAccount = new Nep6WalletAccount(m, publicKeys, protocolSettings, _scryptParameters);

            if (Contains(newWalletAccount.ScriptHash))
                throw new InvalidOperationException();

            return _walletAccounts[newWalletAccount.ScriptHash] = newWalletAccount;
        }

        public IWalletAccount<ProtocolSettings> GetAccount(UInt160 scriptHash) =>
            _walletAccounts[scriptHash];

        public IWalletAccount<ProtocolSettings> GetAccount(ECPoint publicKey) =>
            _walletAccounts[publicKey.Encode(true).ToScriptHash()];

        public IEnumerable<IWalletAccount<ProtocolSettings>> GetAccounts() =>
            _walletAccounts.Values;

        public IEnumerable<IWalletAccount<ProtocolSettings>> GetContractAccounts() =>
            _walletAccounts.Values.Where(
                static s =>
                    s.Contract.Script.Length == 0
            );

        public IWalletAccount<ProtocolSettings> GetDefaultAccount() =>
            _walletAccounts.Values.Single(static s => s.IsDefault);

        public IEnumerable<IWalletAccount<ProtocolSettings>> GetMultiSigAccounts() =>
            _walletAccounts.Values.Where(
                static w =>
                    WitnessContract.IsMultiSigContract(w.Contract.Script)
            );

        public IEnumerable<IWalletAccount<ProtocolSettings>> GetNetworkAccounts(uint network) =>
            _walletAccounts.Values.Where(s => s.ProtocolConfiguration.Network == network);

        public bool RemoveAccount(UInt160 scriptHash) =>
            _walletAccounts.Remove(scriptHash);

        public void SetDefaultAccount(UInt160 scriptHash)
        {
            if (Contains(scriptHash))
                throw new KeyNotFoundException(nameof(scriptHash));

            var defaultWalletAccounts = _walletAccounts.Values.Where(s => s.IsDefault);

            foreach (var account in defaultWalletAccounts)
                account.IsDefault = false;

            _walletAccounts[scriptHash].IsDefault = true;
        }

        public Nep6WalletModel ToObject() =>
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
