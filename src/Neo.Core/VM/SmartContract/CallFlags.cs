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

namespace Neo.Core.VM.SmartContract
{
    /// <summary>
    /// Represents the operations allowed when a contract is called.
    /// </summary>
    [Flags]
    public enum CallFlags : byte
    {
        /// <summary>
        /// No flag is set.
        /// </summary>
        None = 0,

        /// <summary>
        /// Indicates that the called contract is allowed to read states.
        /// </summary>
        ReadStates = 0b_00000001,

        /// <summary>
        /// Indicates that the called contract is allowed to write states.
        /// </summary>
        WriteStates = 0b_00000010,

        /// <summary>
        /// Indicates that the called contract is allowed to call another contract.
        /// </summary>
        AllowCall = 0b_00000100,

        /// <summary>
        /// Indicates that the called contract is allowed to send notifications.
        /// </summary>
        AllowNotify = 0b_00001000,

        /// <summary>
        /// Indicates that the called contract is allowed to read or write states.
        /// </summary>
        States = ReadStates | WriteStates,

        /// <summary>
        /// Indicates that the called contract is allowed to read states or call another contract.
        /// </summary>
        ReadOnly = ReadStates | AllowCall,

        /// <summary>
        /// All flags are set.
        /// </summary>
        All = States | AllowCall | AllowNotify
    }
}
