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

using Neo.Core;
using Neo.Core.VM;
using Neo.Core.VM.Attributes;
using Neo.Core.VM.SmartContract;
using Neo.Core.VM.Specs;
using Neo.VM.Types;
using System;
using System.Numerics;

namespace Neo.VM.Tests
{
    [TestClass]
    public sealed class UT_NeoVirtualMachine
    {
        private class InternalRuntime
        {
            [MethodDescriptor(
                Safe = true,
                Fork = HardFork.Genesis,
                ExecutePrice = 1_00000000L,
                CallFlags = CallFlags.None,
                ReturnType = MethodParameterType.String
            )]
            public static string SayHello(string name)
            {
                return $"Hello, {name}";
            }
        }

        [TestMethod]
        public void TestSystemCall()
        {
            var expectedParamValue = "NEO";
            var expectedSystemName = "System.Runtime.SayHello";
            var expectedSystemCallAddress = MethodDescriptor.CreateCallAddress(expectedSystemName);
            var expectedTargetName = nameof(InternalRuntime.SayHello);
            var expectedTargetMethod = InternalRuntime.SayHello;
            var expectedTargetReturnValue = InternalRuntime.SayHello(expectedParamValue);
            var expectedGasPrice = 1_00000000L + GasTable.GetGasCost(OpCode.PUSHDATA1, HardFork.Gorgon);

            var vm = new VirtualMachineEngine(loggerFactory: TestUtilities.TraceLoggerFactory);

            var actualMethodDesc = vm.RegisterSystemCall<InternalRuntime>(
                expectedSystemName,
                nameof(InternalRuntime.SayHello)
            );

            var actualTargetReturnValue = actualMethodDesc.TargetMethod.DynamicInvoke(expectedParamValue);
            var actualFoundSystemCall = vm.SystemCallTable.TryGetValue(expectedSystemName, out var actualSystemCallAddress);

            using var sb = new ScriptBuilder()
            .EmitSysCall(actualMethodDesc, expectedParamValue)
            .EmitReturn();

            vm.LoadScript(sb.ToArray());

            var actualState = vm.Execute();
            var actualGasPrice = vm.GasConsumed;
            var actualResults = vm.ResultStack;

            Assert.AreEqual(VMState.HALT, actualState);
            Assert.HasCount(1, actualResults);

            Assert.IsNotNull(actualTargetReturnValue);
            Assert.IsTrue(actualFoundSystemCall);

            Assert.AreEqual(expectedGasPrice, actualGasPrice);
            Assert.AreEqual(expectedSystemCallAddress, actualSystemCallAddress);
            Assert.AreEqual(expectedSystemCallAddress, actualMethodDesc);
            Assert.AreEqual(actualSystemCallAddress, actualMethodDesc);
            Assert.AreEqual(expectedSystemName, actualMethodDesc.SystemMethodName);
            Assert.AreEqual(expectedTargetName, actualMethodDesc.TargetMethodInfo.Name);
            Assert.AreEqual(expectedTargetMethod, actualMethodDesc.TargetMethod);
            Assert.AreEqual(expectedTargetReturnValue, actualTargetReturnValue);
            Assert.AreEqual(expectedTargetReturnValue, actualResults.Pop().ToString());
        }

        [TestMethod]
        public void TestVMSimple()
        {
            byte[] script =
            [
                0x15,        // PUSH 5
                0x16,        // PUSH 6
                0x9e,        // ADD
                0x40,        // RET
            ];

            var vm = new VirtualMachineEngine(loggerFactory: TestUtilities.TraceLoggerFactory);
            vm.LoadScript(script);

            var actualState = vm.Execute();
            var actualResults = vm.ResultStack;

            Assert.AreEqual(VMState.HALT, actualState);
            Assert.HasCount(1, actualResults);
            Assert.AreEqual(11, actualResults.Pop().GetInteger());
        }

        [TestMethod]
        public void TestVMPush()
        {
            using var sb = new ScriptBuilder()
                .EmitPush(null as object)
                .EmitPush([])
                .EmitPush(true)
                .EmitPush(false)
                .EmitPush(-1)
                .EmitPush(0)
                .EmitPush(10)
                .EmitPush(sbyte.MaxValue)
                .EmitPush(short.MaxValue)
                .EmitPush(int.MaxValue)
                .EmitPush(long.MaxValue)
                .EmitPush(BigInteger.Pow(long.MaxValue, 2))
                .EmitPush("NEO")
                .EmitPush(VMObjectType.Any)
                .EmitReturn();

            var script = sb.ToArray();

            var vm = new VirtualMachineEngine(loggerFactory: TestUtilities.TraceLoggerFactory);
            vm.LoadScript(script);

            var actualState = vm.Execute();
            var actualResults = vm.ResultStack;

            Assert.AreEqual(VMState.HALT, actualState);
            Assert.HasCount(14, actualResults);

            Assert.IsInstanceOfType<VMNull>(actualResults.Pop());
            Assert.IsTrue(actualResults.Pop().GetReadOnlySpan().IsEmpty);
            Assert.IsTrue(actualResults.Pop().GetBoolean());
            Assert.IsFalse(actualResults.Pop().GetBoolean());
            Assert.AreEqual(-1, actualResults.Pop().GetInteger());
            Assert.AreEqual(0, actualResults.Pop().GetInteger());
            Assert.AreEqual(10, actualResults.Pop().GetInteger());
            Assert.AreEqual(sbyte.MaxValue, actualResults.Pop().GetInteger());
            Assert.AreEqual(short.MaxValue, actualResults.Pop().GetInteger());
            Assert.AreEqual(int.MaxValue, actualResults.Pop().GetInteger());
            Assert.AreEqual(long.MaxValue, actualResults.Pop().GetInteger());
            Assert.AreEqual(BigInteger.Pow(long.MaxValue, 2), actualResults.Pop().GetInteger());
            Assert.IsTrue(new ReadOnlySpan<byte>(CoreUtilities.StrictUtf8Encoding.GetBytes("NEO")).SequenceEqual(actualResults.Pop().GetReadOnlySpan()));
            Assert.AreEqual(0, actualResults.Pop().GetInteger());
        }
    }
}
