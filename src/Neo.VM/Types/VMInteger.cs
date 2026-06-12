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
using System.Numerics;

namespace Neo.VM.Types
{
    public class VMInteger : VMObject, IEquatable<VMInteger>
    {
        public const int MaxSize = 32;

        public override VMObjectType Type => VMObjectType.Integer;

        private readonly BigInteger _value = BigInteger.Zero;

        public VMInteger(BigInteger value)
        {
            if (value.GetByteCount() > MaxSize)
                throw new ArgumentException($"Integer size bytes exceeds maximum allowed size of {MaxSize} bytes.", nameof(value));
            _value = value;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(obj, this)) return true;
            if (obj is null) return false;
            return Equals(obj as VMInteger);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override VMObject Clone()
        {
            var clone = new VMInteger(_value);

            clone.AddReference(); // Since we're cloning, we add a reference

            return clone;
        }

        public override bool GetBoolean()
        {
            return _value != BigInteger.Zero;
        }

        public override BigInteger GetInteger()
        {
            return _value;
        }

        public override ReadOnlySpan<byte> GetReadOnlySpan()
        {
            return _value.ToByteArray();
        }

        public bool Equals(VMInteger? other)
        {
            if (ReferenceEquals(other, this)) return true;
            if (other is null) return false;
            if (RefCount != other.RefCount) return false;
            return _value == other._value;
        }

        public static VMInteger operator +(VMInteger a, VMInteger b)
        {
            a.AddReference();
            b.AddReference();

            var result = new VMInteger(a._value + b._value);

            a.Release();
            b.Release();

            return result;
        }

        public static VMInteger operator -(VMInteger a, VMInteger b)
        {
            a.AddReference();
            b.AddReference();

            var result = new VMInteger(a._value - b._value);

            a.Release();
            b.Release();

            return result;
        }

        public static VMInteger operator *(VMInteger a, VMInteger b)
        {
            a.AddReference();
            b.AddReference();

            var result = new VMInteger(a._value * b._value);

            a.Release();
            b.Release();

            return result;
        }

        public static VMInteger operator /(VMInteger a, VMInteger b)
        {
            if (b._value == BigInteger.Zero)
                throw new DivideByZeroException("Division by zero in VMInteger");

            a.AddReference();
            b.AddReference();

            var result = new VMInteger(a._value / b._value);

            a.Release();
            b.Release();

            return result;
        }

        public static VMInteger operator %(VMInteger a, VMInteger b)
        {
            a.AddReference();
            b.AddReference();

            var result = new VMInteger(a._value % b._value);

            a.Release();
            b.Release();

            return result;
        }

        public static VMInteger operator -(VMInteger a) =>
            new(-a._value);

        // Comparison operators
        public static bool operator ==(VMInteger a, VMInteger b)
        {
            if (ReferenceEquals(a, b)) return true;
            return a.Equals(b);
        }

        public static bool operator !=(VMInteger a, VMInteger b) =>
            !(a == b);

        public static bool operator >(VMInteger a, VMInteger b) =>
            a._value > b._value;

        public static bool operator <(VMInteger a, VMInteger b) =>
            a._value < b._value;

        public static bool operator >=(VMInteger a, VMInteger b) =>
            a._value >= b._value;

        public static bool operator <=(VMInteger a, VMInteger b) =>
            a._value <= b._value;

        public static implicit operator BigInteger(VMInteger value)
        {
            return value.GetInteger();
        }
    }
}
