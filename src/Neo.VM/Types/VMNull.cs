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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Neo.VM.Types
{
    public class VMNull : VMObject, IEquatable<VMNull>
    {
        public override VMObjectType Type => VMObjectType.Any;

        public static VMNull Instance => s_instance;


        private static readonly VMNull s_instance = new();

        protected override void Dispose(bool disposing)
        {
            // Never dispose the singleton instance
            base.Dispose(disposing);
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (ReferenceEquals(obj, this)) return true;
            if (obj is null) return true;
            return Equals(obj as VMNull);
        }

        public bool Equals([NotNullWhen(true)] VMNull? other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override VMObject Clone()
        {
            return this;
        }

        public override bool GetBoolean()
        {
            return false;
        }

        public override BigInteger GetInteger()
        {
            return BigInteger.Zero;
        }

        public override string ToString()
        {
            return "\0";
        }

        public static bool operator ==(VMNull left, VMNull right) =>
            true;

        public static bool operator !=(VMNull left, VMNull right) =>
            false;

    }
}
