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

using Neo.Core.Extensions;
using Neo.Core.Serialization;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Neo.Core.Blockchain
{
    public class WitnessRule : IEquatable<WitnessRule>, INeoSerializable
    {
        /// <summary>
        /// Indicates the action to be taken if the current context meets with the rule.
        /// </summary>
        public WitnessRuleAction Action { get; set; }

        /// <summary>
        /// The condition of the rule.
        /// </summary>
        public WitnessCondition Condition { get; set; } = default!;

        public int Size =>
            Unsafe.SizeOf<WitnessRuleAction>() +
            Condition.Size;

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 397;   // ← Prime seed

                hash *= 31 ^ Action.GetHashCode();
                hash *= 31 ^ Condition.GetHashCode();

                return hash;
            }
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(obj, this)) return true;
            if (obj is null) return false;
            return Equals(obj as WitnessRule);
        }

        public bool Equals(WitnessRule? other)
        {
            if (ReferenceEquals(other, this)) return true;
            if (other is null) return false;
            return Action == other.Action && Condition == other.Condition;
        }

        public void Deserialize(Stream reader)
        {
            Action = reader.Read<WitnessRuleAction>();
            Condition.Deserialize(reader);
        }

        public void Serialize(Stream writer)
        {
            writer.Write(Action);
            Condition.Serialize(writer);
        }
    }
}
