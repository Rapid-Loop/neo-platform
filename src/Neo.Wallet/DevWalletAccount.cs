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
using Neo.SmartContract;
using Neo.Wallet.Json;
using System;

namespace Neo.Wallet
{
    public class DevWalletAccount : IWalletAccount<ProtocolSettings>, IMap<WalletAccountModel>
    {
        public ProtocolSettings ProtocolConfiguration => throw new NotImplementedException();

        public UInt160 ScriptHash => throw new NotImplementedException();

        public string? Label => throw new NotImplementedException();

        public bool IsDefault => throw new NotImplementedException();

        public bool IsLocked => throw new NotImplementedException();

        public bool HasKey => throw new NotImplementedException();

        public ProtocolSettings Extra => ProtocolConfiguration;

        public Contract Contract => throw new NotImplementedException();

        public bool ChangePassword(string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public bool VerifyPassword(string password)
        {
            throw new NotImplementedException();
        }

        public WalletAccountModel ToObject() =>
            new();
    }
}
