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
using Neo.Core;
using Neo.Core.Cryptography;
using Neo.Core.Cryptography.ECC;
using Neo.Core.Extensions;
using Neo.Core.SmartContract;
using Neo.Wallet.Cryptography;
using Neo.Wallet.Json;
using System;
using System.Linq;
using System.Text;

namespace Neo.Wallet
{
    public class Nep6WalletAccount : IWalletAccount<ProtocolSettings>, IMap<Nep6WalletAccountModel>
    {
        public ProtocolSettings ProtocolConfiguration => _protocolSettings;

        public UInt160 ScriptHash => Contract.ScriptHash;

        public string Address => ScriptHash.ToAddress(_protocolSettings.AddressVersion);

        public string? Label { get; set; }

        public bool IsDefault { get; set; }

        public bool IsLocked => _isLocked;

        public bool HasKey => _privateKeyBytes.Length > 0;

        public ProtocolSettings Extra => ProtocolConfiguration;

        public WitnessContract Contract => _witnessContract;

        private readonly ProtocolSettings _protocolSettings;

        private readonly ScryptParameters _scryptParameters;

        private byte[] _privateKeyBytes = [];
        private bool _isLocked;

        private WitnessContract _witnessContract;
        private ProtectedString _password = string.Empty;
        private ProtectedString _nep2String = string.Empty;

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

            var privateKeySpan = _privateKeyBytes.AsSpan();
            var publicKeyPoint = ECPoint.FromPrivateKey(privateKeySpan, ECCurve.SecP256r1);

            _witnessContract = WitnessContract.CreateSignatureContract(publicKeyPoint);
        }

        public Nep6WalletAccount(string nep2String, string password, ProtocolSettings protocolSettings, ScryptParameters? scryptParameters = default)
        {
            _nep2String = nep2String;
            _password = password;
            _protocolSettings = protocolSettings;
            _scryptParameters = scryptParameters ?? ScryptParameters.Default;

            _privateKeyBytes = ChainWallet.GetKeyFromNep2String(nep2String, _password, _scryptParameters, _protocolSettings.AddressVersion);

            var privateKeySpan = _privateKeyBytes.AsSpan();
            var publicKeyPoint = ECPoint.FromPrivateKey(privateKeySpan, ECCurve.SecP256r1);

            _witnessContract = WitnessContract.CreateSignatureContract(publicKeyPoint);
        }

        public Nep6WalletAccount(Nep6WalletAccountModel accountModel, ScryptParameters? scryptParameters = default)
        {
            _nep2String = Encoding.UTF8.GetString(accountModel.Key ?? []);
            _protocolSettings = accountModel.Extra?.ToObject() ?? ProtocolSettings.Default;
            _scryptParameters = scryptParameters ?? ScryptParameters.Default;
            _witnessContract = accountModel.Contract?.ToObject() ?? WitnessContract.Create([], []);
            _isLocked = true;
        }

        public override string ToString() =>
            $"{ToObject()}";

        public bool ChangePassword(ProtectedString oldPassword, ProtectedString newPassword)
        {
            if (_isLocked) return false;
            if (_privateKeyBytes.Length == 0) return false;
            if (string.IsNullOrEmpty(newPassword)) return false;
            if (oldPassword == newPassword) return false;
            if (_password != oldPassword) return false;

            _password.Dispose();
            _password = newPassword;

            _nep2String = ChainWallet.ToNep2String(
                _privateKeyBytes,
                _password,
                _scryptParameters,
                _protocolSettings.AddressVersion
            );

            _isLocked = true;

            return true;
        }

        public bool VerifyPassword(ProtectedString password)
        {
            if (string.IsNullOrEmpty(password)) return false;
            if (string.IsNullOrEmpty(_nep2String))
                throw new InvalidOperationException($"Password is not set. Call {nameof(ChangePassword)} first.");

            var testKeyBytes = ChainWallet.GetKeyFromNep2String(
                    _nep2String,
                    password,
                    _scryptParameters,
                    _protocolSettings.AddressVersion);

            _password = password;
            _privateKeyBytes = testKeyBytes;
            _isLocked = false;

            var privateKeySpan = _privateKeyBytes.AsSpan();
            var publicKeyPoint = ECPoint.FromPrivateKey(privateKeySpan, ECCurve.SecP256r1);
            _witnessContract = WitnessContract.CreateSignatureContract(publicKeyPoint);

            return true;
        }

        public void SetLock() =>
            _isLocked = true;

        public byte[] GetPrivateKey()
        {
            if (_isLocked == true || _privateKeyBytes.Length == 0)
                throw new InvalidOperationException($"Account is locked. Call {nameof(VerifyPassword)} first.");

            return _privateKeyBytes[..];
        }

        public Nep6WalletAccountModel ToObject() =>
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
                Key = Encoding.UTF8.GetBytes(_nep2String),
                Label = Label,
                IsDefault = IsDefault,
                Lock = IsLocked,
                Extra = Extra.ToObject(),
            };
    }
}
