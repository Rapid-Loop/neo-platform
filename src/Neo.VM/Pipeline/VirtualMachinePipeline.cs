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

using Neo.VM.Middleware;

namespace Neo.VM.Pipeline
{
    /// <summary>
    /// Represents the built middleware pipeline for <see cref="VirtualMachineEngine"/>.
    /// Contains four separate, pre-built delegate chains for the different execution hooks.
    /// </summary>
    public sealed class VirtualMachinePipeline
    {
        /// <summary>
        /// Delegate chain executed before the entire VM execution starts.
        /// </summary>
        public ExecutionDelegate PreExecution { get; }

        /// <summary>
        /// Delegate chain executed after the entire VM execution finishes (HALT or FAULT).
        /// </summary>
        public ExecutionDelegate PostExecution { get; }

        /// <summary>
        /// Delegate chain executed before every individual opcode.
        /// </summary>
        public ExecuteDelegate PreExecute { get; }

        /// <summary>
        /// Delegate chain executed after every individual opcode.
        /// </summary>
        public ExecuteDelegate PostExecute { get; }

        internal VirtualMachinePipeline(
            ExecutionDelegate preExecution,
            ExecutionDelegate postExecution,
            ExecuteDelegate preExecute,
            ExecuteDelegate postExecute)
        {
            PreExecution = preExecution;
            PostExecution = postExecution;
            PreExecute = preExecute;
            PostExecute = postExecute;
        }

        /// <summary>
        /// Returns an empty pipeline (no middleware registered).
        /// </summary>
        public static VirtualMachinePipeline Empty => new
        (
            () => { },
            () => { },
            ctx => { },
            ctx => { }
        );
    }
}
