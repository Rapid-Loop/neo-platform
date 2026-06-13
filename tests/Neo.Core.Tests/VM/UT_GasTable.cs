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
using Neo.Core.VM.Specs;

namespace Neo.Core.Tests.VM
{
    [TestClass]
    public class UT_GasTable
    {
        [TestMethod]
        public void TestOpCodePriceAttribute()
        {
            var actualCallTGenesisGasPrice = GasTable.GetGasCost(OpCode.CALLT, HardFork.Genesis);

            // This fallbacks to Genesis Gas Price if Hard Fork Price isn't set
            var actualCallTCockatriceGasPrice = GasTable.GetGasCost(OpCode.CALLT, HardFork.Cockatrice);
            var actualAllGasPrices = GasTable.GetAllCosts(HardFork.Gorgon);

            Assert.AreEqual(32768L, actualAllGasPrices[OpCode.CALLT]);
            Assert.AreEqual(32768L, actualCallTGenesisGasPrice);
            Assert.AreEqual(32768L, actualCallTCockatriceGasPrice);
            Assert.AreEqual(actualCallTGenesisGasPrice, actualCallTCockatriceGasPrice);
        }
    }
}
