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
    public class VMBuffer : VMObject
    {
        public override VMObjectType Type => VMObjectType.Buffer;

        public int Length => _memory.Length;

        private Memory<byte> _memory;

        public VMBuffer(int size)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(size);
            _memory = GC.AllocateUninitializedArray<byte>(size, false);
        }

        public VMBuffer(byte[] data)
        {
            _memory = data.Clone() as byte[] ?? [];
        }

        public VMBuffer(VMByteArray source)
        {
            _memory = source?.GetReadOnlySpan().ToArray() ?? [];
        }

        public VMBuffer(Memory<byte> source)
        {
            _memory = source;
        }

        protected override void Dispose(bool disposing)
        {
            _memory = null; // Help GC
            base.Dispose(disposing);
        }

        public override int GetHashCode()
        {
            return _memory.ToArray().Aggregate(17,
                (hash, b) =>
                        (hash * 31) ^ b);
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

        public void CopyTo(VMBuffer dstBuffer, int startIndex, int dstIndex, int count)
        {
            _memory.Slice(startIndex, count).CopyTo(dstBuffer._memory[dstIndex..]);
        }

        public void Reverse()
        {
            _memory.Span.Reverse();
        }

        public override VMObject Clone()
        {
            var clone = new VMBuffer(_memory.ToArray());

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
            return _memory.ToArray();
        }

        public byte this[int index]
        {
            get => _memory.Span[index];
            set => _memory.Span[index] = value;
        }

        public static implicit operator BigInteger(VMBuffer value)
        {
            return new BigInteger(value._memory.Span[..VMInteger.MaxSize]);
        }
    }
}
