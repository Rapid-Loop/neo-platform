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
using System.Diagnostics.CodeAnalysis;

namespace Neo.VM.Core
{
    public delegate void VTableFunc(VirtualMachineEngine engine, VMInstruction instruction);

    public partial class VirtualTable
    {
        public static readonly VirtualTable Default = new();

        public VTableFunc[] Functions { get; protected set; } = new VTableFunc[byte.MaxValue];

        public VTableFunc this[OpCode opCode]
        {
            get => Functions[(byte)opCode];
            protected set => Functions[(byte)opCode] = value;
        }

        public VirtualTable()
        {
            Array.Fill(Functions, InvalidOpcode);

            foreach (var mi in GetType().GetMethods())
            {
                if (Enum.TryParse<OpCode>(mi.Name, true, out var opCode))
                    Functions[(byte)opCode] = mi.CreateDelegate<VTableFunc>(this);
            }
        }


        [DoesNotReturn]
        public static void InvalidOpcode(VirtualMachineEngine engine, VMInstruction instruction)
        {
            throw new InvalidOperationException($"Opcode {instruction.OpCode} is undefined.");
        }
    }
}
