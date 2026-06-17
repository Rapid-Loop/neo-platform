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

using Microsoft.Extensions.Logging;
using Neo.Localization.VM;
using Neo.VM.Extensions;

namespace Neo.VM
{
    public partial class VirtualMachineEngine
    {
        internal void LogExecuteStartupMessage(LogLevel logLevel = LogLevel.Information)
        {
            if (_logger.IsEnabled(logLevel))
            {
                var message = VirtualMachineLocalizer.GetMessage(
                    VirtualMachineMessageNames.ExecuteStartup,
                    _currentFork,
                    _maxGasLimit
                );

                _logger.LogExecuteMessage(logLevel, message);
            }
        }

        internal void LogExecuteSuccessfullyMessage(LogLevel logLevel = LogLevel.Information)
        {
            if (_logger.IsEnabled(logLevel))
            {
                var message = VirtualMachineLocalizer.GetMessage(
                    VirtualMachineMessageNames.ExecuteSuccessfully,
                    _maxGasConsumed,
                    _maxGasLimit - _maxGasConsumed,
                    State
                );

                _logger.LogExecuteMessage(logLevel, message);
            }
        }

        internal void LogExecuteOpCodeMessage(LogLevel logLevel = LogLevel.Trace)
        {
            if (_logger.IsEnabled(logLevel) && _invocationStack.Count > 0)
            {
                var opcodeInst = _invocationStack.Peek().CurrentInstruction;
                var message = VirtualMachineLocalizer.GetMessage(
                    VirtualMachineMessageNames.ExecuteOpCode,
                    opcodeInst.Position,
                    opcodeInst.OpCode,
                    opcodeInst.DecodeOperand()
                );

                _logger.LogExecuteMessage(logLevel, message);
            }
        }
    }
}
