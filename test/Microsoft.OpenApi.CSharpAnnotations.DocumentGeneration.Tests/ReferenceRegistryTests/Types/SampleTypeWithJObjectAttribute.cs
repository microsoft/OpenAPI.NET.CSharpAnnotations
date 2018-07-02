// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.ReferenceRegistryTests.Types
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    internal class SampleTypeWithJObjectAttribute
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
                ["sampleProperty2"] = new OpenApiSchema
                {
                    Type = "string"
                },
            }
        };

        public string SampleProperty { get; set; }

        public string SampleProperty2 { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string SamplePropertyStringRequired { get; set; }
    }
}