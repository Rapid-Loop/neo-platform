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

using Neo.Configuration.Json.Converters;
using Neo.Core;
using Neo.Core.VM.SmartContract;
using Neo.Wallet.Json;
using System;
using System.Collections.Immutable;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Neo.Wallet.Tests
{
    internal static class TestDefaults
    {
        public static readonly JsonSerializerOptions JsonDefaultSerializerOptions = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            ReadCommentHandling = JsonCommentHandling.Disallow,
            UnknownTypeHandling = JsonUnknownTypeHandling.JsonNode,
            PreferredObjectCreationHandling = JsonObjectCreationHandling.Replace,
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip,
            WriteIndented = false,
            RespectNullableAnnotations = false,
            Converters =
            {
                // TODO: Make sure you add the same converters from JsonDefaults.SerializerOptions.Converters
                // NOTE: JsonConverterAttribute overrides these converters
                new JsonStringEnumConverter(),
                new JsonStringECPointConverter(),
                new JsonStringUInt160Converter(),
            }
        };

        public static readonly DevWalletModel TestDevWalletModel = new()
        {
            Name = "Unit Test Wallet",
            Version = new(1, 0),
            SCrypt = SCryptModel.Default,
            Accounts = [
                new DevWalletAccountModel()
                {
                    Address = "0xce45fca32b8cd071bfbc20389c20cd7025f85ff0",
                    IsDefault = true,
                    Label = "Main Test Account",
                    Lock = false,
                    Key = ChainWallet.GetKeyFromWifString("Ky7cYncUA92kWnh7xymshpfgz7QiX46qPWCQBQPVUSv5vndE2VTR"),
                    Contract = new()
                    {
                        Deployed = false,
                        Script = Convert.FromBase64String("DCECjNhSCkN5\u002BL\u002BEc0/cgGPMgQkyrl8V2ddjYtevNcqDcahBVuezJw=="),
                        Parameters = [
                            new()
                            {
                                Name = "Signature",
                                Type = MethodParameterType.Signature,
                            },
                        ],
                    },
                    Extra = new()
                    {
                        Network = ProtocolSettings.Default.Network,
                        AddressVersion = ProtocolSettings.Default.AddressVersion,
                        MillisecondsPerBlock = ProtocolSettings.Default.MillisecondsPerBlock,
                        MaxTransactionsPerBlock = ProtocolSettings.Default.MaxTransactionsPerBlock,
                        MemoryPoolMaxTransactions = ProtocolSettings.Default.MemoryPoolMaxTransactions,
                        MaxTraceableBlocks = ProtocolSettings.Default.MaxTraceableBlocks,
                        InitialGasDistribution = ProtocolSettings.Default.InitialGasDistribution,
                        ValidatorsCount = ProtocolSettings.Default.ValidatorsCount,
                        SeedList = [.. ProtocolSettings.Default.SeedList],
                        HardForks = ProtocolSettings.Default.HardForks.ToImmutableDictionary(),
                        StandbyCommittee = [.. ProtocolSettings.Default.StandbyCommittee],
                    },
                },
            ],
            Extra = null,
        };

        public static readonly Nep6WalletModel TestNep6WalletModel = new()
        {
            Name = "Unit Test Wallet",
            Version = new(1, 0),
            SCrypt = SCryptModel.Default,
            Accounts = [
                new Nep6WalletAccountModel()
                {
                    Address = "0xce45fca32b8cd071bfbc20389c20cd7025f85ff0",
                    IsDefault = true,
                    Label = "Main Test Account",
                    Lock = false,
                    Key = Encoding.UTF8.GetBytes(ChainWallet.ToNep2String(ChainWallet.GetKeyFromWifString("Ky7cYncUA92kWnh7xymshpfgz7QiX46qPWCQBQPVUSv5vndE2VTR"), "abc123")),
                    Contract = new()
                    {
                        Deployed = false,
                        Script = Convert.FromBase64String("DCECjNhSCkN5\u002BL\u002BEc0/cgGPMgQkyrl8V2ddjYtevNcqDcahBVuezJw=="),
                        Parameters = [
                            new()
                            {
                                Name = "Signature",
                                Type = MethodParameterType.Signature,
                            },
                        ],
                    },
                    Extra = new()
                    {
                        Network = ProtocolSettings.Default.Network,
                        AddressVersion = ProtocolSettings.Default.AddressVersion,
                        MillisecondsPerBlock = ProtocolSettings.Default.MillisecondsPerBlock,
                        MaxTransactionsPerBlock = ProtocolSettings.Default.MaxTransactionsPerBlock,
                        MemoryPoolMaxTransactions = ProtocolSettings.Default.MemoryPoolMaxTransactions,
                        MaxTraceableBlocks = ProtocolSettings.Default.MaxTraceableBlocks,
                        InitialGasDistribution = ProtocolSettings.Default.InitialGasDistribution,
                        ValidatorsCount = ProtocolSettings.Default.ValidatorsCount,
                        SeedList = [.. ProtocolSettings.Default.SeedList],
                        HardForks = ProtocolSettings.Default.HardForks.ToImmutableDictionary(),
                        StandbyCommittee = [.. ProtocolSettings.Default.StandbyCommittee],
                    },
                },
            ],
            Extra = null,
        };
    }
}
