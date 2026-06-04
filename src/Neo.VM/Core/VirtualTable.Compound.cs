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
using System.Linq;
using System.Numerics;

namespace Neo.VM.Core
{
    public partial class JumpTable
    {
        /// <summary>
        /// Packs a map from the evaluation stack.
        /// <see cref="OpCode.PACKMAP"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 2n+1, Push 1</remarks>
        public virtual void PackMap(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var size = unchecked((int)engine.CurrentContext!.Pop().GetInteger());
            if (size < 0 || size * 2 > engine.CurrentContext!.Frame.EvaluationStack.Count)
                throw new InvalidOperationException($"The map size is out of valid range, 2*{size}/[0, {engine.CurrentContext!.Frame.EvaluationStack.Count}].");

            var map = new VMMap();
            for (var i = 0; i < size; i++)
            {
                var key = engine.CurrentContext!.Pop();
                var value = engine.CurrentContext!.Pop();

                map[key] = value;
            }

            engine.CurrentContext!.Push(map);
        }

        /// <summary>
        /// Packs a struct from the evaluation stack.
        /// <see cref="OpCode.PACKSTRUCT"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop n+1, Push 1</remarks>
        public virtual void PackStruct(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var size = unchecked((int)engine.CurrentContext!.Pop().GetInteger());
            if (size < 0 || size > engine.CurrentContext!.Frame.EvaluationStack.Count)
                throw new InvalidOperationException($"The struct size is out of valid range, {size}/[0, {engine.CurrentContext!.Frame.EvaluationStack.Count}].");

            var obj = new VMStruct();
            for (var i = 0; i < size; i++)
            {
                var item = engine.CurrentContext!.Pop();

                obj.Add(item);
            }

            engine.CurrentContext!.Push(obj);
        }

        /// <summary>
        /// Packs an array from the evaluation stack.
        /// <see cref="OpCode.PACK"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop n+1, Push 1</remarks>
        public virtual void Pack(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var size = unchecked((int)engine.CurrentContext!.Pop().GetInteger());
            if (size < 0 || size > engine.CurrentContext!.Frame.EvaluationStack.Count)
                throw new InvalidOperationException($"The array size is out of valid range, {size}/[0, {engine.CurrentContext!.Frame.EvaluationStack.Count}].");

            var array = new VMArray();
            for (var i = 0; i < size; i++)
            {
                var item = engine.CurrentContext!.Pop();

                array.Add(item);
            }

            engine.CurrentContext!.Push(array);
        }

        /// <summary>
        /// Unpacks a compound type from the evaluation stack.
        /// <see cref="OpCode.UNPACK"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 1, Push 2n+1 or n+1</remarks>
        public virtual void Unpack(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var obj = engine.CurrentContext!.Pop();
            switch (obj)
            {
                case VMMap map:
                    foreach (var (key, value) in map.Reverse())
                    {
                        engine.CurrentContext!.Push(value);
                        engine.CurrentContext!.Push(key);
                    }
                    engine.CurrentContext!.Push(map.Count);
                    break;
                case VMArray array:
                    for (var i = array.Count - 1; i >= 0; i--)
                        engine.CurrentContext!.Push(array[i]);
                    engine.CurrentContext!.Push(array.Count);
                    break;
                default:
                    throw new InvalidOperationException($"Invalid type for {instruction.OpCode}: {obj.Type}");
            }
        }

        /// <summary>
        /// Creates a new empty array with zero elements on the evaluation stack.
        /// <see cref="OpCode.NEWARRAY0"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>
        /// Pop 0, Push 1
        /// TODO: Change to NewNullArray method or add it?
        /// </remarks>
        public virtual void NewArray0(NeoVirtualMachine engine, VMInstruction instruction)
        {
            engine.CurrentContext!.Push(new VMArray());
        }

        /// <summary>
        /// Creates a new array with a specified number of elements on the evaluation stack.
        /// <see cref="OpCode.NEWARRAY"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 1, Push 1</remarks>
        public virtual void NewArray(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var n = unchecked((int)engine.CurrentContext!.Pop().GetInteger());
            if (n < 0 || n > engine.Limits.MaxStackSize)
                throw new InvalidOperationException($"The array size is out of valid range, {n}/[0, {engine.Limits.MaxStackSize}].");

            var nullArray = new VMObject[n];
            Array.Fill(nullArray, VMNull.Instance);

            engine.CurrentContext!.Push(new VMArray(nullArray));
        }

        /// <summary>
        /// Creates a new array with a specified number of elements and a specified type on the evaluation stack.
        /// <see cref="OpCode.NEWARRAY_T"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 1, Push 1</remarks>
        public virtual void NewArray_T(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var n = unchecked((int)engine.CurrentContext!.Pop().GetInteger());
            if (n < 0 || n > engine.Limits.MaxStackSize)
                throw new InvalidOperationException($"The array size is out of valid range, {n}/[0, {engine.Limits.MaxStackSize}].");

            var type = instruction.AsToken<VMObjectType>();
            if (!Enum.IsDefined(type))
                throw new InvalidOperationException($"Invalid type for {instruction.OpCode}: {type}");

            VMObject item = type switch
            {
                VMObjectType.Boolean => false,
                VMObjectType.Integer => BigInteger.Zero,
                VMObjectType.ByteString => Array.Empty<byte>(),
                _ => VMNull.Instance,
            };

            var itemArray = new VMObject[n];
            Array.Fill(itemArray, item);

            engine.CurrentContext!.Push(new VMArray(itemArray));
        }

        /// <summary>
        /// Creates a new empty struct with zero elements on the evaluation stack.
        /// <see cref="OpCode.NEWSTRUCT0"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 0, Push 1</remarks>
        public virtual void NewStruct0(NeoVirtualMachine engine, VMInstruction instruction)
        {
            engine.CurrentContext!.Push(new VMStruct());
        }

        /// <summary>
        /// Creates a new struct with a specified number of elements on the evaluation stack.
        /// <see cref="OpCode.NEWSTRUCT"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 1, Push 1</remarks>
        public virtual void NewStruct(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var n = unchecked((int)engine.CurrentContext!.Pop().GetInteger());
            if (n < 0 || n > engine.Limits.MaxStackSize)
                throw new InvalidOperationException($"The struct size is out of valid range, {n}/[0, {engine.Limits.MaxStackSize}].");

            var nullArray = new VMObject[n];
            Array.Fill(nullArray, VMNull.Instance);

            engine.CurrentContext!.Push(new VMStruct(nullArray));
        }

        /// <summary>
        /// Creates a new empty map on the evaluation stack.
        /// <see cref="OpCode.NEWMAP"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 0, Push 1</remarks>
        public virtual void NewVMMap(NeoVirtualMachine engine, VMInstruction instruction)
        {
            engine.CurrentContext!.Push(new VMMap());
        }

        /// <summary>
        /// Gets the size of the top item on the evaluation stack and pushes it onto the stack.
        /// <see cref="OpCode.SIZE"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 1, Push 1</remarks>
        public virtual void Size(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var x = engine.CurrentContext!.Pop();
            switch (x)
            {
                case IReadOnlyCollection<VMObject> array:
                    engine.CurrentContext!.Push(array.Count);
                    break;
                case IReadOnlyDictionary<VMObject, VMObject> map:
                    engine.CurrentContext!.Push(map.Count);
                    break;
                case VMBuffer buffer:
                    engine.CurrentContext!.Push(buffer.Length);
                    break;
                default:
                    engine.CurrentContext!.Push(x.Size);
                    break;
            }
        }

        /// <summary>
        /// Checks whether the top item on the evaluation stack has the specified key.
        /// <see cref="OpCode.HASKEY"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 2, Push 1</remarks>
        public virtual void HasKey(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var key = engine.CurrentContext!.Pop();
            var x = engine.CurrentContext!.Pop();

            switch (x)
            {
                case VMArray array:
                    var index1 = key.GetInteger();
                    if (index1 < 0 || index1 >= engine.Limits.MaxItemSize)
                        throw new InvalidOperationException($"The index {index1} is invalid for OpCode {instruction.OpCode}.");
                    engine.CurrentContext!.Push(index1 < array.Count);
                    break;
                case VMMap map:
                    engine.CurrentContext!.Push(map.ContainsKey(key));
                    break;
                case VMBuffer buffer:
                    var index2 = key.GetInteger();
                    if (index2 < 0 || index2 >= engine.Limits.MaxItemSize)
                        throw new InvalidOperationException($"The index {index2} is invalid for OpCode {instruction.OpCode}.");
                    engine.CurrentContext!.Push(index2 < buffer.Length);
                    break;
                case VMByteArray byteArray:
                    var index3 = key.GetInteger();
                    if (index3 < 0 || index3 >= engine.Limits.MaxItemSize)
                        throw new InvalidOperationException($"The index {index3} is invalid for OpCode {instruction.OpCode}.");
                    engine.CurrentContext!.Push(index3 < byteArray.Length);
                    break;
                default:
                    throw new InvalidOperationException($"Invalid type for {instruction.OpCode}: {x.Type}");
            }
        }

        /// <summary>
        /// Retrieves the keys of a map and pushes them onto the evaluation stack as an array.
        /// <see cref="OpCode.KEYS"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 1, Push 1</remarks>
        public virtual void Keys(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var map = (VMMap)engine.CurrentContext!.Pop();

            engine.CurrentContext!.Push(new VMArray(map.Keys));
        }

        /// <summary>
        /// Retrieves the values of a compound type and pushes them onto the evaluation stack as an array.
        /// <see cref="OpCode.VALUES"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 1, Push 1</remarks>
        public virtual void Values(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var x = engine.CurrentContext!.Pop();
            var values = x switch
            {
                VMArray array => array,
                VMMap map => map.Values,
                _ => throw new InvalidOperationException($"Invalid type for {instruction.OpCode}: {x.Type}"),
            };

            var newArray = new VMArray();
            foreach (var item in values)
                if (item is VMStruct s)
                    newArray.Add(s.Clone());
                else
                    newArray.Add(item);

            engine.CurrentContext!.Push(newArray);
        }

        /// <summary>
        /// Retrieves the item from an array, map, buffer, or byte string based on the specified key,
        /// and pushes it onto the evaluation stack.
        /// <see cref="OpCode.PICKITEM"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 2, Push 1</remarks>
        public virtual void PickItem(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var key = engine.CurrentContext!.Pop();
            var x = engine.CurrentContext!.Pop();

            switch (x)
            {
                case VMArray array:
                    var index1 = key.GetInteger();
                    if (index1 < BigInteger.Zero || index1 >= array.Count)
                        throw new Exception($"The index of {nameof(VMArray)} is out of range, {index1}/[0, {array.Count}).");
                    engine.CurrentContext!.Push(array[unchecked((int)index1)]);
                    break;
                case VMMap map:
                    if (!map.TryGetValue(key, out var value))
                        throw new Exception($"Key {key} not found in {nameof(VMMap)}.");
                    engine.CurrentContext!.Push(value);
                    break;
                case VMByteArray byteArray:
                    var a = byteArray.GetReadOnlySpan();
                    var index2 = key.GetInteger();
                    if (index2 < BigInteger.Zero || index2 >= a.Length)
                        throw new Exception($"The index of {nameof(VMByteArray)} is out of range, {index2}/[0, {a.Length}).");
                    engine.CurrentContext!.Push(new BigInteger(a[unchecked((int)index2)]));
                    break;
                case VMBuffer buffer:
                    var index3 = key.GetInteger();
                    if (index3 < BigInteger.Zero || index3 >= buffer.Length)
                        throw new Exception($"The index of {nameof(VMBuffer)} is out of range, {index3}/[0, {buffer.Length}).");
                    engine.CurrentContext!.Push((BigInteger)buffer.GetReadOnlySpan()[unchecked((int)index3)]);
                    break;
                default:
                    throw new InvalidOperationException($"Invalid type for {instruction.OpCode}: {x.Type}");
            }
        }

        /// <summary>
        /// Appends an item to the end of the specified array.
        /// <see cref="OpCode.APPEND"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 2, Push 0</remarks>
        public virtual void Append(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var newItem = engine.CurrentContext!.Pop();
            var array = (VMArray)engine.CurrentContext!.Pop();

            if (newItem is VMStruct s)
                newItem = s.Clone();

            array.Add(newItem);
        }

        /// <summary>
        /// A value v, index n (or key) and an array (or map) are taken from main stack. Attribution array[n]=v (or map[n]=v) is performed.
        /// <see cref="OpCode.SETITEM"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 3, Push 0</remarks>
        public virtual void SetItem(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var value = engine.CurrentContext!.Pop();
            if (value is VMStruct s)
                value = s.Clone();

            var key = engine.CurrentContext!.Pop();
            var x = engine.CurrentContext!.Pop();

            switch (x)
            {
                case VMArray array:
                    var index1 = key.GetInteger();
                    if (index1 < BigInteger.Zero || index1 >= array.Count)
                        throw new Exception($"The index of {nameof(VMArray)} is out of range, {index1}/[0, {array.Count}).");
                    var i = unchecked((int)index1);
                    array[i] = value;
                    break;
                case VMMap map:
                    map[key] = value;
                    break;
                case VMBuffer buffer:
                    var index2 = key.GetInteger();
                    if (index2 < BigInteger.Zero || index2 >= buffer.Length)
                        throw new Exception($"The index of {nameof(Buffer)} is out of range, {index2}/[0, {buffer.Size}).");
                    if (value is not VMBuffer p)
                        throw new InvalidOperationException($"Only primitive type values can be set in {nameof(VMBuffer)} in {instruction.OpCode}.");
                    var b = p.GetInteger();
                    if (b < sbyte.MinValue || b > byte.MaxValue)
                        throw new InvalidOperationException($"Overflow in {instruction.OpCode}, {b} is not a byte type.");
                    buffer[unchecked((int)index2)] = (byte)b;
                    break;
                default:
                    throw new InvalidOperationException($"Invalid type for {instruction.OpCode}: {x.Type}");
            }
        }

        /// <summary>
        /// Reverses the order of items in the specified array or buffer.
        /// <see cref="OpCode.REVERSEITEMS"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 1, Push 0</remarks>
        public virtual void ReverseItems(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var x = engine.CurrentContext!.Pop();

            switch (x)
            {
                case VMArray array:
                    array.Reverse();
                    break;
                case VMBuffer buffer:
                    buffer.Reverse();
                    break;
                default:
                    throw new InvalidOperationException($"Invalid type for {instruction.OpCode}: {x.Type}");
            }
        }

        /// <summary>
        /// Removes the item at the specified index from the array or map.
        /// <see cref="OpCode.REMOVE"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 2, Push 0</remarks>
        public virtual void Remove(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var key = engine.CurrentContext!.Pop();
            var x = engine.CurrentContext!.Pop();

            switch (x)
            {
                case VMArray array:
                    var index1 = key.GetInteger();
                    if (index1 < 0 || index1 >= array.Count)
                        throw new InvalidOperationException($"The index of {nameof(VMArray)} is out of range, {index1}/[0, {array.Count}).");
                    var i = unchecked((int)index1);
                    var item = array[i];
                    array.RemoveAt(i);
                    break;
                case VMMap map:
                    map.Remove(key);
                    break;
                default:
                    throw new InvalidOperationException($"Invalid type for {instruction.OpCode}: {x.Type}");
            }
        }

        /// <summary>
        /// Clears all items from the compound type.
        /// <see cref="OpCode.CLEARITEMS"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 1, Push 0</remarks>
        public virtual void ClearItems(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var x = engine.CurrentContext!.Pop();

            switch (x)
            {
                case VMArray array:
                    array.Clear();
                    break;
                case VMMap map:
                    map.Clear();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Removes and returns the item at the top of the specified array.
        /// <see cref="OpCode.POPITEM"/>
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <param name="instruction">The instruction being executed.</param>
        /// <remarks>Pop 1, Push 1</remarks>
        public virtual void PopItem(NeoVirtualMachine engine, VMInstruction instruction)
        {
            var x = (VMArray)engine.CurrentContext!.Pop();
            var index = x.Count - 1;
            var item = x[index];

            engine.CurrentContext!.Push(item);
            x.RemoveAt(index);
        }
    }
}
