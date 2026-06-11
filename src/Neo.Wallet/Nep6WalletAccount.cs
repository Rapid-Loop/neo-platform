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
using Neo.Core.SmartContract;
using Neo.Cryptography;
using Neo.Cryptography.ECC;
using Neo.Cryptography.Extensions;
using Neo.SmartContract;
using Neo.Wallet.Cryptography;
using Neo.Wallet.Json;
using System;
using System.Linq;

namespace Neo.Wallet
{
    public class Nep6WalletAccount : IWalletAccount<ProtocolSettings>, IMap<WalletAccountModel>
    {
        public ProtocolSettings ProtocolConfiguration => _protocolSettings;

        public UInt160 ScriptHash => Contract.ScriptHash;

        public string Address => ScriptHash.ToAddress(_protocolSettings.AddressVersion);

        public string? Label { get; set; }

        public bool IsDefault { get; set; }

        public bool IsLocked { get; set; }

        public bool HasKey => _privateKeyBytes.Length > 0;

        public ProtocolSettings Extra => ProtocolConfiguration;

        public WitnessContract Contract => _witnessContract;

        private readonly ProtocolSettings _protocolSettings;
        private readonly WitnessContract _witnessContract;

        private readonly byte[] _privateKeyBytes = [];
        private readonly ScryptParameters _scryptParameters;

        private ProtectedString _password = string.Empty;

        public Nep6WalletAccount(ECPoint[] publicKeys, ProtocolSettings protocolSettings, ScryptParameters? scryptParameters = default) : this((int)Math.Ceiling((2 * publicKeys.Length + 1) / 3m), publicKeys, protocolSettings, scryptParameters) { }

        public Nep6WalletAccount(int m, ECPoint[] publicKeys, ProtocolSettings protocolSettings, ScryptParameters? scryptParameters = default)
        {
            ArgumentOutOfRangeException.ThrowIfEqual(publicKeys.Length, 0, nameof(publicKeys));
            ArgumentOutOfRangeException.ThrowIfGreaterThan(m, publicKeys.Length, nameof(publicKeys));
            ArgumentOutOfRangeException.ThrowIfLessThan(m, 1, nameof(m));
            ArgumentOutOfRangeException.ThrowIfGreaterThan(m, 1024, nameof(m));

            _scryptParameters = scryptParameters ?? ScryptParameters.Default;
            _protocolSettings = protocolSettings;
            _witnessContract = WitnessContract.CreateMultiSigContract(m, publicKeys);
        }

        public Nep6WalletAccount(UInt160 contractHash, ProtocolSettings protocolSettings, ContractParameterType[]? contractParameters = default, byte[]? privateKeyBytes = default, ScryptParameters? scryptParameters = default)
        {
            _scryptParameters = scryptParameters ?? ScryptParameters.Default;
            _protocolSettings = protocolSettings;
            _witnessContract = WitnessContract.Create(contractHash, contractParameters ?? []);

            if (privateKeyBytes is not null)
                _privateKeyBytes = privateKeyBytes;
        }

        public Nep6WalletAccount(byte[] privateKeyBytes, ProtocolSettings protocolSettings, ScryptParameters? scryptParameters = default)
        {
            _privateKeyBytes = privateKeyBytes;
            _scryptParameters = scryptParameters ?? ScryptParameters.Default;
            _protocolSettings = protocolSettings;

            var privateKeySpan = privateKeyBytes.AsSpan();
            var publicKeyPoint = ECPoint.FromPrivateKey(privateKeySpan, ECCurve.SecP256r1);

            _witnessContract = WitnessContract.CreateSignatureContract(publicKeyPoint);
        }

        public override string ToString() =>
            $"{ToObject()}";

        public bool ChangePassword(ProtectedString oldPassword, ProtectedString newPassword)
        {
            if (string.IsNullOrEmpty(oldPassword)) return false;
            if (string.IsNullOrEmpty(newPassword)) return false;
            if (oldPassword == newPassword) return false;

            _password.Dispose();

            _password = newPassword;

            return true;
        }

        public bool VerifyPassword(ProtectedString password) =>
            string.Equals(_password, password);

        public byte[] GetPrivateKey() =>
            _privateKeyBytes[..];

        public WalletAccountModel ToObject() =>
            new()
            {
                Address = Address,
                Contract = new()
                {
                    Deployed = HasKey,
                    Script = _witnessContract.Script,
                    Parameters = [
                        .. _witnessContract.ParameterList
                            .Select(static (s, i) =>
                                new ContractParameterModel()
                                {
                                    Name = $"Parameter{i}",
                                    Type = s,
                                }
                            ),
                        ],
                },
                Key = _privateKeyBytes[..],
                Label = Label,
                IsDefault = IsDefault,
                Lock = IsLocked,
                Extra = Extra.ToObject(),
            };
    }
}
