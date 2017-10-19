// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.OpenApi.CSharpComment.Reader.Tests.SampleApis.Contracts
{
    /// <summary>
    /// Sample contract for object 3
    /// </summary>
    public sealed class SampleObject3
    {
        /// <summary>
        /// Gets or sets sample property object
        /// </summary>
        [JsonProperty(PropertyName = "samplePropertyObject", Required = Required.Always)]
        public SampleObject2 SamplePropertyObject { get; set; }

        /// <summary>
        /// Gets the sample property object list
        /// </summary>
        [JsonProperty(PropertyName = "samplePropertyObjectList", Required = Required.Default)]
        public IList<SampleObject1> SamplePropertyObjectList { get; } = new List<SampleObject1>();
    }
}