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

using Neo.VM.Types;
using System;
using System.Collections.Generic;

namespace Neo.VM.Core
{
    public partial class VirtualTable
    {
        /// <summary>
        /// Initializes the static field slot in the current execution context.
        /// <see cref="OpCode.INITSSLOT"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void InitSSlot(NeoVirtualMachine engine, VMInstruction instruction)
        {
            if (engine.CurrentContext!.Frame.StaticFields.Count != 0)
                throw new InvalidOperationException($"{instruction.OpCode} cannot be executed twice.");

            var token = instruction.AsToken<byte>();
            if (token == 0)
                throw new InvalidOperationException($"The operand {token} is invalid for OpCode.{instruction.OpCode}.");

            engine.CurrentContext!.Frame.SetStaticField(token, VMNull.Instance);
        }

        /// <summary>
        /// Initializes the local variable slot or the argument slot in the current execution context.
        /// <see cref="OpCode.INITSLOT"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void InitSlot(NeoVirtualMachine engine, VMInstruction instruction)
        {
            if (engine.CurrentContext!.Frame.Locals.Count != 0 || engine.CurrentContext!.Frame.Arguments.Count != 0)
                throw new InvalidOperationException($"{instruction.OpCode} cannot be executed twice.");

            var token0 = instruction.AsToken<byte>();
            if (token0 == 0)
                throw new InvalidOperationException($"The operand {token0} is invalid for OpCode.{instruction.OpCode}.");

            if (token0 > 0)
                engine.CurrentContext!.Frame.SetLocal(token0, VMNull.Instance);

            var token1 = instruction.AsToken<byte>(1);
            if (token1 > 0)
            {
                var items = new VMObject[token1];

                for (var i = 0; i < token1; i++)
                    items[i] = engine.CurrentContext!.Pop();

                engine.CurrentContext!.Frame.Arguments.AddRange(items);
            }
        }

        /// <summary>
        /// Loads the value at index 0 from the static field slot onto the evaluation stack.
        /// <see cref="OpCode.LDSFLD0"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void LdSFld0(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteLoadFromSlot(engine, engine.CurrentContext!.Frame.StaticFields, 0);
        }

        /// <summary>
        /// Loads the value at index 1 from the static field slot onto the evaluation stack.
        /// <see cref="OpCode.LDSFLD1"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void LdSFld1(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteLoadFromSlot(engine, engine.CurrentContext!.Frame.StaticFields, 1);
        }

        /// <summary>
        /// Loads the value at index 2 from the static field slot onto the evaluation stack.
        /// <see cref="OpCode.LDSFLD2"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void LdSFld2(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteLoadFromSlot(engine, engine.CurrentContext!.Frame.StaticFields, 2);
        }

        /// <summary>
        /// Loads the value at index 3 from the static field slot onto the evaluation stack.
        /// <see cref="OpCode.LDSFLD3"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void LdSFld3(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteLoadFromSlot(engine, engine.CurrentContext!.Frame.StaticFields, 3);
        }

        /// <summary>
        /// Loads the value at index 4 from the static field slot onto the evaluation stack.
        /// <see cref="OpCode.LDSFLD4"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void LdSFld4(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteLoadFromSlot(engine, engine.CurrentContext!.Frame.StaticFields, 4);
        }

        /// <summary>
        /// Loads the value at index 5 from the static field slot onto the evaluation stack.
        /// <see cref="OpCode.LDSFLD5"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void LdSFld5(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteLoadFromSlot(engine, engine.CurrentContext!.Frame.StaticFields, 5);
        }

        /// <summary>
        /// Loads the value at index 6 from the static field slot onto the evaluation stack.
        /// <see cref="OpCode.LDSFLD6"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void LdSFld6(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteLoadFromSlot(engine, engine.CurrentContext!.Frame.StaticFields, 6);
        }

        /// <summary>
        /// Loads the static field at a specified index onto the evaluation stack.
        /// The index is represented as a 1-byte unsigned integer.
        /// <see cref="OpCode.LDSFLD"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void LdSFld(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteLoadFromSlot(engine, engine.CurrentContext!.Frame.StaticFields, instruction.AsToken<byte>());
        }

        /// <summary>
        /// Stores the value at index 0 from the evaluation stack into the static field slot.
        /// <see cref="OpCode.STSFLD0"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void StSFld0(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteStoreToSlot(engine, engine.CurrentContext!.Frame.StaticFields, 0);
        }

        /// <summary>
        /// Stores the value at index 1 from the evaluation stack into the static field slot.
        /// <see cref="OpCode.STSFLD1"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void StSFld1(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteStoreToSlot(engine, engine.CurrentContext!.Frame.StaticFields, 1);
        }

        /// <summary>
        /// Stores the value at index 2 from the evaluation stack into the static field slot.
        /// <see cref="OpCode.STSFLD2"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void StSFld2(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteStoreToSlot(engine, engine.CurrentContext!.Frame.StaticFields, 2);
        }

        /// <summary>
        /// Stores the value at index 3 from the evaluation stack into the static field slot.
        /// <see cref="OpCode.STSFLD3"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void StSFld3(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteStoreToSlot(engine, engine.CurrentContext!.Frame.StaticFields, 3);
        }

        /// <summary>
        /// Stores the value at index 4 from the evaluation stack into the static field slot.
        /// <see cref="OpCode.STSFLD4"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void StSFld4(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteStoreToSlot(engine, engine.CurrentContext!.Frame.StaticFields, 4);
        }

        /// <summary>
        /// Stores the value at index 5 from the evaluation stack into the static field slot.
        /// <see cref="OpCode.STSFLD5"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void StSFld5(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteStoreToSlot(engine, engine.CurrentContext!.Frame.StaticFields, 5);
        }

        /// <summary>
        /// Stores the value at index 6 from the evaluation stack into the static field slot.
        /// <see cref="OpCode.STSFLD6"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void StSFld6(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteStoreToSlot(engine, engine.CurrentContext!.Frame.StaticFields, 6);
        }

        /// <summary>
        /// Stores the value on top of the evaluation stack in the static field list at a specified index.
        /// The index is represented as a 1-byte unsigned integer.
        /// <see cref="OpCode.STSFLD"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void StSFld(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteStoreToSlot(engine, engine.CurrentContext!.Frame.StaticFields, instruction.AsToken<byte>());
        }

        /// <summary>
        /// Loads the local variable at index 0 onto the evaluation stack.
        /// <see cref="OpCode.LDLOC0"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void LdLoc0(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteLoadFromSlot(engine, engine.CurrentContext!.Frame.Locals, 0);
        }

        /// <summary>
        /// Loads the local variable at index 1 onto the evaluation stack.
        /// <see cref="OpCode.LDLOC1"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void LdLoc1(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteLoadFromSlot(engine, engine.CurrentContext!.Frame.Locals, 1);
        }

        /// <summary>
        /// Loads the local variable at index 2 onto the evaluation stack.
        /// <see cref="OpCode.LDLOC2"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void LdLoc2(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteLoadFromSlot(engine, engine.CurrentContext!.Frame.Locals, 2);
        }

        /// <summary>
        /// Loads the local variable at index 3 onto the evaluation stack.
        /// <see cref="OpCode.LDLOC3"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void LdLoc3(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteLoadFromSlot(engine, engine.CurrentContext!.Frame.Locals, 3);
        }

        /// <summary>
        /// Loads the local variable at index 4 onto the evaluation stack.
        /// <see cref="OpCode.LDLOC4"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void LdLoc4(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteLoadFromSlot(engine, engine.CurrentContext!.Frame.Locals, 4);
        }

        /// <summary>
        /// Loads the local variable at index 5 onto the evaluation stack.
        /// <see cref="OpCode.LDLOC5"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void LdLoc5(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteLoadFromSlot(engine, engine.CurrentContext!.Frame.Locals, 5);
        }

        /// <summary>
        /// Loads the local variable at index 6 onto the evaluation stack.
        /// <see cref="OpCode.LDLOC6"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void LdLoc6(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteLoadFromSlot(engine, engine.CurrentContext!.Frame.Locals, 6);
        }

        /// <summary>
        /// Loads the local variable at a specified index onto the evaluation stack.
        /// The index is represented as a 1-byte unsigned integer.
        /// <see cref="OpCode.LDLOC"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void LdLoc(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteLoadFromSlot(engine, engine.CurrentContext!.Frame.Locals, instruction.AsToken<byte>());
        }

        /// <summary>
        /// Stores the value on top of the evaluation stack in the local variable list at index 0.
        /// <see cref="OpCode.STLOC0"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void StLoc0(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteStoreToSlot(engine, engine.CurrentContext!.Frame.Locals, 0);
        }

        /// <summary>
        /// Stores the value on top of the evaluation stack in the local variable list at index 1.
        /// <see cref="OpCode.STLOC1"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void StLoc1(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteStoreToSlot(engine, engine.CurrentContext!.Frame.Locals, 1);
        }

        /// <summary>
        /// Stores the value on top of the evaluation stack in the local variable list at index 2.
        /// <see cref="OpCode.STLOC2"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void StLoc2(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteStoreToSlot(engine, engine.CurrentContext!.Frame.Locals, 2);
        }

        /// <summary>
        /// Stores the value on top of the evaluation stack in the local variable list at index 3.
        /// <see cref="OpCode.STLOC3"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void StLoc3(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteStoreToSlot(engine, engine.CurrentContext!.Frame.Locals, 3);
        }

        /// <summary>
        /// Stores the value on top of the evaluation stack in the local variable list at index 4.
        /// <see cref="OpCode.STLOC4"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void StLoc4(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteStoreToSlot(engine, engine.CurrentContext!.Frame.Locals, 4);
        }

        /// <summary>
        /// Stores the value on top of the evaluation stack in the local variable list at index 5.
        /// <see cref="OpCode.STLOC5"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void StLoc5(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteStoreToSlot(engine, engine.CurrentContext!.Frame.Locals, 5);
        }

        /// <summary>
        /// Stores the value on top of the evaluation stack in the local variable list at index 6.
        /// <see cref="OpCode.STLOC6"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void StLoc6(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteStoreToSlot(engine, engine.CurrentContext!.Frame.Locals, 6);
        }

        /// <summary>
        /// Stores the value on top of the evaluation stack in the local variable list at a specified index.
        /// The index is represented as a 1-byte unsigned integer.
        /// <see cref="OpCode.STLOC"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void StLoc(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteStoreToSlot(engine, engine.CurrentContext!.Frame.Locals, instruction.AsToken<byte>());
        }

        /// <summary>
        /// Loads the argument at index 0 onto the evaluation stack.
        /// <see cref="OpCode.LDARG0"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void LdArg0(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteLoadFromSlot(engine, engine.CurrentContext!.Frame.Arguments, 0);
        }

        /// <summary>
        /// Loads the argument at index 1 onto the evaluation stack.
        /// <see cref="OpCode.LDARG1"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void LdArg1(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteLoadFromSlot(engine, engine.CurrentContext!.Frame.Arguments, 1);
        }

        /// <summary>
        /// Loads the argument at index 2 onto the evaluation stack.
        /// <see cref="OpCode.LDARG2"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void LdArg2(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteLoadFromSlot(engine, engine.CurrentContext!.Frame.Arguments, 2);
        }

        /// <summary>
        /// Loads the argument at index 3 onto the evaluation stack.
        /// <see cref="OpCode.LDARG3"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void LdArg3(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteLoadFromSlot(engine, engine.CurrentContext!.Frame.Arguments, 3);
        }

        /// <summary>
        /// Loads the argument at index 4 onto the evaluation stack.
        /// <see cref="OpCode.LDARG4"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void LdArg4(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteLoadFromSlot(engine, engine.CurrentContext!.Frame.Arguments, 4);
        }

        /// <summary>
        /// Loads the argument at index 5 onto the evaluation stack.
        /// <see cref="OpCode.LDARG5"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void LdArg5(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteLoadFromSlot(engine, engine.CurrentContext!.Frame.Arguments, 5);
        }

        /// <summary>
        /// Loads the argument at index 6 onto the evaluation stack.
        /// <see cref="OpCode.LDARG6"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void LdArg6(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteLoadFromSlot(engine, engine.CurrentContext!.Frame.Arguments, 6);
        }

        /// <summary>
        /// Loads the argument at a specified index onto the evaluation stack.
        /// The index is represented as a 1-byte unsigned integer.
        /// <see cref="OpCode.LDARG"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void LdArg(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteLoadFromSlot(engine, engine.CurrentContext!.Frame.Arguments, instruction.AsToken<byte>());
        }

        /// <summary>
        /// Stores the value on top of the evaluation stack in the argument slot at index 0.
        /// <see cref="OpCode.STARG0"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void StArg0(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteStoreToSlot(engine, engine.CurrentContext!.Frame.Arguments, 0);
        }

        /// <summary>
        /// Stores the value on top of the evaluation stack in the argument slot at index 1.
        /// <see cref="OpCode.STARG1"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void StArg1(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteStoreToSlot(engine, engine.CurrentContext!.Frame.Arguments, 1);
        }

        /// <summary>
        /// Stores the value on top of the evaluation stack in the argument slot at index 2.
        /// <see cref="OpCode.STARG2"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void StArg2(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteStoreToSlot(engine, engine.CurrentContext!.Frame.Arguments, 2);
        }

        /// <summary>
        /// Stores the value on top of the evaluation stack in the argument slot at index 3.
        /// <see cref="OpCode.STARG3"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void StArg3(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteStoreToSlot(engine, engine.CurrentContext!.Frame.Arguments, 3);
        }

        /// <summary>
        /// Stores the value on top of the evaluation stack in the argument slot at index 4.
        /// <see cref="OpCode.STARG4"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void StArg4(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteStoreToSlot(engine, engine.CurrentContext!.Frame.Arguments, 4);
        }

        /// <summary>
        /// Stores the value on top of the evaluation stack in the argument slot at index 5.
        /// <see cref="OpCode.STARG5"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void StArg5(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteStoreToSlot(engine, engine.CurrentContext!.Frame.Arguments, 5);
        }

        /// <summary>
        /// Stores the value on top of the evaluation stack in the argument slot at index 6.
        /// <see cref="OpCode.STARG6"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void StArg6(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteStoreToSlot(engine, engine.CurrentContext!.Frame.Arguments, 6);
        }

        /// <summary>
        /// Stores the value on top of the evaluation stack in the argument slot at a specified index.
        /// The index is represented as a 1-byte unsigned integer.
        /// <see cref="OpCode.STARG"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        public virtual void StArg(NeoVirtualMachine engine, VMInstruction instruction)
        {
            ExecuteStoreToSlot(engine, engine.CurrentContext!.Frame.Arguments, instruction.AsToken<byte>());
        }

        #region Execute methods

        /// <summary>
        /// Executes the store operation into the specified slot at the given index.
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="slot">The slot to store the value.</param>
        /// <param name="index">The index within the slot.</param>
        public virtual void ExecuteStoreToSlot(NeoVirtualMachine engine, IList<VMObject> slot, int index)
        {
            if (slot is null || slot.Count == 0)
                throw new InvalidOperationException("Slot has not been initialized.");

            if (index < 0 || index >= slot.Count)
                throw new InvalidOperationException($"Index out of range when storing to slot: {index}, {index}/[0, {slot.Count}).");

            slot[index] = engine.CurrentContext!.Pop();
        }

        /// <summary>
        /// Executes the load operation from the specified slot at the given index onto the evaluation stack.
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="slot">The slot to load the value from.</param>
        /// <param name="index">The index within the slot.</param>
        public virtual void ExecuteLoadFromSlot(NeoVirtualMachine engine, IList<VMObject> slot, int index)
        {
            if (slot is null || slot.Count == 0)
                throw new InvalidOperationException("Slot has not been initialized.");

            if (index < 0 || index >= slot.Count)
                throw new InvalidOperationException($"Index out of range when loading from slot: {index}, {index}/[0, {slot.Count}).");

            engine.CurrentContext!.Push(slot[index]);
        }

        #endregion
    }
}
