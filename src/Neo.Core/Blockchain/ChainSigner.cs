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

using Neo.Core.Cryptography.ECC;
using Neo.Core.Extensions;
using Neo.Core.Serialization;
using System;
using System.Collections.Generic;
using System.IO;

namespace Neo.Core.Blockchain
{
    public class ChainSigner : INeoSerializable, IEquatable<ChainSigner>
    {
        /// <summary>
        /// The account of the signer.
        /// </summary>
        public UInt160 Account { get; set; } = UInt160.Zero;

        /// <summary>
        /// The scopes of the witness.
        /// </summary>
        public WitnessScope Scopes { get; set; }

        /// <summary>
        /// The contracts that allowed by the witness.
        /// Only available when the <see cref="WitnessScope.CustomContracts"/> flag is set.
        /// </summary>
        public UInt160[]? AllowedContracts { get; set; }

        /// <summary>
        /// The groups that allowed by the witness.
        /// Only available when the <see cref="WitnessScope.CustomGroups"/> flag is set.
        /// </summary>
        public ECPoint[]? AllowedGroups { get; set; }

        ///// <summary>
        ///// The rules that the witness must meet.
        ///// Only available when the <see cref="WitnessScope.WitnessRules"/> flag is set.
        ///// </summary>
        //public WitnessRule[]? Rules { get; set; }

        public int Size => throw new NotImplementedException();

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 397;   // ← Prime seed

                hash *= 31 ^ Account.GetHashCode();
                hash *= 31 ^ Scopes.GetHashCode();
                hash *= 31 ^ ((AllowedContracts as IReadOnlyList<UInt160>)?.ToHashCode() ?? 0);
                hash *= 31 ^ ((AllowedGroups as IReadOnlyList<ECPoint>)?.ToHashCode() ?? 0);

                return hash;
            }
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(obj, this)) return true;
            if (obj is null) return false;
            return Equals(obj as ChainSigner);
        }
        public bool Equals(ChainSigner? other)
        {
            if (ReferenceEquals(other, this)) return true;
            if (other is null) return false;
            if (Account != other.Account) return false;
            if (Scopes != other.Scopes) return false;

            if (Scopes.HasFlag(WitnessScope.CustomContracts) &&
                AllowedContracts?.SequenceEqual(other.AllowedContracts) == false)
                return false;

            if (Scopes.HasFlag(WitnessScope.CustomGroups) &&
                AllowedGroups?.SequenceEqual(other.AllowedGroups) == false)
                return false;

            //if (Scopes.HasFlag(WitnessScope.WitnessRules) &&
            //    Rules.SequenceEqual(other.Rules) == false)
            //    return false;

            return true;
        }

        public void Deserialize(Stream reader)
        {
            throw new NotImplementedException();
        }

        public void Serialize(Stream writer)
        {
            throw new NotImplementedException();
        }
    }
}
