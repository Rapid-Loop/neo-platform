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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;

namespace Neo.VM.Types
{
    public class VMArray : VMObject, IEnumerable<VMObject>, IEnumerable, ICollection<VMObject>, IReadOnlyCollection<VMObject>, IList<VMObject>, IReadOnlyList<VMObject>
    {
        public override VMObjectType Type => VMObjectType.Array;

        public int Count => _array.Count;

        public bool IsReadOnly => _isReadOnly;

        private bool _isReadOnly = false;

        protected readonly List<VMObject> _array = [];

        public VMArray() { }

        public VMArray(bool isReadonly) : this([], isReadonly) { }

        public VMArray(IEnumerable<VMObject> items) : this(items, false) { }

        public VMArray(IEnumerable<VMObject> items, bool isReadonly)
        {
            foreach (var item in items)
            {
                item.AddReference();
                _array.Add(item);
            }

            _isReadOnly = isReadonly;
        }

        public VMObject this[int index]
        {
            get => _array[index];
            set
            {
                if (_isReadOnly) throw new InvalidOperationException();

                _array[index]?.Release();
                value.AddReference();
                _array[index] = value;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Clear(); // Release all items
            }
            base.Dispose(disposing);
        }

        public override int GetHashCode()
        {
            return _array.Aggregate(17,
                (hash, b) =>
                    (hash * 31) ^ b.GetHashCode());
        }

        public override bool Equals(object? obj)
        {
            if (obj is VMArray other && other.Count == Count)
            {
                for (var i = 0; i < Count; i++)
                {
                    if (!Equals(this[i], other[i]))
                        return false;
                }

                return true;
            }

            return false;
        }

        #region IEnumerable

        public IEnumerator<VMObject> GetEnumerator() =>
            _array.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        #endregion

        #region ICollection

        public void Add(VMObject item)
        {
            if (_isReadOnly) throw new InvalidOperationException();

            item.AddReference();
            _array.Add(item);
        }

        public void Clear()
        {
            if (_isReadOnly) throw new InvalidOperationException();

            _array.ForEach(i => i.Release());
            _array.Clear();
        }

        public bool Contains(VMObject item)
        {
            return _array.Contains(item);
        }

        public void CopyTo(VMObject[] array, int arrayIndex)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(arrayIndex, nameof(arrayIndex));
            ArgumentOutOfRangeException.ThrowIfGreaterThan(arrayIndex, array.Length - Count, nameof(arrayIndex));

            _array.CopyTo(array, arrayIndex);
        }

        public bool Remove(VMObject item)
        {
            if (_isReadOnly) throw new InvalidOperationException();

            _array[_array.IndexOf(item)]?.Release();
            return _array.Remove(item);
        }

        #endregion

        #region IList

        public int IndexOf(VMObject item)
        {
            return _array.IndexOf(item);
        }

        public void Insert(int index, VMObject item)
        {
            if (_isReadOnly) throw new InvalidOperationException();

            item.AddReference();
            _array.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            if (_isReadOnly) throw new InvalidOperationException();

            _array[index]?.Release();
            _array.RemoveAt(index);
        }

        #endregion

        internal override IEnumerable<VMObject> GetChildren() =>
            [.. _array];

        public void Reverse()
        {
            if (_isReadOnly) throw new InvalidOperationException();

            _array.Reverse();
        }

        internal void SetAsReadOnly() =>
            _isReadOnly = true;

        public override VMObject Clone()
        {
            var clone = new VMArray();

            // Important: Use a mapping to handle cycles during cloning
            var objectMap = new Dictionary<VMObject, VMObject>();

            _array.ForEach(i =>
            {
                if (i is null || i is VMNull)
                {
                    clone._array.Add(VMNull.Instance);
                    return;
                }

                if (objectMap.TryGetValue(i, out var alreadyCloned))
                {
                    // Cycle detected during cloning - reuse the cloned object
                    alreadyCloned.AddReference();
                    clone._array.Add(alreadyCloned);
                }
                else
                {
                    var clonedItem = i.Clone();
                    objectMap[i] = clonedItem;
                    clone._array.Add(clonedItem);
                }
            });

            if (_isReadOnly)
                clone.SetAsReadOnly();

            clone.AddReference();
            return clone;
        }

        public override bool GetBoolean()
        {
            return _array.Count > 0;
        }

        [DoesNotReturn]
        public override BigInteger GetInteger()
        {
            throw new InvalidOperationException();
        }

        public override ReadOnlySpan<byte> GetReadOnlySpan()
        {
            var result = new List<byte>();

            _array.ForEach(i => result.AddRange(i.GetReadOnlySpan()));

            return result.ToArray();
        }
    }
}
