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

using Neo.IO.Hashing;
using System;
using System.Collections;

namespace Neo.IO.Data
{
    public class BloomFilter
    {
        private readonly BitArray _bits;
        private readonly int _bitSize;
        private readonly int _hashCount;
        private readonly uint _tweak;

        public int HashCount => _hashCount;

        public int BitSize => _bitSize;

        public uint Tweak => _tweak;

        /// <summary>
        /// Creates a Bloom Filter with optimal size and hash count
        /// </summary>
        /// <param name="expectedElements">Expected number of elements to insert</param>
        /// <param name="falsePositiveRate">Desired false positive probability (default 0.01 = 1%)</param>
        /// <param name="tweak">Random tweak for hash seeds (optional)</param>
        /// <param name="elementBytes">The initial elements contained in <see cref="BloomFilter"/></param>
        public BloomFilter(int expectedElements, double falsePositiveRate = 0.01, uint tweak = 0, byte[]? elementBytes = default)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(expectedElements, nameof(expectedElements));

            _bitSize = (int)Math.Ceiling(-expectedElements * Math.Log(falsePositiveRate)
                        / (Math.Log(2) * Math.Log(2)));

            _hashCount = Math.Max(1, (int)Math.Ceiling((_bitSize / (double)expectedElements) * Math.Log(2)));

            _bits = elementBytes is null ?
                new(_bitSize) :
                new(elementBytes) { Length = _bitSize };

            _tweak = tweak;
        }

        public void Add(ReadOnlySpan<byte> data)
        {
            for (var i = 0; i < _hashCount; i++)
            {
                var hash = Murmur32.HashToUInt32(data, (uint)i * 0xfba4c795u + _tweak);
                var index = (int)(hash % (uint)_bitSize);

                _bits[index] = true;
            }
        }

        public bool Contains(ReadOnlySpan<byte> data)
        {
            for (var i = 0; i < _hashCount; i++)
            {
                var hash = Murmur32.HashToUInt32(data, (uint)i * 0xfba4c795u + _tweak);
                var index = (int)(hash % (uint)_bitSize);

                if (_bits[index] == false)
                    return false; // Definitely not present
            }

            return true; // Probably present
        }

        public void Add(byte[] data) =>
            Add(data.AsSpan());

        public bool Contains(byte[] data) =>
            Contains(data.AsSpan());

        public double GetFalsePositiveProbability(int insertedCount)
        {
            return Math.Pow(1 - Math.Exp(-_hashCount * insertedCount / (double)_bitSize), _hashCount);
        }

        public void GetBits(byte[] bytes) =>
            _bits.CopyTo(bytes, 0);
    }
}
