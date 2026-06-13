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
using Neo.Core.Extensions;
using Neo.Core.Serialization;
using System;
using System.IO;
using System.Linq;

namespace Neo.Core.Blockchain
{
    public class Transaction : IEquatable<Transaction>, IInventory, INeoSerializable, IVerifiable
    {
        public const int MaxTransactionSize = 102400;
        public const int MaxTransactionAttributes = 16;

        /// <summary>
        /// The version of the transaction.
        /// </summary>
        public byte Version { get; set; }

        public UInt256 Hash => this.ToArray().ToTxHash();

        /// <summary>
        /// The script of the transaction.
        /// </summary>
        public ReadOnlyMemory<byte> Script { get; set; }

        /// <summary>
        /// The nonce of the transaction.
        /// </summary>
        public uint Nonce { get; set; }

        /// <summary>
        /// The system fee of the transaction.
        /// </summary>
        public long SystemFee { get; set; }

        /// <summary>
        /// The network fee of the transaction.
        /// </summary>
        public long NetworkFee { get; set; }

        /// <summary>
        /// Indicates that the transaction is only valid before this block height.
        /// </summary>
        public uint ValidUnitBlock { get; set; }

        /// <summary>
        /// The <see cref="NetworkFee"/> for the transaction divided by its <see cref="Size"/>.
        /// </summary>
        public long FeePerByte => NetworkFee / Size;

        /// <summary>
        /// The attributes of the transaction.
        /// </summary>
        public TransactionAttribute[] Attributes { get; set; } = [];

        /// <summary>
        /// The signers of the transaction.
        /// </summary>
        public Signer[] Signers { get; set; } = [];

        public Witness[] Witnesses { get; set; } = [];

        /// <summary>
        /// The sender is the first signer of the transaction, regardless of its <see cref="WitnessScope"/>.
        /// </summary>
        /// <remarks>Note: The sender will pay the fees of the transaction.</remarks>
        public UInt160 Sender => Signers.FirstOrDefault()?.Account ?? UInt160.Zero;

        public InventoryType InventoryType => InventoryType.TX;

        public int Size =>
            sizeof(byte) +  //Version
            sizeof(uint) +  //Nonce
            sizeof(long) +  //SystemFee
            sizeof(long) +  //NetworkFee
            sizeof(uint) +  //ValidUntilBlock
            Signers.GetSerializedSize() +
            Attributes.GetSerializedSize() +
            Script.GetSerializedSize() +
            Witnesses.GetSerializedSize();

        public override int GetHashCode()
        {
            return Hash.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(obj, this)) return true;
            if (obj is null) return false;
            return Equals(obj as Transaction);
        }

        public bool Equals(Transaction? other)
        {
            if (ReferenceEquals(other, this)) return true;
            if (other is null) return false;
            return Hash == other.Hash;
        }

        public void Deserialize(Stream reader)
        {
            Version = reader.Read<byte>();
            Nonce = reader.Read<uint>();
            SystemFee = reader.Read<long>();
            NetworkFee = reader.Read<long>();
            ValidUnitBlock = reader.Read<uint>();
            Signers = reader.ReadObjects<Signer>();
            Attributes = reader.ReadObjects<TransactionAttribute>();
            Script = reader.ReadDynamic<byte>();
            Witnesses = reader.ReadObjects<Witness>();
        }

        public void Serialize(Stream writer)
        {
            writer.Write(Version);
            writer.Write(Nonce);
            writer.Write(SystemFee);
            writer.Write(NetworkFee);
            writer.Write(ValidUnitBlock);
            writer.WriteObjects(Signers);
            writer.WriteObjects(Attributes);
            writer.Write<byte>(Script.Span);
            writer.WriteObjects(Witnesses);
        }
    }
}
