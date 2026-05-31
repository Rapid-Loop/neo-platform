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

using System.Numerics;

namespace Neo.VM.Types
{
    public class VMBoolean(bool value) : VMObject(new byte[] { value ? (byte)1 : (byte)0 })
    {
        public new VMObjectType Type => VMObjectType.Boolean;

        public override string ToString()
        {
            return value.ToString();
        }

        public static implicit operator ReadOnlyMemory<byte>(VMBoolean value)
        {
            return value.ValueMemory;
        }

        public static implicit operator ReadOnlySpan<byte>(VMBoolean value)
        {
            return value.ValueMemory.Span;
        }

        public static implicit operator bool(VMBoolean value)
        {
            return value.ValueMemory.Span[0] == 1 ? true : false;
        }

        public static implicit operator VMBoolean(bool value)
        {
            return value ? new(true) : new(false);
        }

        public static implicit operator BigInteger(VMBoolean value)
        {
            return value.ValueMemory.Span[0] == 1 ? BigInteger.One : BigInteger.Zero;
        }
    }
}
