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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neo.Core.Logging;
using Neo.VM.Builders;
using Neo.VM.Extensions;
using Neo.VM.Middleware;
using System;

namespace Neo.VM.Tests
{
    internal class TestUtilities
    {
        private static readonly ServiceProvider s_serviceProvider;
        public static IServiceProvider Services => s_serviceProvider;

        public static readonly ILoggerFactory TraceLoggerFactory = LoggerFactory.Create(logging =>
        {
            logging.ClearProviders();
            logging.AddNeoPlatform();
            logging.SetMinimumLevel(LogLevel.Trace);
        });

        static TestUtilities()
        {
            var services = new ServiceCollection();

            services
                .AddEngineDebugger()
                .AddExecuteLogger()
                .AddExecutionEngine()
                .AddScoped(sp =>
                {
                    var middleware = sp.GetServices<IEngineMiddleware>(); // NOTE: Get all custom middleware
                    var debugger = sp.GetRequiredService<DebuggerMiddleware>(); // NOTE: get the debugger middleware
                    var pipeline = VirtualMachinePipelineBuilder.Create()
                        .Use([debugger, .. middleware]) // NOTE: ORDER MATTERS HERE
                        .Build();

                    return new TestEngine(pipeline: pipeline);
                })
                .AddLogging(
                    logging =>
                    {
                        logging.ClearProviders();
                        logging.AddNeoPlatform(); // Custom logger for our platform (file, debugger, console, etc)
                        logging.SetMinimumLevel(LogLevel.Trace);
                    });

            s_serviceProvider = services.BuildServiceProvider();
        }
    }
}
