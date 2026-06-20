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

using Neo.Core;
using Neo.Core.Extensions;
using Neo.Core.VM.Type;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;

namespace Neo.VM.Types
{
    public class VMByteArray : VMObject, IEquatable<VMByteArray>
    {
        public override VMObjectType Type => VMObjectType.ByteString;

        public int Length => _byteCount;

        private readonly IMemoryOwner<byte> _memoryOwner;
        private readonly int _byteCount;

        public VMByteArray(byte[] data)
        {
            _byteCount = data.Length;
            _memoryOwner = MemoryPool<byte>.Shared.Rent(_byteCount);
            data.AsMemory().TryCopyTo(_memoryOwner.Memory);
        }

        public VMByteArray(Memory<byte> data)
        {
            _byteCount = data.Length;
            _memoryOwner = MemoryPool<byte>.Shared.Rent(_byteCount);
            data.TryCopyTo(_memoryOwner.Memory);
        }

        public VMByteArray(Span<byte> data)
        {
            _byteCount = data.Length;
            _memoryOwner = MemoryPool<byte>.Shared.Rent(_byteCount);
            data.TryCopyTo(_memoryOwner.Memory.Span);
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (ReferenceEquals(obj, this)) return true;
            if (obj is null) return false;
            return Equals(obj as VMByteArray);
        }

        public override int GetHashCode()
        {
            return _memoryOwner.Memory[.._byteCount]
                .ToHashCode(RefCount ^ 397);
        }

        protected override void Dispose(bool disposing)
        {
            _memoryOwner.Dispose();
            base.Dispose(disposing);
        }

        public override string ToString()
        {
            foreach (var v in _memoryOwner.Memory[.._byteCount].Span)
            {
                if (char.IsAscii((char)v)) continue;
                return Convert.ToBase64String(_memoryOwner.Memory[.._byteCount].Span);
            }

            return CoreUtilities.StrictUtf8Encoding.GetString(_memoryOwner.Memory[.._byteCount].Span);
        }

        public override VMObject Clone()
        {
            var clone = new VMByteArray(_memoryOwner.Memory[.._byteCount].ToArray());

            return clone;
        }

        public override bool GetBoolean()
        {
            return !_memoryOwner.Memory[.._byteCount].IsEmpty;
        }

        public override BigInteger GetInteger()
        {
            var span = _memoryOwner.Memory[.._byteCount].Span;

            if (span.Length > VMInteger.MaxSize)
                return new(span[..VMInteger.MaxSize]);

            return new(span);
        }

        protected override ReadOnlySpan<byte> ComputeSpan(HashSet<VMObject> visited)
        {
            return _memoryOwner.Memory[.._byteCount].Span;
        }

        /// <summary>
        /// Get byte at index
        /// </summary>
        public byte this[int index]
        {
            get => _memoryOwner.Memory[.._byteCount].Span[index];
            set => _memoryOwner.Memory[.._byteCount].Span[index] = value;
        }

        public string ToHexString()
        {
            return Convert.ToHexStringLower(_memoryOwner.Memory[.._byteCount].Span);
        }

        public bool Equals([NotNullWhen(true)] VMByteArray? other)
        {
            if (ReferenceEquals(other, this)) return true;
            if (other is null) return false;
            if (RefCount != other.RefCount) return false;
            if (Length != other.Length) return false;
            return _memoryOwner.Memory[.._byteCount]
                .Span
                .SequenceEqual(other._memoryOwner.Memory[..other._byteCount].Span);
        }

        /// <summary>
        /// Concatenation: VMByteArray + VMByteArray
        /// </summary>
        public static VMByteArray operator +(VMByteArray a, VMByteArray b)
        {
            return new(
                [
                    .. a._memoryOwner.Memory[..a._byteCount].Span,
                    .. b._memoryOwner.Memory[..b._byteCount].Span
                ]
            );
        }

        /// <summary>
        /// Equality comparison
        /// </summary>
        public static bool operator ==(VMByteArray a, VMByteArray b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(VMByteArray a, VMByteArray b) =>
            !(a == b);
    }
}
