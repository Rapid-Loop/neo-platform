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

namespace Neo.VM.Logging
{
    public sealed class LoggerEvents
    {
        public const int Fault = 100;

        public const int Create = 200;
        public const int Load = 201;
        public const int PrePost = 202;
        public const int Post = 203;
        public const int Break = 204;
        public const int Execute = 205;

        public const int Burn = 300;
        public const int Call = 301;
        public const int Notify = 302;
        public const int Log = 303;

        public const int Persist = 400;
        public const int PostPersist = 401;

        public const int StoragePut = 500;
        public const int StorageGet = 501;
        public const int StorageFind = 502;
        public const int StorageDelete = 503;

        public const int IteratorNext = 600;
        public const int IteratorGet = 601;

        public const int ReadStorage = 700;
        public const int UpdateStorage = 701;
    }
}
