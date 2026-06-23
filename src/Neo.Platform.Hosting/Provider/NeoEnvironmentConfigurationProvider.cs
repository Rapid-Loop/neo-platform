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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Neo.Platform.Hosting.Configuration;
using Neo.Platform.Hosting.Factory;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Neo.Platform.Hosting.Provider
{
    internal sealed class NeoEnvironmentConfigurationProvider : ConfigurationProvider
    {
        private const string PREFIX = "NEO";

        private readonly NeoEnvironmentConfigurationSource _source;

        public NeoEnvironmentConfigurationProvider(
            NeoEnvironmentConfigurationSource source)
        {
            _source = source;

            // ==================================
            // * NEO Platform Initial Defaults  *
            // ==================================

            // Hosting Environment
            Data.Add(HostDefaults.EnvironmentKey, NeoHostingEnvironmentNames.LocalNet);
            Data.Add(HostDefaults.ContentRootKey, Environment.CurrentDirectory);

            // Protocol Configuration
            var protocolNetwork = NeoHostingFactory.GetDevNetwork(0);

            Data.Add(ProtocolSettingNames.NetworkKey, $"{protocolNetwork:d}");
            Data.Add(ProtocolSettingNames.AddressVersionKey, "53");
            Data.Add(ProtocolSettingNames.MillisecondsPerBlockKey, "1000");
            Data.Add(ProtocolSettingNames.MaxTransactionsPerBlockKey, "512");
            Data.Add(ProtocolSettingNames.MemoryPoolMaxTransactionsKey, "50000");
            Data.Add(ProtocolSettingNames.MaxTraceableBlocksKey, "2102400");
            Data.Add(ProtocolSettingNames.InitialGasDistributionKey, "5200000000000000");

            // ==================================
            // * End of Initial Defaults        *
            // ==================================

            // Application Overrides Above Defaults

            if (_source.InitialData is not null)
            {
                foreach (var pair in _source.InitialData)
                    Data.Add(pair.Key, pair.Value);
            }

            // Environment Overrides Above Defaults
            Load(Environment.GetEnvironmentVariables(EnvironmentVariableTarget.User));
            Load(Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Process));
        }

        private void Load(IDictionary envVariables)
        {
            var iter = envVariables.GetEnumerator();

            try
            {
                while (iter.MoveNext())
                {
                    var key = (string)iter.Entry.Key;
                    var value = (string?)iter.Entry.Value;

                    AddIfNormalizedKeyMatchesPrefix(Data, Normalize(key), value);
                }
            }
            finally
            {
                (iter as IDisposable)?.Dispose();
            }
        }

        private static void AddIfNormalizedKeyMatchesPrefix(IDictionary<string, string?> data, string normalizedKey, string? value)
        {
            var normalizedPrefix = PREFIX + ':';

            if (normalizedKey.StartsWith(normalizedPrefix, StringComparison.OrdinalIgnoreCase))
                data[normalizedKey[normalizedPrefix.Length..]] = value;
        }

        private static string Normalize(string key) =>
            key.Replace("_", ConfigurationPath.KeyDelimiter);
    }
}
