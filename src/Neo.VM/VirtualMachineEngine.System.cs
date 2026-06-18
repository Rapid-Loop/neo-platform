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
using Neo.Core.VM.Attributes;
using Neo.Core.VM.Specs;
using Neo.VM.Extensions;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Neo.VM
{
    public partial class VirtualMachineEngine
    {
        private readonly Dictionary<uint, Dictionary<HardFork, MethodDescriptor>> _systemCallMethods = [];
        private readonly Dictionary<string, uint> _systemCallTable = [];

        internal IReadOnlyDictionary<string, uint> SystemCallTable => _systemCallTable;

        internal MethodDescriptor RegisterSystemCall<TSource>(string systemMethodName, string targetMethodName, TSource? targetInstance = default)
            where TSource : class?, new()
        {
            var hash = MethodDescriptor.CreateCallAddress(systemMethodName);

            if (_systemCallMethods.TryGetValue(hash, out var forks) &&
                forks.TryGetValue(_currentFork, out _))
                throw new InvalidOperationException($"Method {systemMethodName} is already registered.");

            var targetMethodDesc = new MethodDescriptor(
                targetInstance ?? new TSource(),
                targetMethodName,
                systemMethodName
            );

            if (_systemCallMethods.ContainsKey(targetMethodDesc) == false)
                _systemCallMethods[targetMethodDesc] = [];

            _systemCallMethods[targetMethodDesc].Add(_currentFork, targetMethodDesc);
            _systemCallTable[systemMethodName] = targetMethodDesc;

            return targetMethodDesc;
        }

        internal MethodDescriptorAttribute GetMethodAttribute(MethodDescriptor targetMethodDesc)
        {
            var attributes = targetMethodDesc.TargetMethodInfo.GetCustomAttributes<MethodDescriptorAttribute>();
            var attr = default(MethodDescriptorAttribute);

            for (var current = _currentFork; current >= HardFork.Genesis && Enum.IsDefined(current); current--)
            {
                attr = attributes.FirstOrDefault(s => s.Fork == current);

                if (attr is null) continue; else break;
            }

            return attr ?? throw new NotImplementedException($"Missing attribute \'{typeof(MethodDescriptorAttribute)}\' from method \'{targetMethodDesc.TargetMethod.Target!.GetType().FullName}.{targetMethodDesc.TargetMethodInfo.Name}\'.");
        }

        internal MethodDescriptor GetSystemCallMethod(uint systemCallAddress)
        {
            for (var current = _currentFork; current >= HardFork.Genesis; current--)
            {
                if (_systemCallMethods.TryGetValue(systemCallAddress, out var table) &&
                    table.TryGetValue(current, out var methodDescriptor))
                {
                    return methodDescriptor;
                }
            }

            throw new MissingMethodException($"SYSCALL [{systemCallAddress:D}]");
        }

        internal void ExecuteSystemCall(uint systemCallAddress)
        {
            var methodDesc = GetSystemCallMethod(systemCallAddress);
            var parameters = new object?[methodDesc.Parameters.Count];
            var vmObjectType = typeof(VMObject);

            for (var i = 0; i < parameters.Length; i++)
            {
                var paramType = methodDesc.Parameters[i].ParameterType;
                var paramConverter = TypeDescriptor.GetConverter(vmObjectType);
                var vmObject = _currentContext!.Pop();

                if (paramConverter.CanConvertTo(paramType) == false)
                    throw new InvalidCastException($"No conversion from \'{vmObject.Type}\' to \'{paramType}\'.");

                parameters[i] = paramConverter.ConvertTo(vmObject, paramType);
            }

            var result = methodDesc.TargetMethod.DynamicInvoke(parameters);

            if (methodDesc.TargetMethodInfo.ReturnType != typeof(void))
                _currentContext!.Push(result.ToStackItem());

            _maxGasConsumed += GasTable.GetGasCost(methodDesc, _currentFork);
        }
    }
}
