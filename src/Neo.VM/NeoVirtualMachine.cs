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

using Neo.VM.Core;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.VM
{
    /// <summary>
    /// Represents the VM used to execute the script.
    /// </summary>
    public class NeoVirtualMachine : IDisposable
    {
        public VMState State { get; internal set; }

        public BigInteger TotalGasConsumed { get; private set; }

        /// <summary>
        /// Restrictions on the VM.
        /// </summary>
        public ExecutionEngineLimits Limits { get; } = ExecutionEngineLimits.Default;

        /// <summary>
        /// The invocation stack of the VM.
        /// </summary>
        public Stack<ExecutionContext> InvocationStack => _invocationStack;

        /// <summary>
        /// The top frame of the invocation stack.
        /// </summary>
        public ExecutionContext? CurrentContext => _currentContext;

        /// <summary>
        /// The bottom frame of the invocation stack.
        /// </summary>
        public ExecutionContext? EntryContext => _entryContext;

        /// <summary>
        /// The stack to store the return values.
        /// </summary>
        public Stack<VMObject> ResultStack { get; } = [];

        public bool IsRunning { get; internal set; } = false;

        private readonly VirtualTable _defaultOpCodeTable = VirtualTable.Default;
        private readonly Stack<ExecutionContext> _invocationStack = [];
        private ExecutionContext? _currentContext;
        private ExecutionContext? _entryContext;

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public ExecutionContext LoadScript(byte[] script, int initialPosition = 0)
        {
            var rootContext = new ExecutionContext(script); // TODO: Pass parameters for Transaction Class

            LoadContext(rootContext);
            return rootContext;
        }

        /// <summary>
        /// Loads the specified context into the invocation stack.
        /// </summary>
        /// <param name="context">The context to load.</param>
        internal virtual void LoadContext(ExecutionContext context)
        {
            if (_invocationStack.Count >= Limits.MaxInvocationStackSize)
                throw new InvalidOperationException($"MaxInvocationStackSize exceed: {InvocationStack.Count}");

            _entryContext ??= context;
            _currentContext = context;

            InvocationStack.Push(context);
        }

        /// <summary>
        /// Called when a context is unloaded.
        /// </summary>
        /// <param name="context">The context being unloaded.</param>
        internal virtual void ContextUnloaded(ExecutionContext context)
        {
            if (InvocationStack.Count > 0)
                _currentContext = InvocationStack.Peek();
            else
            {
                _currentContext = null;
                _entryContext = null;
            }

            context.Cleanup();
        }

        public VMState Execute()
        {
            TotalGasConsumed = 0;

            if (State == VMState.BREAK)
                State = VMState.NONE;

            try
            {
                while ((State != VMState.HALT && State != VMState.FAULT)
                    && _invocationStack.Count > 0)
                {
                    _currentContext = _invocationStack.Peek();

                    if (!_currentContext.ShouldContinue())
                        continue;

                    var currentInst = _currentContext.CurrentInstruction;
                    _defaultOpCodeTable[currentInst.OpCode](this, currentInst);

                    if (!IsRunning && currentInst != null)
                        _currentContext.InstructionPointer += currentInst.Size;
                    IsRunning = false;
                }
            }
            catch (Exception ex)
            {
                OnFault(ex);
            }
            finally
            {
                Cleanup();
            }

            return State;
        }

        /// <summary>
        /// Called when an exception that cannot be caught by the VM is thrown.
        /// </summary>
        /// <param name="ex">The exception that caused the <see cref="VMState.FAULT"/> state.</param>
        internal void OnFault(Exception ex)
        {
            State = VMState.FAULT;
        }

        private void Cleanup()
        {
            while (_invocationStack.Count > 0)
            {
                var context = _invocationStack.Pop();
                context.Cleanup();
            }

            State = VMState.HALT;
            IsRunning = false;
        }
    }
}
