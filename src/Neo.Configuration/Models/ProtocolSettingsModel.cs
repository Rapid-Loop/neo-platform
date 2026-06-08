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

using Neo.Core.Interfaces;
using Neo.Cryptography.ECC;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Neo.Configuration.Models
{
    public class ProtocolSettingsModel : JsonModel, IMap<ProtocolSettings>
    {
        public uint Network { get; set; }

        public byte AddressVersion { get; set; }

        public uint MillisecondsPerBlock { get; set; }

        public uint MaxTransactionsPerBlock { get; set; }

        public int MemoryPoolMaxTransactions { get; set; }

        public uint MaxTraceableBlocks { get; set; }

        public ulong InitialGasDistribution { get; set; }

        public int ValidatorsCount { get; set; }

        public string[]? SeedList { get; set; }

        public IDictionary<Hardfork, uint>? Hardforks { get; set; }

        public ECPoint[]? StandbyCommittee { get; set; }

        public ProtocolSettings ToObject() =>
            new()
            {
                Network = Network,
                AddressVersion = AddressVersion,
                MillisecondsPerBlock = MillisecondsPerBlock,
                MaxTransactionsPerBlock = MaxTransactionsPerBlock,
                MemoryPoolMaxTransactions = MemoryPoolMaxTransactions,
                MaxTraceableBlocks = MaxTraceableBlocks,
                InitialGasDistribution = InitialGasDistribution,
                ValidatorsCount = ValidatorsCount,
                SeedList = SeedList ?? [],
                Hardforks = Hardforks?.ToImmutableDictionary() ?? [],
                StandbyCommittee = StandbyCommittee?.ToList() ?? [],
            };
    }
}
