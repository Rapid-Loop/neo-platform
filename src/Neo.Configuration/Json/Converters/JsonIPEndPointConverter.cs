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

using System;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Neo.Configuration.Json.Converters
{
    public class JsonIPEndPointConverter : JsonConverter<IPEndPoint?>
    {
        public override IPEndPoint? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
                throw new FormatException();

            var valueString = reader.GetString();

            if (string.IsNullOrEmpty(valueString))
                return default;

            if (IPEndPoint.TryParse(valueString, out var value))
                return value;
            else
            {
                // Try to see if its a hostname
                var spString = valueString.Split(':', StringSplitOptions.TrimEntries);
                if (spString.Length == 2)
                {
                    try
                    {
                        var host = Dns.GetHostEntry(spString[0]);
                        if (host.AddressList.Length > 0)
                        {
                            var length = host.AddressList.Length - 1;
                            var ipAddress = host.AddressList
                            [
                                length > 0 ? Random.Shared.Next(length) : 0
                            ];

                            return new IPEndPoint(ipAddress, int.Parse(spString[1]));
                        }
                    }
                    catch (SocketException) // Host doesn't exist or network is down
                    {
                    }
                }
            }

            return default;
        }

        public override void Write(Utf8JsonWriter writer, IPEndPoint? value, JsonSerializerOptions options)
        {
            if (value is null)
                writer.WriteNullValue();
            else
                writer.WriteStringValue($"{value}");
        }
    }
}
