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

namespace Neo.Core.Blockchain
{
    /// <summary>
    /// Represents the type of <see cref="WitnessCondition"/>.
    /// </summary>
    public enum WitnessConditionType : byte
    {
        /// <summary>
        /// Indicates that the condition will always be met or not met.
        /// </summary>
        Boolean = 0x00,

        /// <summary>
        /// Reverse another condition.
        /// </summary>
        Not = 0x01,

        /// <summary>
        /// Indicates that all conditions must be met.
        /// </summary>
        And = 0x02,

        /// <summary>
        /// Indicates that any of the conditions meets.
        /// </summary>
        Or = 0x03,

        /// <summary>
        /// Indicates that the condition is met when the current context
        /// has the specified script hash.
        /// </summary>
        ScriptHash = 0x18,

        /// <summary>
        /// Indicates that the condition is met when the current context
        /// has the specified group.
        /// </summary>
        Group = 0x19,

        /// <summary>
        /// Indicates that the condition is met when the current context
        /// is the entry point or is called by the entry point.
        /// </summary>
        CalledByEntry = 0x20,

        /// <summary>
        /// Indicates that the condition is met when the current context
        /// is called by the specified contract.
        /// </summary>
        CalledByContract = 0x28,

        /// <summary>
        /// Indicates that the condition is met when the current context
        /// is called by the specified group.
        /// </summary>
        CalledByGroup = 0x29
    }
}
