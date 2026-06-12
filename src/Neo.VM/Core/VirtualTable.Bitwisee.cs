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

namespace Neo.VM.Core
{
    public partial class VirtualTable
    {
        /// <summary>
        /// Flips all of the bits of an integer.
        /// <see cref="OpCode.INVERT"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 1, Push 1</remarks>
        public virtual void Invert(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var x = engine.CurrentContext!.Pop().GetInteger();

            engine.CurrentContext!.Push(~x);
        }

        /// <summary>
        /// Computes the bitwise AND of two integers.
        /// <see cref="OpCode.AND"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 2, Push 1</remarks>
        public virtual void And(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var x2 = engine.CurrentContext!.Pop().GetInteger();
            var x1 = engine.CurrentContext!.Pop().GetInteger();

            engine.CurrentContext!.Push(x1 & x2);
        }

        /// <summary>
        /// Computes the bitwise OR of two integers.
        /// <see cref="OpCode.OR"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 2, Push 1</remarks>
        public virtual void Or(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var x2 = engine.CurrentContext!.Pop().GetInteger();
            var x1 = engine.CurrentContext!.Pop().GetInteger();

            engine.CurrentContext!.Push(x1 | x2);
        }

        /// <summary>
        /// Computes the bitwise XOR (exclusive OR) of two integers.
        /// <see cref="OpCode.XOR"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 2, Push 1</remarks>
        public virtual void XOr(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var x2 = engine.CurrentContext!.Pop().GetInteger();
            var x1 = engine.CurrentContext!.Pop().GetInteger();

            engine.CurrentContext!.Push(x1 ^ x2);
        }

        /// <summary>
        /// Determines whether two objects are equal according to the execution engine's comparison rules.
        /// <see cref="OpCode.EQUAL"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 2, Push 1</remarks>
        public virtual void Equal(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var x2 = engine.CurrentContext!.Pop();
            var x1 = engine.CurrentContext!.Pop();

            engine.CurrentContext!.Push(x1.Equals(x2));
        }

        /// <summary>
        /// Determines whether two objects are not equal according to the execution engine's comparison rules.
        /// <see cref="OpCode.NOTEQUAL"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 2, Push 1</remarks>
        public virtual void NotEqual(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var x2 = engine.CurrentContext!.Pop();
            var x1 = engine.CurrentContext!.Pop();

            engine.CurrentContext!.Push(!x1.Equals(x2));
        }
    }
}
