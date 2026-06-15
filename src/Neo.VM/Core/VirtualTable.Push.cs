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
using Neo.VM.Types;
using System.Numerics;

namespace Neo.VM.Core
{
    public partial class VirtualTable
    {
        /// <summary>
        /// Pushes an 8-bit signed integer onto the evaluation stack.
        /// <see cref="OpCode.PUSHINT8"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void PushInt8(VirtualMachine engine, VMInstruction instruction)
        {
            engine.CurrentContext!.Push(new BigInteger(instruction.Operand.Span));
        }

        /// <summary>
        /// Pushes an 16-bit signed integer onto the evaluation stack.
        /// <see cref="OpCode.PUSHINT16"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void PushInt16(VirtualMachine engine, VMInstruction instruction)
        {
            engine.CurrentContext!.Push(new BigInteger(instruction.Operand.Span));
        }

        /// <summary>
        /// Pushes an 32-bit signed integer onto the evaluation stack.
        /// <see cref="OpCode.PUSHINT32"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void PushInt32(VirtualMachine engine, VMInstruction instruction)
        {
            engine.CurrentContext!.Push(new BigInteger(instruction.Operand.Span));
        }

        /// <summary>
        /// Pushes an 64-bit signed integer onto the evaluation stack.
        /// <see cref="OpCode.PUSHINT64"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void PushInt64(VirtualMachine engine, VMInstruction instruction)
        {
            engine.CurrentContext!.Push(new BigInteger(instruction.Operand.Span));
        }

        /// <summary>
        /// Pushes an 128-bit signed integer onto the evaluation stack.
        /// <see cref="OpCode.PUSHINT128"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void PushInt128(VirtualMachine engine, VMInstruction instruction)
        {
            engine.CurrentContext!.Push(new BigInteger(instruction.Operand.Span));
        }

        /// <summary>
        /// Pushes an 256-bit signed integer onto the evaluation stack.
        /// <see cref="OpCode.PUSHINT256"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void PushInt256(VirtualMachine engine, VMInstruction instruction)
        {
            engine.CurrentContext!.Push(new BigInteger(instruction.Operand.Span));
        }

        /// <summary>
        /// Pushes a boolean value of true onto the evaluation stack.
        /// <see cref="OpCode.PUSHT"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void PushT(VirtualMachine engine, VMInstruction instruction)
        {
            engine.CurrentContext!.Push(true);
        }

        /// <summary>
        /// Pushes a boolean value of false onto the evaluation stack.
        /// <see cref="OpCode.PUSHF"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void PushF(VirtualMachine engine, VMInstruction instruction)
        {
            engine.CurrentContext!.Push(false);
        }

        /// <summary>
        /// Pushes the address of the specified instruction onto the evaluation stack.
        /// <see cref="OpCode.PUSHA"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void PushA(VirtualMachine engine, VMInstruction instruction)
        {
            // TODO: Add VMPointer to stack
        }

        /// <summary>
        /// Pushes a null onto the evaluation stack.
        /// <see cref="OpCode.PUSHNULL"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void PushNull(VirtualMachine engine, VMInstruction instruction)
        {
            engine.CurrentContext!.Push(VMNull.Instance);
        }

        /// <summary>
        /// Pushes a byte array with a length prefix onto the evaluation stack.
        /// The length of the array is 1 byte.
        /// <see cref="OpCode.PUSHDATA1"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void PushData1(VirtualMachine engine, VMInstruction instruction)
        {
            engine.CurrentContext!.Push(instruction.Operand[instruction.OperandPrefixSize..].ToArray());
        }

        /// <summary>
        /// Pushes a byte array with a length prefix onto the evaluation stack.
        /// The length of the array is 1 bytes.
        /// <see cref="OpCode.PUSHDATA2"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void PushData2(VirtualMachine engine, VMInstruction instruction)
        {
            engine.CurrentContext!.Push(instruction.Operand[instruction.OperandPrefixSize..].ToArray());
        }

        /// <summary>
        /// Pushes a byte array with a length prefix onto the evaluation stack.
        /// The length of the array is 4 bytes.
        /// <see cref="OpCode.PUSHDATA4"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void PushData4(VirtualMachine engine, VMInstruction instruction)
        {
            engine.CurrentContext!.Push(instruction.Operand[instruction.OperandPrefixSize..].ToArray());
        }

        /// <summary>
        /// Pushes the integer value of -1 onto the evaluation stack.
        /// <see cref="OpCode.PUSHM1"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void PushM1(VirtualMachine engine, VMInstruction instruction)
        {
            engine.CurrentContext!.Push(-1);
        }

        /// <summary>
        /// Pushes the integer value of 0 onto the evaluation stack.
        /// <see cref="OpCode.PUSH0"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void Push0(VirtualMachine engine, VMInstruction instruction)
        {
            engine.CurrentContext!.Push(0);
        }

        /// <summary>
        /// Pushes the integer value of 1 onto the evaluation stack.
        /// <see cref="OpCode.PUSH1"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void Push1(VirtualMachine engine, VMInstruction instruction)
        {
            engine.CurrentContext!.Push(1);
        }

        /// <summary>
        /// Pushes the integer value of 2 onto the evaluation stack.
        /// <see cref="OpCode.PUSH2"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void Push2(VirtualMachine engine, VMInstruction instruction)
        {
            engine.CurrentContext!.Push(2);
        }

        /// <summary>
        /// Pushes the integer value of 3 onto the evaluation stack.
        /// <see cref="OpCode.PUSH3"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void Push3(VirtualMachine engine, VMInstruction instruction)
        {
            engine.CurrentContext!.Push(3);
        }

        /// <summary>
        /// Pushes the integer value of 4 onto the evaluation stack.
        /// <see cref="OpCode.PUSH4"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void Push4(VirtualMachine engine, VMInstruction instruction)
        {
            engine.CurrentContext!.Push(4);
        }

        /// <summary>
        /// Pushes the integer value of 5 onto the evaluation stack.
        /// <see cref="OpCode.PUSH5"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void Push5(VirtualMachine engine, VMInstruction instruction)
        {
            engine.CurrentContext!.Push(5);
        }

        /// <summary>
        /// Pushes the integer value of 6 onto the evaluation stack.
        /// <see cref="OpCode.PUSH6"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void Push6(VirtualMachine engine, VMInstruction instruction)
        {
            engine.CurrentContext!.Push(6);
        }

        /// <summary>
        /// Pushes the integer value of 7 onto the evaluation stack.
        /// <see cref="OpCode.PUSH7"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void Push7(VirtualMachine engine, VMInstruction instruction)
        {
            engine.CurrentContext!.Push(7);
        }

        /// <summary>
        /// Pushes the integer value of 8 onto the evaluation stack.
        /// <see cref="OpCode.PUSH8"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void Push8(VirtualMachine engine, VMInstruction instruction)
        {
            engine.CurrentContext!.Push(8);
        }

        /// <summary>
        /// Pushes the integer value of 9 onto the evaluation stack.
        /// <see cref="OpCode.PUSH9"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void Push9(VirtualMachine engine, VMInstruction instruction)
        {
            engine.CurrentContext!.Push(9);
        }

        /// <summary>
        /// Pushes the integer value of 10 onto the evaluation stack.
        /// <see cref="OpCode.PUSH10"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void Push10(VirtualMachine engine, VMInstruction instruction)
        {
            engine.CurrentContext!.Push(10);
        }

        /// <summary>
        /// Pushes the integer value of 11 onto the evaluation stack.
        /// <see cref="OpCode.PUSH11"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void Push11(VirtualMachine engine, VMInstruction instruction)
        {
            engine.CurrentContext!.Push(11);
        }

        /// <summary>
        /// Pushes the integer value of 12 onto the evaluation stack.
        /// <see cref="OpCode.PUSH12"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void Push12(VirtualMachine engine, VMInstruction instruction)
        {
            engine.CurrentContext!.Push(12);
        }

        /// <summary>
        /// Pushes the integer value of 13 onto the evaluation stack.
        /// <see cref="OpCode.PUSH13"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void Push13(VirtualMachine engine, VMInstruction instruction)
        {
            engine.CurrentContext!.Push(13);
        }

        /// <summary>
        /// Pushes the integer value of 14 onto the evaluation stack.
        /// <see cref="OpCode.PUSH14"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void Push14(VirtualMachine engine, VMInstruction instruction)
        {
            engine.CurrentContext!.Push(14);
        }

        /// <summary>
        /// Pushes the integer value of 15 onto the evaluation stack.
        /// <see cref="OpCode.PUSH15"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void Push15(VirtualMachine engine, VMInstruction instruction)
        {
            engine.CurrentContext!.Push(15);
        }

        /// <summary>
        /// Pushes the integer value of 16 onto the evaluation stack.
        /// <see cref="OpCode.PUSH16"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void Push16(VirtualMachine engine, VMInstruction instruction)
        {
            engine.CurrentContext!.Push(16);
        }
    }
}
