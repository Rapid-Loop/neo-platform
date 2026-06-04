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
using System.Linq;
using System.Numerics;

namespace Neo.VM.Types
{
    public class VMByteArray : VMObject
    {
        public override VMObjectType Type => VMObjectType.ByteString;

        public int Length => _memory.Length;

        private Memory<byte> _memory;

        public VMByteArray(byte[] data)
        {
            _memory = data;
        }

        public VMByteArray(Memory<byte> data)
        {
            _memory = data;
        }

        public VMByteArray(Span<byte> data)
        {
            _memory = GC.AllocateUninitializedArray<byte>(data.Length, false);
            data.TryCopyTo(_memory.Span);
        }

        public override bool Equals(object? obj)
        {
            if (obj is VMByteArray other && other.Length == Length)
                return _memory.Span.SequenceEqual(other._memory.Span);

            return false;
        }

        public override int GetHashCode()
        {
            return _memory.ToArray()
                .Aggregate(0, (hash, b) => (hash * 31) ^ b);
        }

        protected override void Dispose(bool disposing)
        {
            _memory = null;
            base.Dispose(disposing);
        }

        public override string ToString()
        {
            foreach (var v in _memory.Span)
            {
                if (char.IsAsciiLetterOrDigit((char)v)) continue;
                return Convert.ToBase64String(_memory.Span);
            }

            return VMUility.StrictUtf8Encoding.GetString(_memory.Span);
        }

        public override VMObject Clone()
        {
            var clone = new VMByteArray(_memory.ToArray());

            clone.AddReference();

            return clone;
        }

        public override bool GetBoolean()
        {
            return !_memory.IsEmpty;
        }

        public override BigInteger GetInteger()
        {
            return new(_memory.Span[..VMInteger.MaxSize]);
        }

        public override ReadOnlySpan<byte> GetReadOnlySpan()
        {
            return _memory.Span;
        }

        /// <summary>
        /// Get byte at index
        /// </summary>
        public byte this[int index]
        {
            get => _memory.Span[index];
        }

        public string ToHexString()
        {
            return Convert.ToHexStringLower(_memory.Span);
        }

        /// <summary>
        /// Concatenation: VMByteArray + VMByteArray
        /// </summary>
        public static VMByteArray operator +(VMByteArray a, VMByteArray b)
        {
            return new VMByteArray([.. a._memory.Span, .. b._memory.Span]);
        }

        /// <summary>
        /// Equality comparison
        /// </summary>
        public static bool operator ==(VMByteArray a, VMByteArray b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            return a._memory.Span.SequenceEqual(b._memory.Span);
        }

        public static bool operator !=(VMByteArray a, VMByteArray b) =>
            !(a == b);
    }
}
