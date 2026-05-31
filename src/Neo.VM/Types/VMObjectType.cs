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

namespace Neo.VM.Types
{
    /// <summary>
    /// An enumeration representing the types in the VM.
    /// </summary>
    public enum VMObjectType : byte
    {
        /// <summary>
        /// Represents any type.
        /// </summary>
        Any = 0x00,

        /// <summary>
        /// Represents a code pointer.
        /// </summary>
        Pointer = 0x10,

        /// <summary>
        /// Represents the boolean (<see langword="true" /> or <see langword="false" />) type.
        /// </summary>
        Boolean = 0x20,

        /// <summary>
        /// Represents an integer.
        /// </summary>
        Integer = 0x21,

        /// <summary>
        /// Represents an immutable memory block.
        /// </summary>
        ByteString = 0x28,

        /// <summary>
        /// Represents a memory block that can be used for reading and writing.
        /// </summary>
        Buffer = 0x30,

        /// <summary>
        /// Represents an array or a complex object.
        /// </summary>
        Array = 0x40,

        /// <summary>
        /// Represents a structure.
        /// </summary>
        Struct = 0x41,

        /// <summary>
        /// Represents an ordered collection of key-value pairs.
        /// </summary>
        Map = 0x48,

        /// <summary>
        /// Represents an interface used to interoperate with the outside of the the VM.
        /// </summary>
        InteropInterface = 0x60,
    }
}
