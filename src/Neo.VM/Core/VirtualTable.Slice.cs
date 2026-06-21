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
using Neo.Core.VM.Type;
using Neo.VM.Types;
using System;

namespace Neo.VM.Core
{
    public partial class VirtualTable
    {
        /// <summary>
        /// Creates a new buffer with the specified length and pushes it onto the evaluation stack.
        /// <see cref="OpCode.NEWBUFFER"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 1, Push 1</remarks>
        public virtual void NewBuffer(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            var length = unchecked((int)engine.CurrentContext!.Pop().GetInteger());

            engine.Limits.AssertMaxItemSize(length);
            engine.CurrentContext!.Push(new VMBuffer(length));
        }

        /// <summary>
        /// Copies a specified number of bytes from one buffer to another buffer.
        /// <see cref="OpCode.MEMCPY"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 5, Push 0</remarks>
        public virtual void Memcpy(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            var count = unchecked((int)engine.CurrentContext!.Pop().GetInteger());
            if (count < 0)
                throw new InvalidOperationException($"The count can not be negative for {nameof(OpCode.MEMCPY)}, count: {count}.");

            var si = unchecked((int)engine.CurrentContext!.Pop().GetInteger());
            if (si < 0)
                throw new InvalidOperationException($"The source index can not be negative for {nameof(OpCode.MEMCPY)}, index: {si}.");

            var src = new VMBuffer(engine.CurrentContext!.Pop().AsSpan());
            if (checked(si + count) > src.Length)
                throw new InvalidOperationException($"The source index + count is out of range for {nameof(OpCode.MEMCPY)}, index: {si}, count: {count}, {si}/[0, {src.Length}].");

            var di = unchecked((int)engine.CurrentContext!.Pop().GetInteger());
            if (di < 0)
                throw new InvalidOperationException($"The destination index can not be negative for {nameof(OpCode.MEMCPY)}, index: {si}.");

            var dst = new VMBuffer(engine.CurrentContext!.Pop().AsSpan());
            if (checked(di + count) > dst.Size)
                throw new InvalidOperationException($"The destination index + count is out of range for {nameof(OpCode.MEMCPY)}, index: {di}, count: {count}, {di}/[0, {dst.Size}].");

            src.CopyTo(dst, si, di, count);
        }

        /// <summary>
        /// Concatenates two buffers and pushes the result onto the evaluation stack.
        /// The result is the first pushed item concatenated with the second pushed item.
        /// <see cref="OpCode.CAT"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 2, Push 1</remarks>
        public virtual void Cat(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            var x2 = engine.CurrentContext!.Pop();
            var x1 = engine.CurrentContext!.Pop();

            switch (x2.Type)
            {
                case VMObjectType.Array:
                case VMObjectType.Struct:
                case VMObjectType.Map:
                case VMObjectType.Interop:
                    // NOTE: Matching official neo-vm (see https://github.com/neo-project/proposals/issues/234)
                    throw new InvalidCastException();
                default:
                    break;
            }

            switch (x1.Type)
            {
                case VMObjectType.Array:
                case VMObjectType.Struct:
                case VMObjectType.Map:
                case VMObjectType.Interop:
                    // NOTE: Matching official neo-vm (see https://github.com/neo-project/proposals/issues/234)
                    throw new InvalidCastException();
                default:
                    break;
            }

            var span1 = x1.AsSpan();
            var span2 = x2.AsSpan();
            var length = span1.Length + span2.Length;

            engine.Limits.AssertMaxItemSize(length);

            var result = new VMBuffer([.. span1, .. span2]);
            engine.CurrentContext!.Push(result);
        }

        /// <summary>
        /// Extracts a sub-buffer from the specified buffer and pushes it onto the evaluation stack.
        /// <see cref="OpCode.SUBSTR"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 3, Push 1</remarks>
        public virtual void SubStr(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            var count = unchecked((int)engine.CurrentContext!.Pop().GetInteger());
            if (count < 0)
                throw new InvalidOperationException($"The count can not be negative for {nameof(OpCode.SUBSTR)}, count: {count}.");

            var index = unchecked((int)engine.CurrentContext!.Pop().GetInteger());
            if (index < 0)
                throw new InvalidOperationException($"The index can not be negative for {nameof(OpCode.SUBSTR)}, index: {index}.");

            var x = engine.CurrentContext!.Pop();

            switch (x.Type)
            {
                case VMObjectType.Array:
                case VMObjectType.Struct:
                case VMObjectType.Map:
                case VMObjectType.Interop:
                    // NOTE: Matching official neo-vm (see https://github.com/neo-project/proposals/issues/234)
                    throw new InvalidCastException();
                default:
                    break;
            }

            var span = x.AsSpan();

            if (checked(index + count) > span.Length)
                throw new InvalidOperationException($"The index + count is out of range for {nameof(OpCode.SUBSTR)}, index: {index}, count: {count}, {index + count}/[0, {span.Length}].");

            var result = new VMBuffer([.. span.Slice(index, count)]);
            engine.CurrentContext!.Push(result);
        }

        /// <summary>
        /// Extracts a specified number of characters from the left side of the buffer and pushes them onto the evaluation stack.
        /// <see cref="OpCode.LEFT"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 2, Push 1</remarks>
        public virtual void Left(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            var count = unchecked((int)engine.CurrentContext!.Pop().GetInteger());
            if (count < 0)
                throw new InvalidOperationException($"The count can not be negative for {nameof(OpCode.LEFT)}, count: {count}.");

            var x = engine.CurrentContext!.Pop();

            switch (x.Type)
            {
                case VMObjectType.Array:
                case VMObjectType.Struct:
                case VMObjectType.Map:
                case VMObjectType.Interop:
                    // NOTE: Matching official neo-vm (see https://github.com/neo-project/proposals/issues/234)
                    throw new InvalidCastException();
                default:
                    break;
            }

            var span = x.AsSpan();

            if (count > span.Length)
                throw new InvalidOperationException($"The count is out of range for {nameof(OpCode.LEFT)}, {count}/[0, {span.Length}].");

            var result = new VMBuffer([.. span[..count]]);
            engine.CurrentContext!.Push(result);
        }

        /// <summary>
        /// Extracts a specified number of characters from the right side of the buffer and pushes them onto the evaluation stack.
        /// <see cref="OpCode.RIGHT"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 2, Push 1</remarks>
        public virtual void Right(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            var count = unchecked((int)engine.CurrentContext!.Pop().GetInteger());
            if (count < 0)
                throw new InvalidOperationException($"The count can not be negative for {nameof(OpCode.RIGHT)}, count: {count}.");

            var x = engine.CurrentContext!.Pop();

            switch (x.Type)
            {
                case VMObjectType.Array:
                case VMObjectType.Struct:
                case VMObjectType.Map:
                case VMObjectType.Interop:
                    // NOTE: Matching official neo-vm (see https://github.com/neo-project/proposals/issues/234)
                    throw new InvalidCastException();
                default:
                    break;
            }

            var span = x.AsSpan();

            if (count > span.Length)
                throw new InvalidOperationException($"The count is out of range for {nameof(OpCode.RIGHT)}, {count}/[0, {span.Length}].");

            var result = new VMBuffer([.. span[^count..^0]]);
            engine.CurrentContext!.Push(result);
        }
    }
}
