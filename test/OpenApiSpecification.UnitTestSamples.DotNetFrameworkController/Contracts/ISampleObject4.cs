// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Newtonsoft.Json;

namespace OpenApiSpecification.UnitTestSamples.DotNetFrameworkController.Contracts
{
    /// <summary>
    /// Interface Sample Object 4
    /// </summary>
    public interface ISampleObject4<T1, T2>
    {
        /// <summary>
        /// Sample Property of Type T1
        /// </summary>
        [JsonProperty(PropertyName = "samplePropertyTypeT1", Required = Required.Always)]
        T1 SamplePropertyTypeT1 { get; set; }

        /// <summary>
        /// Sample Property of Type T2
        /// </summary>
        [JsonProperty(PropertyName = "samplePropertyTypeT2", Required = Required.Always)]
        T2 SamplePropertyTypeT2 { get; set; }
    }
}