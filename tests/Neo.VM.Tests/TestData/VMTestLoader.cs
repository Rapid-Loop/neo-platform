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

using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Neo.VM.Tests.TestData
{

    internal static class VMTestLoader
    {
        private static readonly JsonSerializerOptions s_options = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            ReadCommentHandling = JsonCommentHandling.Disallow,
            UnknownTypeHandling = JsonUnknownTypeHandling.JsonNode,
            PreferredObjectCreationHandling = JsonObjectCreationHandling.Replace,
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip,
            WriteIndented = true,
            RespectNullableAnnotations = false,
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
            Converters =
            {
                new JsonStringEnumConverter(),
            }
        };

        public static List<VMTestSuite> LoadAllTests()
        {
            var assembly = typeof(VMTestLoader).Assembly;
            using var stream = assembly.GetManifestResourceStream("Neo.VM.Tests.TestData.VMTests.zip");
            using var archive = new ZipArchive(stream!, ZipArchiveMode.Read);

            var suites = new List<VMTestSuite>();

            foreach (var entry in archive.Entries.Where(e => e.FullName.EndsWith(".json")))
            {
                using var jsonStream = entry.Open();
                var suite = JsonSerializer.Deserialize<VMTestSuite>(jsonStream, s_options);
                if (suite != null) suites.Add(suite);
            }

            return suites;
        }
    }
}
