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
using System;
using System.Diagnostics.CodeAnalysis;

namespace Neo.VM.Core
{
    public partial class VirtualTable
    {
        /// <summary>
        /// No operation. Does nothing.
        /// <see cref="OpCode.NOP"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void Nop(VirtualMachineEngine engine, OpCodeInst instruction)
        {
        }

        /// <summary>
        /// Jumps to the specified offset from the current instruction pointer,
        /// where the offset is obtained from the first operand of the instruction and interpreted as a signed byte.
        /// <see cref="OpCode.JMP"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction containing the offset as the first operand.</param>
        public virtual void Jmp(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            ExecuteJumpOffset(engine, instruction.AsToken<sbyte>());
        }

        /// <summary>
        /// Jumps to the specified offset from the current instruction pointer,
        /// where the offset is obtained from the first operand of the instruction and interpreted as a 32-bit signed integer.
        /// <see cref="OpCode.JMP_L"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction containing the offset as the first operand.</param>
        public virtual void Jmp_L(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            ExecuteJumpOffset(engine, instruction.AsToken<int>());
        }

        /// <summary>
        /// Jumps to the specified offset from the current instruction pointer
        /// if the boolean result of popping the evaluation stack is true.
        /// The offset is obtained from the instruction's first operand interpreted as a signed byte.
        /// <see cref="OpCode.JMPIF"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction containing the offset as the first operand.</param>
        /// <remarks>Pop 1, Push 0</remarks>
        public virtual void JmpIf(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            if (engine.CurrentContext!.Pop().GetBoolean())
                ExecuteJumpOffset(engine, instruction.AsToken<sbyte>());
        }

        /// <summary>
        /// Jumps to the specified offset from the current instruction pointer
        /// if the boolean result of popping the evaluation stack is true.
        /// The offset is obtained from the instruction's first operand interpreted as a 32-bit signed integer.
        /// <see cref="OpCode.JMPIF_L"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction containing the offset as the first operand.</param>
        /// <remarks>Pop 1, Push 0</remarks>
        public virtual void JmpIf_L(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            if (engine.CurrentContext!.Pop().GetBoolean())
                ExecuteJumpOffset(engine, instruction.AsToken<int>());
        }

        /// <summary>
        /// Jumps to the specified offset from the current instruction pointer
        /// if the boolean result of popping the evaluation stack is false.
        /// The offset is obtained from the instruction's first operand interpreted as a signed byte.
        /// <see cref="OpCode.JMPIFNOT"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction containing the offset as the first operand.</param>
        /// <remarks>Pop 1, Push 0</remarks>
        public virtual void JmpIfNot(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            if (!engine.CurrentContext!.Pop().GetBoolean())
                ExecuteJumpOffset(engine, instruction.AsToken<sbyte>());
        }

        /// <summary>
        /// Jumps to the specified offset from the current instruction pointer
        /// if the boolean result of popping the evaluation stack is false.
        /// The offset is obtained from the instruction's first operand interpreted as a 32-bit signed integer.
        /// <see cref="OpCode.JMPIFNOT_L"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction containing the offset as the first operand.</param>
        /// <remarks>Pop 1, Push 0</remarks>
        public virtual void JmpIfNot_L(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            if (!engine.CurrentContext!.Pop().GetBoolean())
                ExecuteJumpOffset(engine, instruction.AsToken<int>());
        }

        /// <summary>
        /// Jumps to the specified offset from the current instruction pointer
        /// if the two integers popped from the evaluation stack are equal.
        /// The offset is obtained from the instruction's first operand interpreted as a signed byte.
        /// <see cref="OpCode.JMPEQ"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction containing the offset as the first operand.</param>
        /// <remarks>Pop 2, Push 0</remarks>
        public virtual void JmpEq(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            var x2 = engine.CurrentContext!.Pop().GetInteger();
            var x1 = engine.CurrentContext!.Pop().GetInteger();

            if (x1 == x2)
                ExecuteJumpOffset(engine, instruction.AsToken<sbyte>());
        }

        /// <summary>
        /// Jumps to the specified offset from the current instruction pointer
        /// if the two integers popped from the evaluation stack are equal.
        /// The offset is obtained from the instruction's first operand interpreted as a 32-bit signed integer.
        /// <see cref="OpCode.JMPEQ_L"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction containing the offset as the first operand.</param>
        /// <remarks>Pop 2, Push 0</remarks>
        public virtual void JmpEq_L(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            var x2 = engine.CurrentContext!.Pop().GetInteger();
            var x1 = engine.CurrentContext!.Pop().GetInteger();

            if (x1 == x2)
                ExecuteJumpOffset(engine, instruction.AsToken<int>());
        }

        /// <summary>
        /// Jumps to the specified offset from the current instruction pointer
        /// if the two integers popped from the evaluation stack are not equal.
        /// The offset is obtained from the instruction's first operand interpreted as a signed byte.
        /// <see cref="OpCode.JMPNE"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction containing the offset as the first operand.</param>
        /// <remarks>Pop 2, Push 0</remarks>
        public virtual void JmpNe(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            var x2 = engine.CurrentContext!.Pop().GetInteger();
            var x1 = engine.CurrentContext!.Pop().GetInteger();

            if (x1 != x2)
                ExecuteJumpOffset(engine, instruction.AsToken<sbyte>());
        }

        /// <summary>
        /// Jumps to the specified offset from the current instruction pointer
        /// if the two integers popped from the evaluation stack are not equal.
        /// The offset is obtained from the instruction's first operand interpreted as a 32-bit signed integer.
        /// <see cref="OpCode.JMPNE_L"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction containing the offset as the first operand.</param>
        /// <remarks>Pop 2, Push 0</remarks>
        public virtual void JmpNe_L(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            var x2 = engine.CurrentContext!.Pop().GetInteger();
            var x1 = engine.CurrentContext!.Pop().GetInteger();

            if (x1 != x2)
                ExecuteJumpOffset(engine, instruction.AsToken<int>());
        }

        /// <summary>
        /// Jumps to the specified offset from the current instruction pointer
        /// if the first integer pushed onto the evaluation stack is greater than the second integer.
        /// The offset is obtained from the instruction's first operand interpreted as a signed byte.
        /// <see cref="OpCode.JMPGT"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction containing the offset as the first operand.</param>
        /// <remarks>Pop 2, Push 0</remarks>
        public virtual void JmpGt(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            var x2 = engine.CurrentContext!.Pop().GetInteger();
            var x1 = engine.CurrentContext!.Pop().GetInteger();

            if (x1 > x2)
                ExecuteJumpOffset(engine, instruction.AsToken<sbyte>());
        }

        /// <summary>
        /// Jumps to the specified offset from the current instruction pointer
        /// if the first integer pushed onto the evaluation stack is greater than the second integer.
        /// The offset is obtained from the instruction's first operand interpreted as a 32-bit signed integer.
        /// <see cref="OpCode.JMPGT_L"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction containing the offset as the first operand.</param>
        /// <remarks>Pop 2, Push 0</remarks>
        public virtual void JmpGt_L(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            var x2 = engine.CurrentContext!.Pop().GetInteger();
            var x1 = engine.CurrentContext!.Pop().GetInteger();

            if (x1 > x2)
                ExecuteJumpOffset(engine, instruction.AsToken<int>());
        }

        /// <summary>
        /// Jumps to the specified offset from the current instruction pointer
        /// if the first integer pushed onto the evaluation stack is greater than or equal to the second integer.
        /// The offset is obtained from the instruction's first operand interpreted as a signed byte.
        /// <see cref="OpCode.JMPGE"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction containing the offset as the first operand.</param>
        /// <remarks>Pop 2, Push 0</remarks>
        public virtual void JmpGe(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            var x2 = engine.CurrentContext!.Pop().GetInteger();
            var x1 = engine.CurrentContext!.Pop().GetInteger();

            if (x1 >= x2)
                ExecuteJumpOffset(engine, instruction.AsToken<sbyte>());
        }

        /// <summary>
        /// Jumps to the specified offset from the current instruction pointer
        /// if the first integer pushed onto the evaluation stack is greater than or equal to the second integer.
        /// The offset is obtained from the instruction's first operand interpreted as a 32-bit signed integer.
        /// <see cref="OpCode.JMPGE_L"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction containing the offset as the first operand.</param>
        /// <remarks>Pop 2, Push 0</remarks>
        public virtual void JmpGe_L(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            var x2 = engine.CurrentContext!.Pop().GetInteger();
            var x1 = engine.CurrentContext!.Pop().GetInteger();

            if (x1 >= x2)
                ExecuteJumpOffset(engine, instruction.AsToken<int>());
        }

        /// <summary>
        /// Jumps to the specified offset from the current instruction pointer
        /// if the first integer pushed onto the evaluation stack is less than the second integer.
        /// The offset is obtained from the instruction's first operand interpreted as a signed byte.
        /// <see cref="OpCode.JMPLT"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction containing the offset as the first operand.</param>
        /// <remarks>Pop 2, Push 0</remarks>
        public virtual void JmpLt(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            var x2 = engine.CurrentContext!.Pop().GetInteger();
            var x1 = engine.CurrentContext!.Pop().GetInteger();

            if (x1 < x2)
                ExecuteJumpOffset(engine, instruction.AsToken<sbyte>());
        }

        /// <summary>
        /// Jumps to the specified offset from the current instruction pointer
        /// if the first integer pushed onto the evaluation stack is less than the second integer.
        /// The offset is obtained from the instruction's first operand interpreted as a 32-bit signed integer.
        /// <see cref="OpCode.JMPLT_L"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction containing the offset as the first operand.</param>
        /// <remarks>Pop 2, Push 0</remarks>
        public virtual void JmpLt_L(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            var x2 = engine.CurrentContext!.Pop().GetInteger();
            var x1 = engine.CurrentContext!.Pop().GetInteger();

            if (x1 < x2)
                ExecuteJumpOffset(engine, instruction.AsToken<int>());
        }

        /// <summary>
        /// Jumps to the specified offset from the current instruction pointer
        /// if the first integer pushed onto the evaluation stack is less than or equal to the second integer.
        /// The offset is obtained from the instruction's first operand interpreted as a signed byte.
        /// <see cref="OpCode.JMPLE"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction containing the offset as the first operand.</param>
        /// <remarks>Pop 2, Push 0</remarks>
        public virtual void JmpLe(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            var x2 = engine.CurrentContext!.Pop().GetInteger();
            var x1 = engine.CurrentContext!.Pop().GetInteger();

            if (x1 <= x2)
                ExecuteJumpOffset(engine, instruction.AsToken<sbyte>());
        }

        /// <summary>
        /// Jumps to the specified offset from the current instruction pointer
        /// if the first integer pushed onto the evaluation stack is less than or equal to the second integer.
        /// The offset is obtained from the instruction's first operand interpreted as a 32-bit signed integer.
        /// <see cref="OpCode.JMPLE_L"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction containing the offset as the first operand.</param>
        /// <remarks>Pop 2, Push 0</remarks>
        public virtual void JmpLe_L(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            var x2 = engine.CurrentContext!.Pop().GetInteger();
            var x1 = engine.CurrentContext!.Pop().GetInteger();

            if (x1 <= x2)
                ExecuteJumpOffset(engine, instruction.AsToken<int>());
        }

        /// <summary>
        /// Calls a method specified by the offset from the current instruction pointer.
        /// The offset is obtained from the instruction's first operand interpreted as a signed byte.
        /// <see cref="OpCode.CALL"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction containing the offset as the first operand.</param>
        public virtual void Call(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            ExecuteCall(engine, checked(engine.CurrentContext!.InstructionPointer + instruction.AsToken<sbyte>()));
        }

        /// <summary>
        /// Calls a method specified by the offset from the current instruction pointer.
        /// The offset is obtained from the instruction's first operand interpreted as a 32-bit signed integer.
        /// <see cref="OpCode.CALL_L"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction containing the offset as the first operand.</param>
        public virtual void Call_L(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            ExecuteCall(engine, checked(engine.CurrentContext!.InstructionPointer + instruction.AsToken<int>()));
        }

        /// <summary>
        /// Calls a method specified by the pointer pushed onto the evaluation stack.
        /// It verifies if the pointer belongs to the current script.
        /// <see cref="OpCode.CALLA"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The current instruction.</param>
        /// <remarks>Pop 1, Push 0</remarks>
        public virtual void CallA(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            var x = (VMPointer)engine.CurrentContext!.Pop();

            if (x.Script.Span.SequenceEqual(engine.CurrentContext!.Script.Span))
                throw new InvalidOperationException("Pointers can't be shared between scripts");

            ExecuteCall(engine, x.Position);
        }

        /// <summary>
        /// Calls the function described by the token.
        /// <see cref="OpCode.CALLT"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The current instruction.</param>
        public virtual void CallT(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            throw new InvalidOperationException($"Token not found: {instruction.AsToken<ushort>()}");
        }

        /// <summary>
        /// Aborts the execution by turning the virtual machine state to FAULT immediately, and the exception cannot be caught.
        /// <see cref="OpCode.ABORT"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The current instruction.</param>
        public virtual void Abort(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            throw new Exception($"{OpCode.ABORT} is executed.");
        }

        /// <summary>
        /// Pop the top value of the stack. If it's false, exit vm execution and set vm state to FAULT.
        /// <see cref="OpCode.ASSERT"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The current instruction.</param>
        /// <remarks>Pop 1, Push 0</remarks>
        public virtual void Assert(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            var x = engine.CurrentContext!.Pop().GetBoolean();
            if (!x)
                throw new Exception($"{OpCode.ASSERT} is executed with false result.");
        }

        /// <summary>
        /// Pop the top value of the stack, and throw it.
        /// <see cref="OpCode.THROW"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The current instruction.</param>
        /// <remarks>Pop 1, Push 0</remarks>
        public virtual void Throw(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            ExecuteThrow(engine, engine.CurrentContext!.Pop());
        }

        /// <summary>
        /// Initiates a try block with the specified catch and finally offsets.
        /// If there's no catch block, set CatchOffset to 0. If there's no finally block, set FinallyOffset to 0.
        /// where the catch offset is obtained from the first operand of the instruction and interpreted as a signed byte，
        /// the catch offset is obtained from the second operand of the instruction and interpreted as a signed byte.
        /// <see cref="OpCode.TRY"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The current instruction.</param>
        public virtual void Try(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            int catchOffset = instruction.AsToken<sbyte>();
            int finallyOffset = instruction.AsToken<sbyte>(1);

            ExecuteTry(engine, catchOffset, finallyOffset);
        }

        /// <summary>
        /// Initiates a try block with the specified catch and finally offsets.
        /// If there's no catch block, set CatchOffset to 0. If there's no finally block, set FinallyOffset to 0.
        /// where the catch offset is obtained from the first operand of the instruction and interpreted as a 32-bit signed integer，
        /// the catch offset is obtained from the second operand of the instruction and interpreted as a 32-bit signed integer.
        /// <see cref="OpCode.TRY_L"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The current instruction.</param>
        public virtual void Try_L(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            var catchOffset = instruction.AsToken<int>();
            var finallyOffset = instruction.AsToken<int>(1);

            ExecuteTry(engine, catchOffset, finallyOffset);
        }

        /// <summary>
        /// Ensures that the appropriate surrounding finally blocks are executed,
        /// then unconditionally transfers control to the specific target instruction represented as a 1-byte signed offset
        /// from the beginning of the current instruction.
        /// <see cref="OpCode.ENDTRY"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The current instruction.</param>
        public virtual void EndTry(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            var endOffset = instruction.AsToken<sbyte>();

            ExecuteEndTry(engine, endOffset);
        }

        /// <summary>
        /// Ensures that the appropriate surrounding finally blocks are executed,
        /// then unconditionally transfers control to the specific target instruction represented as a 4-byte signed offset
        /// from the beginning of the current instruction.
        /// <see cref="OpCode.ENDTRY_L"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The current instruction.</param>
        public virtual void EndTry_L(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            var endOffset = instruction.AsToken<int>();

            ExecuteEndTry(engine, endOffset);
        }

        /// <summary>
        /// Ends the finally block. If no exception occurs or is caught,
        /// the VM jumps to the target instruction specified by ENDTRY/ENDTRY_L.
        /// Otherwise, the VM rethrows the exception to the upper layer.
        /// <see cref="OpCode.ENDFINALLY"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The current instruction.</param>
        public virtual void EndFinally(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            // TODO: Add this
        }

        /// <summary>
        /// Returns from the current method.
        /// <see cref="OpCode.RET"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The current instruction.</param>
        public virtual void Ret(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            var context = engine.InvocationStack.Pop();
            var stackEval = engine.InvocationStack.Count == 0 ?
                    engine.ResultStack :
                    engine.InvocationStack.Peek().Frame.EvaluationStack;

            if (context.Frame.EvaluationStack != stackEval)
            {
                while (context.Frame.EvaluationStack.Count > 0)
                    stackEval.Push(context.Pop());
            }

            if (engine.InvocationStack.Count == 0)
                engine.State = VMState.HALT;

            engine.UnloadedContext(context);
        }

        /// <summary>
        /// Calls to an interop service.
        /// <see cref="OpCode.SYSCALL"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The current instruction.</param>
        public virtual void SysCall(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            engine.ExecuteSystemCall(instruction.AsToken<uint>());
        }

        #region Execute methods

        /// <summary>
        /// Executes a call operation by loading a new execution context at the specified position.
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="position">The position to load the new execution context.</param>
        public virtual void ExecuteCall(VirtualMachineEngine engine, int position)
        {
            var context = engine.CurrentContext!;
            var script = context.Script.Span[position..];
            var newContext = new ExecutionContext([.. script], engine.ActiveFork, context.BlockHeight, engine.GasLeft - context.GasConsumed, context.Depth + 1, context);

            context.InstructionPointer += context.CurrentInstruction.Size;

            engine.LoadContext(newContext);
        }

        /// <summary>
        /// Executes the end of a try block, either popping it from the try stack or transitioning to the finally block.
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="endOffset">The offset to the end of the try block.</param>
        public virtual void ExecuteEndTry(VirtualMachineEngine engine, int endOffset)
        {
            // TODO: Add this
        }

        /// <summary>
        /// Executes a jump operation to the specified position.
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="position">The position to jump to.</param>
        public virtual void ExecuteJump(VirtualMachineEngine engine, int position)
        {
            if (position < 0 || position >= engine.CurrentContext!.Script.Length)
                throw new ArgumentOutOfRangeException($"Jump out of range for position: {position}");

            engine.CurrentContext.InstructionPointer = position;
        }

        /// <summary>
        /// Executes a jump operation with the specified offset from the current instruction pointer.
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="offset">The offset from the current instruction pointer.</param>
        public virtual void ExecuteJumpOffset(VirtualMachineEngine engine, int offset)
        {
            ExecuteJump(engine, checked(engine.CurrentContext!.InstructionPointer + offset));
        }

        /// <summary>
        /// Executes a try block operation with the specified catch and finally offsets.
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="catchOffset">The catch block offset.</param>
        /// <param name="finallyOffset">The finally block offset.</param>
        public virtual void ExecuteTry(VirtualMachineEngine engine, int catchOffset, int finallyOffset)
        {
            // TODO: Add this
        }

        /// <summary>
        /// Executes a throw operation, handling any surrounding try-catch-finally blocks.
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="ex">The exception to throw.</param>
        [DoesNotReturn]
        public virtual void ExecuteThrow(VirtualMachineEngine engine, VMObject? ex)
        {
            throw new Exception($"{ex}");
        }

        #endregion
    }
}
