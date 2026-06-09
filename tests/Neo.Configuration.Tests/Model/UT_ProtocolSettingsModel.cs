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

using Neo.Configuration.Models;
using Neo.Cryptography.ECC;

namespace Neo.Configuration.Tests.Model
{
    [TestClass]
    public class UT_ProtocolSettingsModel
    {
        [TestMethod]
        public void TestPropertyValues()
        {
            var jsonTestString = "{\"network\":810960196,\"addressVersion\":53,\"millisecondsPerBlock\":1000,\"maxTransactionsPerBlock\":512,\"memoryPoolMaxTransactions\":50000,\"maxTraceableBlocks\":2102400,\"hardforks\":{\"HF_Aspidochelone\":666},\"initialGasDistribution\":5200000000000000,\"validatorsCount\":1,\"standbyCommittee\":[\"03b209fd4f53a7170ea4444e0cb0a6bb6a53c2bd016926989cf85f9b0fba17a70c\"],\"seedList\":[\"seed1.neo.org:10333\"]}";
            var actualProtocolOptionsModel = JsonModel.FromJson<ProtocolSettingsModel>(jsonTestString, TestDefaults.JsonDefaultSerializerOptions);

            Assert.IsNotNull(actualProtocolOptionsModel);

            Assert.AreEqual(810960196u, actualProtocolOptionsModel.Network);
            Assert.AreEqual(53, actualProtocolOptionsModel.AddressVersion);
            Assert.AreEqual(1_000u, actualProtocolOptionsModel.MillisecondsPerBlock);
            Assert.AreEqual(512u, actualProtocolOptionsModel.MaxTransactionsPerBlock);
            Assert.AreEqual(50_000, actualProtocolOptionsModel.MemoryPoolMaxTransactions);
            Assert.AreEqual(2_102_400u, actualProtocolOptionsModel.MaxTraceableBlocks);
            Assert.AreEqual(5_200_000_000_000_000uL, actualProtocolOptionsModel.InitialGasDistribution);
            Assert.AreEqual(1, actualProtocolOptionsModel.ValidatorsCount);

            Assert.IsNotNull(actualProtocolOptionsModel.Hardforks);
            Assert.AreEqual(666u, actualProtocolOptionsModel.Hardforks[Hardfork.HF_Aspidochelone]);

            Assert.IsNotNull(actualProtocolOptionsModel.StandbyCommittee);
            Assert.AreEqual(ECPoint.Parse("03b209fd4f53a7170ea4444e0cb0a6bb6a53c2bd016926989cf85f9b0fba17a70c", ECCurve.SecP256r1), actualProtocolOptionsModel.StandbyCommittee[0]);

            Assert.IsNotNull(actualProtocolOptionsModel.SeedList);
            Assert.AreEqual("seed1.neo.org:10333", actualProtocolOptionsModel.SeedList[0]);
        }

        [TestMethod]
        public void TestObjectToProtocolSettings()
        {
            var jsonTestString = "{\"network\":810960196,\"addressVersion\":53,\"millisecondsPerBlock\":1000,\"maxTransactionsPerBlock\":512,\"memoryPoolMaxTransactions\":50000,\"maxTraceableBlocks\":2102400,\"hardforks\":{\"HF_Aspidochelone\":666},\"initialGasDistribution\":5200000000000000,\"validatorsCount\":1,\"standbyCommittee\":[\"03b209fd4f53a7170ea4444e0cb0a6bb6a53c2bd016926989cf85f9b0fba17a70c\"],\"seedList\":[\"seed1.neo.org:10333\"]}";
            var actualProtocolOptionsModel = JsonModel.FromJson<ProtocolSettingsModel>(jsonTestString, TestDefaults.JsonDefaultSerializerOptions);

            Assert.IsNotNull(actualProtocolOptionsModel);

            var actualProtocolSettings = actualProtocolOptionsModel.ToObject();

            Assert.AreEqual(810960196u, actualProtocolSettings.Network);
            Assert.AreEqual(53, actualProtocolSettings.AddressVersion);
            Assert.AreEqual(1_000u, actualProtocolSettings.MillisecondsPerBlock);
            Assert.AreEqual(512u, actualProtocolSettings.MaxTransactionsPerBlock);
            Assert.AreEqual(50_000, actualProtocolSettings.MemoryPoolMaxTransactions);
            Assert.AreEqual(2_102_400u, actualProtocolSettings.MaxTraceableBlocks);
            Assert.AreEqual(5_200_000_000_000_000uL, actualProtocolSettings.InitialGasDistribution);
            Assert.AreEqual(1, actualProtocolSettings.ValidatorsCount);

            Assert.IsNotNull(actualProtocolSettings.Hardforks);
            Assert.AreEqual(666u, actualProtocolSettings.Hardforks[Hardfork.HF_Aspidochelone]);

            Assert.IsNotNull(actualProtocolSettings.StandbyCommittee);
            Assert.AreEqual(ECPoint.Parse("03b209fd4f53a7170ea4444e0cb0a6bb6a53c2bd016926989cf85f9b0fba17a70c", ECCurve.SecP256r1), actualProtocolSettings.StandbyCommittee[0]);

            Assert.IsNotNull(actualProtocolSettings.SeedList);
            Assert.AreEqual("seed1.neo.org:10333", actualProtocolSettings.SeedList[0]);
        }
    }
}
