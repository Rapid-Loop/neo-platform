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

using Neo.Configuration.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;

namespace Neo.Configuration.Models
{
    public abstract class JsonModel
    {
        protected JsonSerializerOptions _jsonSerializerOptions = JsonDefaults.SerializerOptions;

        [return: NotNull]
        public virtual string? ToString(JsonSerializerOptions? options = default) =>
            ToJson(options);

        public virtual string ToJson(JsonSerializerOptions? options) =>
            JsonSerializer.Serialize<object>(this, options ?? _jsonSerializerOptions);

        public static TModel? FromJson<TModel>(string jsonString, JsonSerializerOptions? options = default)
            where TModel : notnull, JsonModel =>
            JsonSerializer.Deserialize<TModel>(
                jsonString,
                options ?? JsonDefaults.SerializerOptions);

        public static TModel? FromJson<TModel>(FileInfo file, JsonSerializerOptions? options = default)
            where TModel : notnull, JsonModel
        {
            if (file.Exists == false)
                throw new FileNotFoundException($"{file}");

            var jsonOptions = options ?? JsonDefaults.SerializerOptions;
            var jsonString = File.ReadAllText(file.FullName);

            if (string.IsNullOrEmpty(jsonString))
                throw new FormatException($"\'{file}\' is empty.");

            return FromJson<TModel>(jsonString, jsonOptions);
        }
    }
}
