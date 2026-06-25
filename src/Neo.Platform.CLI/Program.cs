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

using Microsoft.Extensions.Hosting;
using Neo.Platform.CLI.Commands;
using Neo.Platform.Hosting;
using Neo.Platform.Hosting.Builder;
using System.Threading.Tasks;

namespace Neo.Platform.CLI
{
    internal class Program
    {
        private static Task<int> Main(string[] args)
        {
            var rootCommand = new ProgramRootCommand();
            var cmd = new PlatformCommandLineBuilder(rootCommand, args)
                .UseHost(DefaultNeoBuildHostFactory, builder =>
                {
                    //builder.UseCommandAction<ProgramRootCommand, EmptyHandler>();
                    builder.UseCommandAction<ShowCommand, ShowCommand.Handler>();
                })
                .EnablePosixBundling()
                .EnableDefaultExceptionHandler(false)
                .Build();

            return cmd.InvokeAsync();
        }

        private static IHostBuilder DefaultNeoBuildHostFactory(string[] args) =>
            new HostBuilder()
            .UseNeoPlatformConfiguration();
    }
}
