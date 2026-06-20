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
using Neo.Core.VM.Interface;
using Neo.Core.VM.Type;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Threading;

namespace Neo.VM.Types
{
    public abstract class VMObject : IVMComponent, IEquatable<VMObject>
    {
        public virtual VMObjectType Type => VMObjectType.Any;

        public int Size => AsSpan().Length;

        public int RefCount => _refCount;

        private bool _disposed = false;
        private int _refCount = 0;

        #region IEquatable

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (ReferenceEquals(obj, this)) return true;
            if (obj is null) return false;
            return Equals(obj as VMObject);
        }

        public bool Equals([NotNullWhen(true)] VMObject? other)
        {
            if (other is null) return false;
            if (Type != other.Type) return false;
            if (_disposed != other._disposed) return false;
            if (_refCount != other._refCount) return false;
            return AsSpan().SequenceEqual(other.AsSpan());
        }

        public override int GetHashCode()
        {
            return AsSpan().ToHashCode(RefCount ^ 397);
        }

        #endregion

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Cleanup managed resources
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~VMObject()
        {
            Dispose(false);
        }

        #endregion

        #region References

        internal void AddReference()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            Interlocked.Increment(ref _refCount);
        }

        internal void Release()
        {
            if (_disposed) return;

            var newCount = Interlocked.Decrement(ref _refCount);

            if (newCount < 0)
                Dispose(true);

        }

        #endregion

        public bool HasCircularReference()
        {
            var visited = new HashSet<VMObject>(ReferenceEqualityComparer.Instance);
            return DetectCycle(this, visited);
        }

        internal virtual IEnumerable<VMObject> GetChildren() =>
            [];

        /// <summary>
        /// Returns a deep clone of this object
        /// </summary>
        public abstract VMObject Clone();

        /// <summary>
        /// Internal clone helper that shares the cycle map across the entire object graph.
        /// </summary>
        protected internal virtual VMObject Clone(Dictionary<VMObject, VMObject> objectMap)
        {
            if (objectMap.TryGetValue(this, out var existing)) return existing;
            var clone = CloneCore(objectMap); // Call derived implementation
            objectMap[this] = clone;          // Register immediately
            return clone;
        }

        /// <summary>
        /// Override this in derived classes for actual cloning logic (without map).
        /// </summary>
        protected virtual VMObject CloneCore(Dictionary<VMObject, VMObject> objectMap)
        {
            return Clone();
        }

        /// <summary>
        /// Converts this object to a <see cref="ReadOnlySpan{T}"/> (for serialization/storage)
        /// </summary>
        public ReadOnlySpan<byte> AsSpan()
        {
            var visited = new HashSet<VMObject>(ReferenceEqualityComparer.Instance);
            return GetSafeSpan(visited);
        }

        /// <summary>
        /// Internal method that derived classes should override
        /// </summary>
        protected internal ReadOnlySpan<byte> GetSafeSpan(HashSet<VMObject> visited)
        {
            if (visited.Contains(this))
                return []; // Cycle detected - return empty

            visited.Add(this);

            var result = ComputeSpan(visited);

            visited.Remove(this);

            return result;
        }

        /// <summary>
        /// Override this in derived classes to provide their byte representation
        /// </summary>
        protected virtual ReadOnlySpan<byte> ComputeSpan(HashSet<VMObject> visited)
        {
            return [];
        }

        /// <summary>
        /// Converts this object to a signed integer (if possible)
        /// </summary>
        public virtual BigInteger GetInteger()
        {
            throw new InvalidOperationException($"Cannot convert {Type} to integer");
        }

        /// <summary>
        /// Converts this object to a boolean
        /// </summary>
        public virtual bool GetBoolean()
        {
            return true; // NeoVM treats most non-null objects as true
        }

        private static bool DetectCycle(VMObject current, HashSet<VMObject> visited)
        {
            if (current == null)
                return false;

            if (visited.Contains(current))
                return true; // Cycle found!

            visited.Add(current);

            // Check all child objects
            foreach (var child in current.GetChildren())
            {
                if (DetectCycle(child, visited))
                    return true;
            }

            visited.Remove(current);

            return false;
        }

        public static implicit operator VMObject(byte value) =>
            new VMInteger(value);

        public static implicit operator VMObject(sbyte value) =>
            new VMInteger(value);

        public static implicit operator VMObject(short value) =>
            new VMInteger(value);

        public static implicit operator VMObject(ushort value) =>
            new VMInteger(value);

        public static implicit operator VMObject(int value) =>
            new VMInteger(value);

        public static implicit operator VMObject(uint value) =>
            new VMInteger(value);

        public static implicit operator VMObject(long value) =>
            new VMInteger(value);

        public static implicit operator VMObject(ulong value) =>
            new VMInteger(value);

        public static implicit operator VMObject(Int128 value) =>
            new VMInteger(value);

        public static implicit operator VMObject(UInt128 value) =>
            new VMInteger(value);

        public static implicit operator VMObject(BigInteger value) =>
            new VMInteger(value);

        public static implicit operator VMObject(bool value) =>
            new VMBoolean(value);

        public static implicit operator VMObject(byte[] value) =>
            new VMByteArray(value);

        public static implicit operator VMObject(string value) =>
            new VMByteArray(CoreUtilities.StrictUtf8Encoding.GetBytes(value));

        public static implicit operator byte(VMObject value) =>
            (byte)value.GetInteger();

        public static implicit operator sbyte(VMObject value) =>
            (sbyte)value.GetInteger();

        public static implicit operator short(VMObject value) =>
            new VMInteger(value);

        public static implicit operator ushort(VMObject value) =>
            (ushort)value.GetInteger();

        public static implicit operator int(VMObject value) =>
            (int)value.GetInteger();

        public static implicit operator uint(VMObject value) =>
            (uint)value.GetInteger();

        public static implicit operator long(VMObject value) =>
            (long)value.GetInteger();

        public static implicit operator ulong(VMObject value) =>
            (ulong)value.GetInteger();

        public static implicit operator Int128(VMObject value) =>
            (Int128)value.GetInteger();

        public static implicit operator UInt128(VMObject value) =>
            (UInt128)value.GetInteger();

        public static implicit operator BigInteger(VMObject value) =>
            value.GetInteger();

        public static implicit operator bool(VMObject value) =>
            value.GetBoolean();

        public static implicit operator byte[](VMObject value) =>
            [.. value.AsSpan()];

        public static implicit operator string(VMObject value) =>
            $"{value}";
    }
}
