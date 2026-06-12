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

namespace Neo.Core.Net
{
    /// <summary>
    /// Represents the command of a message.
    /// </summary>
    public enum MessageCommandType : byte
    {
        #region handshaking

        /// <summary>
        /// Sent when a connection is established.
        /// </summary>
        Version = 0x00,

        /// <summary>
        /// Sent to respond to <see cref="Version"/> messages.
        /// </summary>
        VersionAck = 0x01,

        #endregion

        #region connectivity

        /// <summary>
        /// Sent to request for remote nodes.
        /// </summary>
        GetAddress = 0x10,

        /// <summary>
        /// Sent to respond to <see cref="GetAddress"/> messages.
        /// </summary>
        Address = 0x11,

        /// <summary>
        /// Sent to detect whether the connection has been disconnected.
        /// </summary>
        Ping = 0x18,

        /// <summary>
        /// Sent to respond to <see cref="Ping"/> messages.
        /// </summary>
        Pong = 0x19,

        #endregion

        #region synchronization

        /// <summary>
        /// Sent to request for headers.
        /// </summary>
        GetHeaders = 0x20,

        /// <summary>
        /// Sent to respond to <see cref="GetHeaders"/> messages.
        /// </summary>
        Headers = 0x21,

        /// <summary>
        /// Sent to request for blocks.
        /// </summary>
        GetBlocks = 0x24,

        /// <summary>
        /// Sent to request for memory pool.
        /// </summary>
        MemoryPool = 0x25,

        /// <summary>
        /// Sent to relay inventories.
        /// </summary>
        Inventory = 0x27,

        /// <summary>
        /// Sent to request for inventories.
        /// </summary>
        GetData = 0x28,

        /// <summary>
        /// Sent to request for blocks.
        /// </summary>
        GetBlockByIndex = 0x29,

        /// <summary>
        /// Sent to respond to <see cref="GetData"/> messages when the inventories are not found.
        /// </summary>
        NotFound = 0x2a,

        /// <summary>
        /// Sent to send a transaction.
        /// </summary>
        Transaction = 0x2b,

        /// <summary>
        /// Sent to send a block.
        /// </summary>
        Block = 0x2c,

        /// <summary>
        /// Sent to send an extensible payload.
        /// </summary>
        Extensible = 0x2e,

        /// <summary>
        /// Sent to reject an inventory.
        /// </summary>
        Reject = 0x2f,

        #endregion

        #region SPV protocol

        /// <summary>
        /// Sent to load the bloom filter.
        /// </summary>
        FilterLoad = 0x30,

        /// <summary>
        /// Sent to update the items for the bloom filter.
        /// </summary>
        FilterAdd = 0x31,

        /// <summary>
        /// Sent to clear the bloom filter.
        /// </summary>
        FilterClear = 0x32,

        /// <summary>
        /// Sent to send a filtered block.
        /// </summary>
        MerkleBlock = 0x38,

        #endregion

        #region others

        /// <summary>
        /// Sent to send an alert.
        /// </summary>
        Alert = 0x40,

        #endregion
    }
}
