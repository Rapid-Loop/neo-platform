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

using Neo.Core;
using Neo.Core.Blockchain;
using Neo.Core.Blockchain.Interface;
using Neo.Core.VM;
using Neo.VM.Core;
using Neo.VM.Pipeline;
using Neo.VM.Types;
using System;
using System.Collections.Generic;

namespace Neo.VM
{
    /// <summary>
    /// Represents the VM used to execute the script.
    /// </summary>
    public partial class VirtualMachineEngine : IDisposable
    {
        public VMState State { get; internal set; }

        public long GasConsumed => _maxGasConsumed;

        public long GasLeft => _maxGasLimit - _maxGasConsumed;

        public long GasLimit => _maxGasLimit;

        /// <summary>
        /// Restrictions on the VM.
        /// </summary>
        public ExecutionEngineLimits Limits => _limits;

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

        public Exception? FaultException { get; private set; }

        public HardFork ActiveFork => _currentFork;


        private readonly Stack<ExecutionContext> _invocationStack = [];
        private readonly VirtualTable _defaultOpCodeTable;
        private readonly ExecutionEngineLimits _limits;
        private readonly long _maxGasLimit;

        private readonly VirtualMachinePipeline _pipeline;

        private readonly ProtocolSettings _protocolSettings;
        private readonly Block _persistingBlock;
        private readonly IVerifiable _container;
        private readonly HardFork _currentFork;

        private ExecutionContext? _currentContext;
        private ExecutionContext? _entryContext;

        private long _maxGasConsumed;

        public VirtualMachineEngine(
            Block? persistingBlock = default,
            IVerifiable? container = default,
            ProtocolSettings? protocolSettings = default,
            VirtualTable? opCodeTable = default,
            ExecutionEngineLimits? limits = default,
            VirtualMachinePipeline? pipeline = default)
        {
            _persistingBlock = persistingBlock ?? new();
            _container = container ?? new Transaction();
            _protocolSettings = protocolSettings ?? ProtocolSettings.Default;
            _maxGasLimit = container is Transaction tx ? tx.SystemFee : ExecutionEngineLimits.MaxGas;
            _defaultOpCodeTable = opCodeTable ?? VirtualTable.Default;
            _limits = limits ?? ExecutionEngineLimits.Default;
            _currentFork = _protocolSettings.GetActiveHardFork(_persistingBlock.Index);
            _pipeline = pipeline ?? VirtualMachinePipeline.Empty;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public ExecutionContext LoadScript(byte[] script, int initialPosition = 0)
        {
            var activeFork = _protocolSettings.GetActiveHardFork(_persistingBlock.Index);

            var rootContext = new ExecutionContext(
                script,
                activeFork,
                _persistingBlock.Index,
                _maxGasLimit - _maxGasConsumed
            );

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
                throw new InvalidOperationException($"{nameof(Limits.MaxInvocationStackSize)} exceed: {InvocationStack.Count}");

            _entryContext ??= context;
            _currentContext = context;

            // NOTE: May need to update the context gas with leftover gas

            InvocationStack.Push(context);
        }

        /// <summary>
        /// Called when a context is unloaded.
        /// </summary>
        /// <param name="context">The context being unloaded.</param>
        internal virtual void UnloadedContext(ExecutionContext context)
        {
            _maxGasConsumed += context.GasConsumed;

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
            _maxGasConsumed = 0;

            _pipeline.PreExecution();

            if (State == VMState.BREAK)
                State = VMState.NONE;

            try
            {
                while ((State != VMState.HALT && State != VMState.FAULT)
                    && _invocationStack.Count > 0)
                {
                    _currentContext = _invocationStack.Peek();

                    var currentInst = _currentContext.CurrentInstruction;

                    _pipeline.PreExecute(_currentContext);

                    if (_currentContext.ConsumeGas(currentInst.OpCode))
                        _defaultOpCodeTable[currentInst.OpCode](this, currentInst);

                    if (_currentContext is not null && _currentContext.ShouldContinue())
                        _currentContext.InstructionPointer += currentInst.Size;

                    _pipeline.PostExecute(_currentContext);
                }
            }
            catch (Exception ex)
            {
                OnFault(ex);
            }
            finally
            {
                Cleanup();
                _pipeline.PostExecution();
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
            FaultException = ex;
        }

        private void Cleanup()
        {
            while (_invocationStack.Count > 0)
            {
                var context = _invocationStack.Pop();
                context.Cleanup();
            }
        }
    }
}
