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

using Neo.Core.VM.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Neo.Core.VM.Specs
{
    public static class GasTable
    {
        private readonly static Dictionary<HardFork, Dictionary<OpCode, long>> s_gasCosts = [];

        static GasTable()
        {
            // Initialize all HardFork levels with empty dictionaries
            foreach (var fork in Enum.GetValues<HardFork>())
                s_gasCosts[fork] = [];

            // Populate gas costs from attributes
            foreach (var field in typeof(OpCode).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (field.IsLiteral == false) continue; // Skip non-enum values

                var opcode = (OpCode)field.GetValue(null)!;

                var attributes = field.GetCustomAttributes<OpCodePriceAttribute>(inherit: false);

                foreach (var attr in attributes)
                {
                    if (attr == null) continue;

                    if (s_gasCosts.TryGetValue(attr.HardFork, out var table) == false)
                        // Fallback in case of unexpected HardFork value
                        s_gasCosts[attr.HardFork] = new() { { opcode, attr.Cost } };
                    else
                        table[opcode] = attr.Cost;
                }
            }
        }

        /// <summary>
        /// Get gas cost for an <see cref="OpCode"/> under a specific <see cref="HardFork"/>.
        /// </summary>
        public static long GetGasCost(OpCode opcode, HardFork fork = HardFork.Genesis)
        {
            for (var current = fork; current >= HardFork.Genesis && Enum.IsDefined(current); current--)
            {
                if (s_gasCosts.TryGetValue(current, out var table) &&
                    table.TryGetValue(opcode, out var cost))
                {
                    return cost;
                }
            }

            return s_gasCosts[HardFork.Genesis][opcode]; // fallback Genesis MUST exist
        }

        public static long GetGasCost(MethodDescriptor method, HardFork fork = HardFork.Genesis)
        {
            var attributes = method.TargetMethodInfo.GetCustomAttributes<MethodDescriptorAttribute>();
            var attr = default(MethodDescriptorAttribute);

            for (var current = fork; current >= HardFork.Genesis && Enum.IsDefined(current); current--)
            {
                attr = attributes.FirstOrDefault(s => s.Fork == current);

                if (attr is null) continue; else break;
            }

            return attr?.ExecutePrice ?? GetGasCost(OpCode.CALL, fork);
        }

        /// <summary>
        /// Get all gas costs for a specific <see cref="HardFork"/> (useful for debugging).
        /// </summary>
        public static IReadOnlyDictionary<OpCode, long> GetAllCosts(HardFork fork = HardFork.Genesis)
        {
            for (var current = fork; current >= HardFork.Genesis && Enum.IsDefined(current); current--)
            {
                if (s_gasCosts.TryGetValue(current, out var table))
                {
                    if (table.Count == 0) continue;

                    return table;
                }
            }

            return s_gasCosts[HardFork.Genesis]; // fallback Genesis MUST exist
        }
    }
}
