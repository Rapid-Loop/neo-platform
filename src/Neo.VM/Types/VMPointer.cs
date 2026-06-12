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

using Neo.Core.Extensions;
using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;

namespace Neo.VM.Types
{
    public class VMPointer : VMObject, IEquatable<VMPointer>
    {
        public override VMObjectType Type => VMObjectType.Pointer;

        public ReadOnlyMemory<byte> Script => _memoryOwner.Memory;

        public int Length => _memoryOwner.Memory.Length;

        public int Position => _ip;

        private readonly IMemoryOwner<byte> _memoryOwner;
        private readonly int _ip = 0;

        public VMPointer(byte[] script, int ip)
        {
            _memoryOwner = MemoryPool<byte>.Shared.Rent(script.Length);
            script.AsMemory().TryCopyTo(_memoryOwner.Memory);

            _ip = ip;
        }

        public VMPointer(Memory<byte> source, int ip)
        {
            _memoryOwner = MemoryPool<byte>.Shared.Rent(source.Length);
            source.TryCopyTo(_memoryOwner.Memory);

            _ip = ip;
        }

        public VMPointer(ReadOnlyMemory<byte> script, int ip)
        {
            _memoryOwner = MemoryPool<byte>.Shared.Rent(script.Length);
            script.TryCopyTo(_memoryOwner.Memory);

            _ip = ip;
        }

        public VMPointer(VMByteArray source, int ip)
        {
            _memoryOwner = MemoryPool<byte>.Shared.Rent(source.Length);
            source.GetReadOnlySpan().TryCopyTo(_memoryOwner.Memory.Span);

            _ip = ip;
        }

        public bool Equals(VMPointer? other)
        {
            if (ReferenceEquals(other, this)) return true;
            if (other is null) return false;
            if (RefCount != other.RefCount) return false;
            return _ip == other._ip &&
                _memoryOwner.Memory.Span.SequenceEqual(other._memoryOwner.Memory.Span);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(obj, this)) return true;
            if (obj is null) return false;
            return Equals(obj as VMPointer);
        }

        public override int GetHashCode()
        {
            return _memoryOwner.Memory.ToHashCode(RefCount ^ 397);
        }

        public override VMObject Clone()
        {
            var clone = new VMPointer(_memoryOwner.Memory.ToArray(), _ip);

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
            return _memoryOwner.Memory.Span;
        }
    }
}
