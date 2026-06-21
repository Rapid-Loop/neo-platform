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
using System;
using System.Numerics;

namespace Neo.VM.Core
{
    public partial class VirtualTable
    {
        /// <summary>
        /// Pushes the number of stack items in the evaluation stack onto the stack.
        /// <see cref="OpCode.DEPTH"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 0, Push 1</remarks>
        public virtual void Depth(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            engine.CurrentContext!.Push(engine.CurrentContext!.Frame.EvaluationStack.Count);
        }

        /// <summary>
        /// Removes the top item from the evaluation stack.
        /// <see cref="OpCode.DROP"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 1, Push 0</remarks>
        public virtual void Drop(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            engine.CurrentContext!.Pop();
        }

        /// <summary>
        /// Removes the second-to-top stack item.
        /// <see cref="OpCode.NIP"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void Nip(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            engine.CurrentContext!.RemoveAt(1);
        }

        /// <summary>
        /// Removes the n-th item from the top of the evaluation stack.
        /// <see cref="OpCode.XDROP"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 1, Push 0</remarks>
        public virtual void XDrop(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            var n = engine.CurrentContext!.Pop().GetInteger();

            if (n < BigInteger.Zero)
                throw new InvalidOperationException($"The negative value {n} is invalid for OpCode.{instruction.OpCode}.");

            engine.CurrentContext!.RemoveAt(checked((int)n));
        }

        /// <summary>
        /// Clears all items from the evaluation stack.
        /// <see cref="OpCode.CLEAR"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void Clear(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            engine.CurrentContext!.Clear();
        }

        /// <summary>
        /// Duplicates the item on the top of the evaluation stack.
        /// <see cref="OpCode.DUP"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 0, Push 1</remarks>
        public virtual void Dup(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            engine.CurrentContext!.Push(engine.CurrentContext!.Peek());
        }

        /// <summary>
        /// Copies the second item from the top of the evaluation stack and pushes the copy onto the stack.
        /// <see cref="OpCode.OVER"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 0, Push 1</remarks>
        public virtual void Over(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            engine.CurrentContext!.Push(engine.CurrentContext!.Peek(1));
        }

        /// <summary>
        /// Copies the nth item from the top of the evaluation stack and pushes the copy onto the stack.
        /// <see cref="OpCode.PICK"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 1, Push 1</remarks>
        public virtual void Pick(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            var n = engine.CurrentContext!.Pop().GetInteger();

            if (n < 0)
                throw new InvalidOperationException($"The negative value {n} is invalid for OpCode.{instruction.OpCode}.");

            engine.CurrentContext!.Push(engine.CurrentContext!.Peek(checked((int)n)));
        }

        /// <summary>
        /// Copies the top item on the evaluation stack and inserts the copy between the first and second items.
        /// <see cref="OpCode.TUCK"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void Tuck(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            engine.CurrentContext!.Insert(2, engine.CurrentContext!.Peek());
        }

        /// <summary>
        /// Swaps the top two items on the evaluation stack.
        /// <see cref="OpCode.SWAP"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 0, Push 0</remarks>
        public virtual void Swap(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            if (engine.CurrentContext!.Frame.EvaluationStack.Count < 2)
                throw new ArgumentOutOfRangeException($"Swap index is out of stack bounds: 1/{engine.CurrentContext!.Frame.EvaluationStack.Count}");

            engine.CurrentContext!.Swap(0, 1);
        }

        /// <summary>
        /// Left rotates the top three items on the evaluation stack.
        /// <see cref="OpCode.ROT"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 0, Push 0</remarks>
        public virtual void Rot(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            // ROT: [a, b, c] -> [b, c, a] (c is top)
            // Equivalent to: swap(1,2), swap(0,1)

            if (engine.CurrentContext!.Frame.EvaluationStack.Count < 3)
                throw new ArgumentOutOfRangeException($"Swap index is out of stack bounds: 2/{engine.CurrentContext!.Frame.EvaluationStack.Count}");

            engine.CurrentContext!.Swap(1, 2);
            engine.CurrentContext!.Swap(0, 1);
        }

        /// <summary>
        /// The item n back in the stack is moved to the top.
        /// <see cref="OpCode.ROLL"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 1, Push 1</remarks>
        public virtual void Roll(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            var n = engine.CurrentContext!.Pop().GetInteger();

            if (n < BigInteger.Zero)
                throw new InvalidOperationException($"The negative value {n} is invalid for OpCode.{instruction.OpCode}.");

            if (n == BigInteger.Zero) return;

            var x = engine.CurrentContext!.RemoveAt(unchecked((int)n));

            engine.CurrentContext!.Push(x);
        }

        /// <summary>
        /// Reverses the order of the top 3 items on the evaluation stack.
        /// <see cref="OpCode.REVERSE3"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void Reverse3(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            engine.CurrentContext!.Reverse(3);
        }

        /// <summary>
        /// Reverses the order of the top 4 items on the evaluation stack.
        /// <see cref="OpCode.REVERSE4"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void Reverse4(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            engine.CurrentContext!.Reverse(4);
        }

        /// <summary>
        /// Reverses the order of the top n items on the evaluation stack.
        /// <see cref="OpCode.REVERSEN"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 1, Push 0</remarks>
        public virtual void ReverseN(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            var n = engine.CurrentContext!.Pop().GetInteger();

            engine.CurrentContext!.Reverse(checked((int)n));
        }
    }
}
