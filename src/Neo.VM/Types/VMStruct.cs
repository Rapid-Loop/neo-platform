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

namespace Neo.VM.Types
{
    public class VMStruct(IEnumerable<VMObject> items) : VMArray(items)
    {
        public override VMObjectType Type => VMObjectType.Struct;

        public VMStruct() : this([]) { }

        public override VMObject Clone()
        {
            var clone = new VMStruct();

            // Important: Use a mapping to handle cycles during cloning
            var objectMap = new Dictionary<VMObject, VMObject>();

            _array.ForEach(i =>
            {
                if (i is null || i is VMNull)
                {
                    clone._array.Add(VMNull.Instance);
                    return;
                }

                if (objectMap.TryGetValue(i, out var alreadyCloned))
                {
                    // Cycle detected during cloning - reuse the cloned object
                    alreadyCloned.AddReference();
                    clone._array.Add(alreadyCloned);
                }
                else
                {
                    var clonedItem = i.Clone();
                    objectMap[i] = clonedItem;
                    clone._array.Add(clonedItem);
                }
            });

            clone.AddReference();
            return clone;
        }
    }
}
