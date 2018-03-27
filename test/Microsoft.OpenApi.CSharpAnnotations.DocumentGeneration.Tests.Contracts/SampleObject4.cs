// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.Contracts
{
    /// <summary>
    /// Sample contract for object 4
    /// </summary>
    public sealed class SampleObject4
    {
        /// <summary>
        /// Gets or sets sample property object
        /// </summary>
        [JsonProperty(PropertyName = "samplePropertyObject", Required = Required.Always)]
        public SampleObject2 SamplePropertyObject { get; set; }

        /// <summary>
        /// Gets or sets sample property object
        /// </summary>
        [JsonProperty]
        public SampleObject3 Sample3PropertyObject { get; set; }

        /// <summary>
        /// Gets the sample property object list
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public IList<SampleObject1> SamplePropertyObjectList { get; } = new List<SampleObject1>();
    }
}