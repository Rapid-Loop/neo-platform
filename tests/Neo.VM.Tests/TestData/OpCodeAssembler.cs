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

using Neo.Core.VM;
using System;
using System.Collections.Generic;

namespace Neo.VM.Tests.TestData
{
    public static class OpCodeAssembler
    {
        private static readonly Dictionary<string, OpCode> s_opcodeMap = new(StringComparer.OrdinalIgnoreCase);

        static OpCodeAssembler()
        {
            foreach (var op in Enum.GetValues<OpCode>())
                s_opcodeMap[op.ToString()] = op;
        }

        public static byte[] Assemble(IEnumerable<string> lines)
        {
            var script = new List<byte>();

            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;

                if (trimmed.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                {
                    // Hex data (e.g., "0x04" for operand or small PUSH)
                    var hex = trimmed[2..].Replace(" ", "");
                    if (hex.Length % 2 != 0) throw new FormatException($"Invalid hex: {trimmed}");
                    script.AddRange(Convert.FromHexString(hex));
                }
                else if (s_opcodeMap.TryGetValue(trimmed, out var opcode))
                    script.Add((byte)opcode);
                else
                    throw new NotSupportedException($"Unknown opcode or token: {trimmed}");
            }

            return [.. script];
        }

        public static byte[] Assemble(params string[] lines) =>
            Assemble(lines as IEnumerable<string>);

        public static byte[] Assemble(string scriptText)
        {
            var lines = scriptText.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
            return Assemble(lines);
        }
    }
}
