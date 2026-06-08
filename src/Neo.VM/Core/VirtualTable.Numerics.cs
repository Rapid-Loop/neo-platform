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

using Neo.Numerics.Extensions;
using Neo.VM.Types;
using System.Numerics;

namespace Neo.VM.Core
{
    public partial class VirtualTable
    {
        /// <summary>
        /// Computes the sign of the specified integer.
        /// If the value is negative, puts -1; if positive, puts 1; if zero, puts 0.
        /// <see cref="OpCode.SIGN"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 1, Push 1</remarks>
        public virtual void Sign(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var x = engine.CurrentContext!.Pop().GetInteger();

            engine.CurrentContext!.Push(x.Sign);
        }

        /// <summary>
        /// Computes the absolute value of the specified integer.
        /// <see cref="OpCode.ABS"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 1, Push 1</remarks>
        public virtual void Abs(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var x = engine.CurrentContext!.Pop().GetInteger();

            engine.CurrentContext!.Push(BigInteger.Abs(x));
        }

        /// <summary>
        /// Computes the negation of the specified integer.
        /// <see cref="OpCode.NEGATE"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 1, Push 1</remarks>
        public virtual void Negate(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var x = engine.CurrentContext!.Pop().GetInteger();

            engine.CurrentContext!.Push(-x);
        }

        /// <summary>
        /// Increments the specified integer by one.
        /// <see cref="OpCode.INC"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 1, Push 1</remarks>
        public virtual void Inc(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var x = engine.CurrentContext!.Pop().GetInteger();

            engine.CurrentContext!.Push(x + 1);
        }

        /// <summary>
        /// Decrements the specified integer by one.
        /// <see cref="OpCode.DEC"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 1, Push 1</remarks>
        public virtual void Dec(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var x = engine.CurrentContext!.Pop().GetInteger();

            engine.CurrentContext!.Push(x - 1);
        }

        /// <summary>
        /// Computes the sum of two integers.
        /// <see cref="OpCode.ADD"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 2, Push 1</remarks>
        public virtual void Add(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var x2 = engine.CurrentContext!.Pop().GetInteger();
            var x1 = engine.CurrentContext!.Pop().GetInteger();

            engine.CurrentContext!.Push(x1 + x2);
        }

        /// <summary>
        /// Computes the difference between two integers.
        /// <see cref="OpCode.SUB"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 2, Push 1</remarks>
        public virtual void Sub(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var x2 = engine.CurrentContext!.Pop().GetInteger();
            var x1 = engine.CurrentContext!.Pop().GetInteger();

            engine.CurrentContext!.Push(x1 - x2);
        }

        /// <summary>
        /// Computes the product of two integers.
        /// <see cref="OpCode.MUL"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 2, Push 1</remarks>
        public virtual void Mul(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var x2 = engine.CurrentContext!.Pop().GetInteger();
            var x1 = engine.CurrentContext!.Pop().GetInteger();

            engine.CurrentContext!.Push(x1 * x2);
        }

        /// <summary>
        /// Computes the quotient of two integers.
        /// <see cref="OpCode.DIV"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 2, Push 1</remarks>
        public virtual void Div(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var x2 = engine.CurrentContext!.Pop().GetInteger();
            var x1 = engine.CurrentContext!.Pop().GetInteger();

            engine.CurrentContext!.Push(x1 / x2);
        }

        /// <summary>
        /// Computes the remainder after dividing a by b.
        /// <see cref="OpCode.MOD"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 2, Push 1</remarks>
        public virtual void Mod(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var x2 = engine.CurrentContext!.Pop().GetInteger();
            var x1 = engine.CurrentContext!.Pop().GetInteger();

            engine.CurrentContext!.Push(x1 % x2);
        }

        /// <summary>
        /// Computes the result of raising a number to the specified power.
        /// <see cref="OpCode.POW"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 2, Push 1</remarks>
        public virtual void Pow(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var exponent = (int)engine.CurrentContext!.Pop().GetInteger();
            engine.Limits.AssertShift(exponent);

            var value = engine.CurrentContext!.Pop().GetInteger();
            engine.CurrentContext!.Push(BigInteger.Pow(value, exponent));
        }

        /// <summary>
        /// Returns the square root of a specified number.
        /// <see cref="OpCode.SQRT"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 1, Push 1</remarks>
        public virtual void Sqrt(NeoVirtualMachine engine, VMInstruction instruction)
        {
            engine.CurrentContext!.Push(engine.CurrentContext!.Pop().GetInteger().Sqrt());
        }

        /// <summary>
        /// Computes the modular multiplication of two integers.
        /// <see cref="OpCode.MODMUL"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 3, Push 1</remarks>
        public virtual void ModMul(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var modulus = engine.CurrentContext!.Pop().GetInteger();
            var x2 = engine.CurrentContext!.Pop().GetInteger();
            var x1 = engine.CurrentContext!.Pop().GetInteger();

            engine.CurrentContext!.Push(x1 * x2 % modulus);
        }

        /// <summary>
        /// Computes the modular exponentiation of an integer.
        /// <see cref="OpCode.MODPOW"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 3, Push 1</remarks>
        public virtual void ModPow(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var modulus = engine.CurrentContext!.Pop().GetInteger();
            var exponent = engine.CurrentContext!.Pop().GetInteger();
            var value = engine.CurrentContext!.Pop().GetInteger();

            var result = exponent == -1
                ? value.ModInverse(modulus)
                : BigInteger.ModPow(value, exponent, modulus);

            engine.CurrentContext!.Push(result);
        }

        /// <summary>
        /// Computes the left shift of an integer.
        /// <see cref="OpCode.SHL"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 2, Push 1</remarks>
        public virtual void Shl(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var shift = (int)engine.CurrentContext!.Pop().GetInteger();
            engine.Limits.AssertShift(shift);

            var x = engine.CurrentContext!.Pop().GetInteger();
            engine.CurrentContext!.Push(x << shift);
        }

        /// <summary>
        /// Computes the right shift of an integer.
        /// <see cref="OpCode.SHR"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 2, Push 1</remarks>
        public virtual void Shr(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var shift = (int)engine.CurrentContext!.Pop().GetInteger();
            engine.Limits.AssertShift(shift);

            var x = engine.CurrentContext!.Pop().GetInteger();
            engine.CurrentContext!.Push(x >> shift);
        }

        /// <summary>
        /// If the input is 0 or 1, it is flipped. Otherwise the output will be 0.
        /// <see cref="OpCode.NOT"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 1, Push 1</remarks>
        public virtual void Not(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var x = engine.CurrentContext!.Pop().GetBoolean();

            engine.CurrentContext!.Push(!x);
        }

        /// <summary>
        /// Computes the logical AND of the top two stack items and pushes the result onto the stack.
        /// <see cref="OpCode.BOOLAND"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 2, Push 1</remarks>
        public virtual void BoolAnd(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var x2 = engine.CurrentContext!.Pop().GetBoolean();
            var x1 = engine.CurrentContext!.Pop().GetBoolean();

            engine.CurrentContext!.Push(x1 && x2);
        }

        /// <summary>
        /// Computes the logical OR of the top two stack items and pushes the result onto the stack.
        /// <see cref="OpCode.BOOLOR"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 2, Push 1</remarks>
        public virtual void BoolOr(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var x2 = engine.CurrentContext!.Pop().GetBoolean();
            var x1 = engine.CurrentContext!.Pop().GetBoolean();

            engine.CurrentContext!.Push(x1 || x2);
        }

        /// <summary>
        /// Determines whether the top stack item is not zero and pushes the result onto the stack.
        /// <see cref="OpCode.NZ"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 1, Push 1</remarks>
        public virtual void Nz(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var x = engine.CurrentContext!.Pop().GetInteger();

            engine.CurrentContext!.Push(!x.IsZero);
        }

        /// <summary>
        /// Determines whether the top two stack items are equal and pushes the result onto the stack.
        /// <see cref="OpCode.NUMEQUAL"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 2, Push 1</remarks>
        public virtual void NumEqual(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var x2 = engine.CurrentContext!.Pop().GetInteger();
            var x1 = engine.CurrentContext!.Pop().GetInteger();

            engine.CurrentContext!.Push(x1 == x2);
        }

        /// <summary>
        /// Determines whether the top two stack items are not equal and pushes the result onto the stack.
        /// <see cref="OpCode.NUMNOTEQUAL"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 2, Push 1</remarks>
        public virtual void NumNotEqual(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var x2 = engine.CurrentContext!.Pop().GetInteger();
            var x1 = engine.CurrentContext!.Pop().GetInteger();

            engine.CurrentContext!.Push(x1 != x2);
        }

        /// <summary>
        /// Determines whether the two integer at the top of the stack, x1 are less than x2, and pushes the result onto the stack.
        /// <see cref="OpCode.LT"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 2, Push 1</remarks>
        public virtual void Lt(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var x2 = engine.CurrentContext!.Pop();
            var x1 = engine.CurrentContext!.Pop();

            if (x1 is VMNull || x2 is VMNull)
                engine.CurrentContext!.Push(false);
            else
                engine.CurrentContext!.Push(x1.GetInteger() < x2.GetInteger());
        }

        /// <summary>
        /// Determines whether the two integer at the top of the stack, x1 are less than or equal to x2, and pushes the result onto the stack.
        /// <see cref="OpCode.LE"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 2, Push 1</remarks>
        public virtual void Le(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var x2 = engine.CurrentContext!.Pop();
            var x1 = engine.CurrentContext!.Pop();

            if (x1 is VMNull || x2 is VMNull)
                engine.CurrentContext!.Push(false);
            else
                engine.CurrentContext!.Push(x1.GetInteger() <= x2.GetInteger());
        }

        /// <summary>
        /// Determines whether the two integer at the top of the stack, x1 are greater than x2, and pushes the result onto the stack.
        /// <see cref="OpCode.GT"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 2, Push 1</remarks>
        public virtual void Gt(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var x2 = engine.CurrentContext!.Pop();
            var x1 = engine.CurrentContext!.Pop();

            if (x1 is VMNull || x2 is VMNull)
                engine.CurrentContext!.Push(false);
            else
                engine.CurrentContext!.Push(x1.GetInteger() > x2.GetInteger());
        }

        /// <summary>
        /// Determines whether the two integer at the top of the stack, x1 are greater than or equal to x2, and pushes the result onto the stack.
        /// <see cref="OpCode.GE"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 2, Push 1</remarks>
        public virtual void Ge(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var x2 = engine.CurrentContext!.Pop();
            var x1 = engine.CurrentContext!.Pop();

            if (x1 is VMNull || x2 is VMNull)
                engine.CurrentContext!.Push(false);
            else
                engine.CurrentContext!.Push(x1.GetInteger() >= x2.GetInteger());
        }

        /// <summary>
        /// Computes the minimum of the top two stack items and pushes the result onto the stack.
        /// <see cref="OpCode.MIN"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 2, Push 1</remarks>
        public virtual void Min(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var x2 = engine.CurrentContext!.Pop().GetInteger();
            var x1 = engine.CurrentContext!.Pop().GetInteger();

            engine.CurrentContext!.Push(BigInteger.Min(x1, x2));
        }

        /// <summary>
        /// Computes the maximum of the top two stack items and pushes the result onto the stack.
        /// <see cref="OpCode.MAX"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 2, Push 1</remarks>
        public virtual void Max(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var x2 = engine.CurrentContext!.Pop().GetInteger();
            var x1 = engine.CurrentContext!.Pop().GetInteger();

            engine.CurrentContext!.Push(BigInteger.Max(x1, x2));
        }

        /// <summary>
        /// Determines whether the top stack item is within the range specified by the next two top stack items
        /// and pushes the result onto the stack.
        /// <see cref="OpCode.WITHIN"/>
        /// </summary>
        /// <param name="engine">The execution engine.CurrentContext!.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 3, Push 1</remarks>
        public virtual void Within(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var b = engine.CurrentContext!.Pop().GetInteger();
            var a = engine.CurrentContext!.Pop().GetInteger();
            var x = engine.CurrentContext!.Pop().GetInteger();

            engine.CurrentContext!.Push(a <= x && x < b);
        }
    }
}
