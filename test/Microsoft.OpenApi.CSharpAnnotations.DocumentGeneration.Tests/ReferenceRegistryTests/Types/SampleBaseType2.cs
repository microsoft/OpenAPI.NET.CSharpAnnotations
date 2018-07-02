// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.ReferenceRegistryTests.Types
{
    internal class SampleBaseType2
    {
        public static readonly OpenApiSchema schema = new OpenApiSchema
        {
            Type = "object",
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["sampleList"] = new OpenApiSchema
                {
                    Type = "array",
                    Items = new OpenApiSchema
                    {
                        Type = "string"
                    }
                },
                ["SampleList2"] = new OpenApiSchema
                {
                    Type = "array",
                    Items = new OpenApiSchema
                    {
                        Type = "integer",
                        Format = "int32"
                    }
                }
            }
        };

        [JsonProperty("sampleList")]
        public IList<string> SampleStringList { get; }

        public IList<int> SampleList2 { get; }
    }
}