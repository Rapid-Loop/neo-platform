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

using System.IO;
using System.Threading;

namespace Neo.Platform.Hosting.Factory
{
    internal static class NeoHostingFactory
    {
        private static readonly uint s_networkSeed = 810960196u; // DEV0 Magic Code
        private static readonly string s_globalString = "Global\\";

        public static uint GetDevNetwork(uint index) =>
            s_networkSeed & ~(0xfu << 24) | (index << 24);

        public static FileInfo ResolveFileName(string filename, string rootPath)
        {
            if (Path.IsPathRooted(filename))
                return new(filename);
            if (Path.IsPathRooted(rootPath) == false)
                rootPath = Path.GetFullPath(rootPath);
            return new(Path.Combine(rootPath, filename));
        }

        public static Mutex CreateMutex(string? name = null)
        {
            if (string.IsNullOrEmpty(name))
                name = Path.Combine(s_globalString, Path.GetRandomFileName());
            return new(initiallyOwned: true, name);
        }
    }
}
