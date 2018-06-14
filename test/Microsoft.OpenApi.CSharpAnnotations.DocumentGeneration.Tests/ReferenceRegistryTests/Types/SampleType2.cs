// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.ReferenceRegistryTests.Types
{
    internal class SampleType2
    {
        public static readonly OpenApiSchema schema = new OpenApiSchema
        {
            Type = "object",
            Required = new HashSet<string>
            {
                "samplePropertyStringRequired"
            },
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["samplePropertyStringRequired"] = new OpenApiSchema
                {
                    Type = "string"
                },
                ["sampleProperty"] = new OpenApiSchema
                {
                    Type = "string"
                },
                ["samplePropertyDiffName"] = new OpenApiSchema
                {
                    Type = "string"
                },
            }
        };

        [JsonProperty("SampleProperty", Required = Required.Default)]
        public string SampleProperty { get; set; }

        [JsonProperty("SamplePropertyDiffName")]
        public string SampleProperty2 { get; set; }

        [JsonProperty("samplePropertyStringRequired", Required = Required.Always)]
        public string SamplePropertyStringRequired { get; set; }
    }
}