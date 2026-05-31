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

using Neo.VM.Interfaces;

namespace Neo.VM.Types
{
    public class VMObject(ReadOnlyMemory<byte> value) : IVMComponent, IEquatable<VMObject>
    {
        public ReadOnlyMemory<byte> ValueMemory { get; } = value;

        public VMObjectType Type { get; }

        public int Size => ValueMemory.Length;

        public int RefCount { get; private set; } = 1;

        #region IEquatable

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(obj, this)) return true;
            return Equals(obj as VMObject);
        }

        public bool Equals(VMObject? other)
        {
            if (other is null) return false;
            if (Type != other.Type) return false;
            if (Size != other.Size) return false;
            return ValueMemory.Span.SequenceEqual(other.ValueMemory.Span);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Size, ValueMemory, RefCount);
        }

        #endregion

        #region References

        public void AddRef() =>
            RefCount++;

        public void Release() =>
            RefCount--;

        #endregion

    }
}
