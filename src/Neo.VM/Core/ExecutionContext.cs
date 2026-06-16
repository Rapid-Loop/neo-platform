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

using Neo.Core.VM;
using Neo.Core.VM.Specs;
using Neo.VM.Types;
using System;
using System.Collections.Generic;

namespace Neo.VM.Core
{
    public class ExecutionContext
    {
        /// <summary>
        /// The script being executed (bytecode)
        /// </summary>
        public ReadOnlyMemory<byte> Script => _script;

        /// <summary>
        /// The pointer indicating the current instruction.
        /// </summary>
        public int InstructionPointer { get; internal set; }

        /// <summary>
        /// Returns the current <see cref="VMInstruction"/>.
        /// </summary>
        public VMInstruction CurrentInstruction => new(_script, InstructionPointer);

        /// <summary>
        /// Returns the next <see cref="VMInstruction"/>.
        /// </summary>
        public VMInstruction NextInstruction => new(_script, InstructionPointer + CurrentInstruction.Size);

        /// <summary>
        /// Current stack frame
        /// </summary>
        public StackFrame Frame { get; }

        /// <summary>
        /// Gas remaining for this execution
        /// </summary>
        public long GasConsumed => _maxGasConsumed;

        /// <summary>
        /// Whether this context is currently executing
        /// </summary>
        public bool IsExecuting { get; internal set; } = true;

        /// <summary>
        /// Parent context (for nested calls)
        /// </summary>
        public ExecutionContext? Parent => _parentContext;

        /// <summary>
        /// Custom state / context data (e.g., contract storage, runtime)
        /// </summary>
        public Dictionary<Type, object> State { get; } = [];

        public HardFork HardFork => _fork;

        public uint BlockHeight => _blockHeight;

        /// <summary>
        /// Invocation stack depth
        /// </summary>
        public int Depth => _contextDepth;

        private readonly ReadOnlyMemory<byte> _script;
        private readonly HardFork _fork;
        private readonly uint _blockHeight;

        private readonly ExecutionContext? _parentContext;
        private readonly int _contextDepth;

        private long _initialGasLeft;
        private long _maxGasConsumed;

        public ExecutionContext(byte[] script, HardFork fork = HardFork.Genesis, uint blockHeight = 0, long initialGas = 1_000000L, int depth = 0, ExecutionContext? parent = null)
        {
            _script = script.Clone() as byte[] ?? throw new ArgumentNullException(nameof(script));
            _blockHeight = blockHeight;
            _fork = fork;

            _initialGasLeft = initialGas;
            _contextDepth = depth;
            _parentContext = parent;

            Frame = new(_parentContext?.Frame);
        }

        /// <summary>
        /// Check if execution should continue
        /// </summary>
        public bool ShouldContinue()
        {
            return IsExecuting && InstructionPointer < Script.Length && _initialGasLeft > 0;
        }

        /// <summary>
        /// Consume gas for an operation
        /// </summary>
        public bool ConsumeGas(OpCode opcode)
        {
            var cost = GasTable.GetGasCost(opcode, HardFork);

            if (_initialGasLeft < cost)
            {
                IsExecuting = false;
                return false;
            }

            _initialGasLeft -= cost;
            _maxGasConsumed += cost;
            return true;
        }

        /// <summary>
        /// Push value onto current frame's evaluation stack
        /// </summary>
        public void Push(VMObject item)
        {
            Frame.Push(item);
        }

        /// <summary>
        /// Pop value from current frame's evaluation stack
        /// </summary>
        public VMObject Pop()
        {
            return Frame.Pop();
        }

        /// <summary>
        /// Peek value from current frame's evaluation stack without removing
        /// </summary>
        public VMObject Peek()
        {
            return Frame.Peek();
        }

        /// <summary>
        /// Peek value from current frame's evaluation stack without removing by an index
        /// </summary>
        public VMObject Peek(int index)
        {
            return Frame.Peek(index);
        }

        public void Insert(int index, VMObject item)
        {
            Frame.Insert(index, item);
        }

        public void Swap(int fromIndex, int toIndex)
        {
            Frame.Swap(fromIndex, toIndex);
        }

        public void Reverse(int n)
        {
            Frame.Reverse(n);
        }

        /// <summary>
        /// Pop value from current frame's evaluation stack at an index
        /// </summary>
        public VMObject RemoveAt(int index)
        {
            return Frame.RemoveAt(index);
        }

        /// <summary>
        /// Safely clear current frame's evaluation stack and release all references
        /// </summary>
        public void Clear()
        {
            Frame.Clear();
        }

        /// <summary>
        /// Clean up this execution context
        /// </summary>
        public void Cleanup()
        {
            Frame.Cleanup();
            IsExecuting = false;

            // Clear custom state
            State.Clear();
        }
    }
}
