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
using System.Diagnostics.CodeAnalysis;

namespace Neo.VM.Core
{
    public partial class VirtualTable
    {
        /// <summary>
        /// Determines whether the item on top of the evaluation stack is null.
        /// <see cref="OpCode.ISNULL"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 1, Push 1</remarks>
        public virtual void IsNull(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            var x = engine.CurrentContext!.Pop();

            engine.CurrentContext!.Push(x is VMNull || x is null);
        }

        /// <summary>
        /// Determines whether the item on top of the evaluation stack has a specified type.
        /// <see cref="OpCode.ISTYPE"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 1, Push 1</remarks>
        public virtual void IsType(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            var x = engine.CurrentContext!.Pop();
            var type = instruction.AsToken<VMObjectType>();

            if (type == VMObjectType.Any || Enum.IsDefined(type) == false)
                throw new InvalidOperationException($"Invalid type: {type}");

            engine.CurrentContext!.Push(x.Type == type);
        }

        /// <summary>
        /// Converts the item on top of the evaluation stack to a specified type.
        /// <see cref="OpCode.CONVERT"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 1, Push 1</remarks>
        public virtual void Convert(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            var x = engine.CurrentContext!.Pop();

            VMObject obj = instruction.AsToken<VMObjectType>() switch
            {
                VMObjectType.Any => throw new InvalidCastException($"Type {nameof(VMNull)} can't be converted to VMObjectType: {VMObjectType.Any}"),
                VMObjectType.Pointer => new VMPointer(engine.CurrentContext!.Script, unchecked((int)x.GetInteger())),
                VMObjectType.Boolean => x.GetBoolean(),
                VMObjectType.Integer => x.GetInteger(),
                VMObjectType.ByteString => new VMByteArray([.. x.AsSpan()]),
                VMObjectType.Buffer => new VMBuffer([.. x.AsSpan()]),
                VMObjectType.Array => new VMArray(x.GetChildren()),
                VMObjectType.Struct => new VMStruct(x.GetChildren()),
                VMObjectType.Map => new VMMap([.. x.GetChildren()]),
                VMObjectType.Interop or //=> new VMInteropInterface(x),
                _ => throw new InvalidCastException()
            };

            engine.CurrentContext!.Push(obj);
        }

        /// <summary>
        /// Aborts execution with a specified message.
        /// <see cref="OpCode.ABORTMSG"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 1, Push 0</remarks>
        [DoesNotReturn]
        public virtual void AbortMsg(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            var msg = engine.CurrentContext!.Pop().ToString();

            throw new Exception($"{OpCode.ABORTMSG} is executed. Reason: {msg}");
        }

        /// <summary>
        /// Asserts a condition with a specified message, throwing an exception if the condition is false.
        /// <see cref="OpCode.ASSERTMSG"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 2, Push 0</remarks>
        public virtual void AssertMsg(VirtualMachineEngine engine, OpCodeInst instruction)
        {
            var msg = engine.CurrentContext!.Pop().ToString();
            var x = engine.CurrentContext!.Pop().GetBoolean();

            if (x == false)
                throw new Exception($"{OpCode.ASSERTMSG} is executed with false result. Reason: {msg}");
        }
    }
}
