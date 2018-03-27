// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.Contracts
{
    /// <summary>
    /// Sample contract for object 2
    /// </summary>
    public sealed class SampleObject2
    {
        /// <summary>
        /// Gets or sets the sample property enum
        /// </summary>
        [JsonProperty(PropertyName = "samplePropertyEnum", Required = Required.Default)]
        public SampleEnum1 SamplePropertyEnum { get; set; }

        /// <summary>
        /// Gets or sets the sample property string 1
        /// </summary>
        [JsonProperty(PropertyName = "samplePropertyInt", Required = Required.Always)]
        public string SamplePropertyInt { get; set; }

        /// <summary>
        /// Gets the sample property object dictionary
        /// </summary>
        [JsonProperty(PropertyName = "samplePropertyObjectDictionary", Required = Required.Always)]
        public IDictionary<string, SampleObject1> SamplePropertyObjectDictionary { get; } = new Dictionary<string, SampleObject1>();

        /// <summary>
        /// Gets the sample property object list
        /// </summary>
        [JsonProperty(PropertyName = "samplePropertyObjectList", Required = Required.Always)]
        public IList<SampleObject1> SamplePropertyObjectList { get; } = new List<SampleObject1>();

        /// <summary>
        /// Gets or sets the sample property string 1
        /// </summary>
        [JsonProperty(PropertyName = "samplePropertyString1", Required = Required.Default)]
        public string SamplePropertyString1 { get; set; }
    }
}