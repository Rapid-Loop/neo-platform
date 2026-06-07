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
    public class VMMap : VMObject, IDictionary<VMObject, VMObject>, IReadOnlyDictionary<VMObject, VMObject>
    {
        public const int MaxKeySize = 64;

        public override VMObjectType Type => VMObjectType.Map;

        public int Count => _map.Count;

        public ICollection<VMObject> Keys => _map.Keys;

        public ICollection<VMObject> Values => _map.Values;

        public bool IsReadOnly => _isReadOnly;

        IEnumerable<VMObject> IReadOnlyDictionary<VMObject, VMObject>.Keys => Keys;

        IEnumerable<VMObject> IReadOnlyDictionary<VMObject, VMObject>.Values => Values;

        private readonly Dictionary<VMObject, VMObject> _map = [];
        private bool _isReadOnly = false;

        public VMObject this[VMObject key]
        {
            get
            {
                if (key.Size > MaxKeySize)
                    throw new ArgumentException($"Key size {key.Size} bytes exceeds maximum allowed size of {MaxKeySize} bytes.", nameof(key));
                return _map[key];
            }
            set => Add(key, value);
        }

        public VMMap() { }

        public VMMap(bool isReadOnly)
        {
            _isReadOnly = isReadOnly;
        }

        public VMMap(IEnumerable<KeyValuePair<VMObject, VMObject>> items, bool isReadOnly = false)
        {
            foreach (var kvp in items)
                Add(kvp);

            _isReadOnly = isReadOnly;
        }

        public VMMap(IList<VMObject> items, bool isReadOnly = false)
        {
            for (var i = 0; i < items.Count / 2; i += 2)
                Add(new(items[i], items[i + 1]));

            _isReadOnly = isReadOnly;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Clear(); // Release all keys and values
            }
            base.Dispose(disposing);
        }

        public override bool Equals(object? obj)
        {
            if (obj is VMMap other && other.Count == Count)
            {
                var children = GetChildren().ToArray();
                var otherChildren = other.GetChildren().ToArray();

                for (var i = 0; i < Count; i++)
                {
                    if (!Equals(children[i], otherChildren[i]))
                        return false;
                }

                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return _map.ToArray().Aggregate(RefCount,
                (hash, b) =>
                    (hash * 31) + (b.Key.GetHashCode() ^ b.Value.GetHashCode()));
        }

        public void Add(VMObject key, VMObject value)
        {
            if (key.Size > MaxKeySize)
                throw new ArgumentException($"Key size {key.Size} bytes exceeds maximum allowed size of {MaxKeySize} bytes.", nameof(key));

            if (_isReadOnly)
                throw new InvalidOperationException();

            if (_map.TryGetValue(key, out var oldValue))
                oldValue?.Release();

            value.AddReference();
            _map[key] = value;
        }

        public bool ContainsKey(VMObject key)
        {
            if (key.Size > MaxKeySize)
                throw new ArgumentException($"Key size {key.Size} bytes exceeds maximum allowed size of {MaxKeySize} bytes.", nameof(key));

            return key is not VMNull && _map.ContainsKey(key);
        }

        public bool Remove(VMObject key)
        {
            if (key.Size > MaxKeySize)
                throw new ArgumentException($"Key size {key.Size} bytes exceeds maximum allowed size of {MaxKeySize} bytes.", nameof(key));

            if (_isReadOnly)
                throw new InvalidOperationException();

            var result = _map.TryGetValue(key, out var oldValue);

            if (result)
                oldValue?.Release();

            _map.Remove(key);

            return result;
        }

        public bool TryGetValue(VMObject key, [MaybeNullWhen(false)] out VMObject value)
        {
            if (key.Size > MaxKeySize)
                throw new ArgumentException($"Key size {key.Size} bytes exceeds maximum allowed size of {MaxKeySize} bytes.", nameof(key));

            return _map.TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<VMObject, VMObject> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            if (_isReadOnly)
                throw new InvalidOperationException();

            foreach (var kvp in _map)
            {
                kvp.Key?.Release();
                kvp.Value?.Release();
            }
            _map.Clear();
        }

        public bool Contains(KeyValuePair<VMObject, VMObject> item)
        {
            if (item.Key.Size > MaxKeySize)
                throw new ArgumentException($"Key size {item.Key.Size} bytes exceeds maximum allowed size of {MaxKeySize} bytes.", nameof(item.Key));

            return ContainsKey(item.Key) && _map.Values.Any(a => a == item.Value);
        }

        public void CopyTo(KeyValuePair<VMObject, VMObject>[] array, int arrayIndex)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(arrayIndex, nameof(arrayIndex));
            ArgumentOutOfRangeException.ThrowIfGreaterThan(arrayIndex, array.Length - Count, nameof(arrayIndex));

            _map.ToArray().CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<VMObject, VMObject> item)
        {
            if (item.Key.Size > MaxKeySize)
                throw new ArgumentException($"Key size {item.Key.Size} bytes exceeds maximum allowed size of {MaxKeySize} bytes.", nameof(item.Key));

            return Remove(item.Key);
        }

        internal override IEnumerable<VMObject> GetChildren()
        {
            foreach (var kvp in _map)
            {
                yield return kvp.Key;
                yield return kvp.Value;
            }
        }

        public IEnumerator<KeyValuePair<VMObject, VMObject>> GetEnumerator() =>
            _map.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        internal void SetAsReadOnly() =>
            _isReadOnly = true;

        public override VMObject Clone()
        {
            var clone = new VMMap();

            // Important: Use a mapping to handle cycles during cloning
            var objectMap = new Dictionary<VMObject, VMObject>();

            foreach (var kvp in _map)
            {
                if (kvp.Key is null || kvp.Key is VMNull)
                    continue;

                VMObject clonedKey;
                VMObject clonedValue;

                // Clone key
                if (objectMap.TryGetValue(kvp.Key, out var existingKeyClone))
                    clonedKey = existingKeyClone;
                else
                {
                    clonedKey = kvp.Key.Clone();
                    objectMap[kvp.Key] = clonedKey;
                }

                // Clone value
                if (kvp.Value is null || kvp.Value is VMNull)
                    clonedValue = VMNull.Instance;
                else if (objectMap.TryGetValue(kvp.Value, out var existingValueClone))
                    clonedValue = existingValueClone;
                else
                {
                    clonedValue = kvp.Value.Clone();
                    objectMap[kvp.Value] = clonedValue;
                }

                clone._map[clonedKey] = clonedValue;
            }

            if (_isReadOnly)
                clone.SetAsReadOnly();

            clone.AddReference();

            return clone;
        }

        public override ReadOnlySpan<byte> GetReadOnlySpan()
        {
            var result = new List<byte>();

            foreach (var kvp in _map)
            {
                result.AddRange(kvp.Key.GetReadOnlySpan());
                result.AddRange(kvp.Value.GetReadOnlySpan());
            }

            return result.ToArray();
        }

        public override bool GetBoolean() =>
            _map.Count > 0;

        [DoesNotReturn]
        public override BigInteger GetInteger()
        {
            throw new InvalidOperationException();
        }
    }
}
