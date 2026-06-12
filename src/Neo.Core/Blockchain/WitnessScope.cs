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

namespace Neo.Core.Blockchain
{
    /// <summary>
    /// Represents the scope of a <see cref="ChainWitness"/>.
    /// </summary>
    [Flags]
    public enum WitnessScope : byte
    {
        /// <summary>
        /// Indicates that no contract was witnessed. Only sign the transaction.
        /// </summary>
        None = 0,

        /// <summary>
        /// Indicates that the calling contract must be the entry contract.
        /// The witness/permission/signature given on first invocation will automatically expire if entering deeper internal invokes.
        /// This can be the default safe choice for native NEO/GAS (previously used on Neo 2 as "attach" mode).
        /// </summary>
        CalledByEntry = 0x01,

        /// <summary>
        /// Custom hash for contract-specific.
        /// </summary>
        CustomContracts = 0x10,

        /// <summary>
        /// Custom pubkey for group members.
        /// </summary>
        CustomGroups = 0x20,

        /// <summary>
        /// Indicates that the current context must satisfy the specified rules.
        /// </summary>
        WitnessRules = 0x40,

        /// <summary>
        /// This allows the witness in all contexts (default Neo2 behavior).
        /// </summary>
        /// <remarks>Note: It cannot be combined with other flags.</remarks>
        Global = 0x80
    }
}
