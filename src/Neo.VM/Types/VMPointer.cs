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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;

namespace Neo.VM.Types
{
    public class VMPointer : VMObject
    {
        public override VMObjectType Type => VMObjectType.Pointer;

        public ReadOnlyMemory<byte> Script => _memory;

        public int Length => _memory.Length;

        public int Position => _ip;

        private readonly ReadOnlyMemory<byte> _memory;
        private readonly int _ip = 0;

        public VMPointer(byte[] script, int ip)
        {
            _memory = script.Clone() as byte[] ?? [];
            _ip = ip;
        }

        public VMPointer(ReadOnlyMemory<byte> script, int ip)
        {
            _memory = script;
            _ip = ip;
        }

        public override bool Equals(VMObject? other)
        {
            if (other is VMPointer p)
                return _ip == p._ip && base.Equals(other);

            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return _memory.ToArray().Aggregate(_ip,
                (hash, b) =>
                    (hash * 31) ^ b);
        }

        public override VMObject Clone()
        {
            var clone = new VMPointer(_memory.ToArray(), _ip);

            clone.AddReference();

            return clone;
        }

        public override bool GetBoolean()
        {
            return true;
        }

        [DoesNotReturn]
        public override BigInteger GetInteger()
        {
            throw new InvalidOperationException($"Cannot convert {Type} to integer");
        }

        public override ReadOnlySpan<byte> GetReadOnlySpan()
        {
            return _memory.ToArray();
        }
    }
}
