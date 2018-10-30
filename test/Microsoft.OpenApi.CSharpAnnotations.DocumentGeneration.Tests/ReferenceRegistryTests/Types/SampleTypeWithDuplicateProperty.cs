// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.ReferenceRegistryTests.Types
{
    internal class SampleTypeWithDuplicateProperty
    {
        [JsonProperty("sampleList")]
        public IList<string> SampleStringList { get; }

        [JsonProperty("sampleList")]
        public IList<int> SampleList2 { get; }
    }
}