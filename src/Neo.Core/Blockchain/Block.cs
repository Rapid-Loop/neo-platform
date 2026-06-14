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

using Neo.Core.Blockchain.Interface;
using Neo.Core.Cryptography;
using Neo.Core.Extensions;
using Neo.Core.Serialization;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace Neo.Core.Blockchain
{
    public class Block : BlockHeader, IEquatable<Block>, INeoSerializable, IInventory, IVerifiable
    {
        public override UInt256 Hash => this.ToArray().ToTxHash();

        /// <summary>
        /// The transaction list of the block.
        /// </summary>
        public Transaction[] Transactions { get; set; } = [];

        public InventoryType InventoryType => InventoryType.Block;

        public override int Size =>
            base.Size +
            Transactions.GetSerializedSize();

        public override int GetHashCode()
        {
            return Hash.GetHashCode();
        }

        public bool Equals([NotNullWhen(true)] Block? other)
        {
            if (ReferenceEquals(other, this)) return true;
            if (other is null) return false;
            return Hash == other.Hash;
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (ReferenceEquals(obj, this)) return true;
            if (obj is null) return false;
            return Equals(obj as Block);
        }

        public override void Deserialize(Stream reader)
        {
            base.Deserialize(reader);
            Transactions = reader.ReadObjects<Transaction>();

            var txHashes = Transactions
                .Select(static s => s.Hash)
                .ToHashSet();

            if (MerkleTree.ComputeRoot([.. txHashes]) != MerkleRoot)
                throw new FormatException("The computed Merkle root does not match the expected value.");
        }

        public override void Serialize(Stream writer)
        {
            var txHashes = Transactions
                .Select(static s => s.Hash)
                .ToHashSet();

            MerkleRoot = MerkleTree.ComputeRoot([.. txHashes]);

            base.Serialize(writer);
            writer.WriteObjects(Transactions);
        }
    }
}
