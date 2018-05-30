// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.ReferenceRegistryTests.Types
{
    internal class SampleChildType2 : SampleBaseType2
    {
        public new static readonly OpenApiSchema schema = new OpenApiSchema
        {
            Type = "object",
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["sampleList"] = new OpenApiSchema
                {
                    Type = "array",
                    ReadOnly = true,
                    Items = new OpenApiSchema
                    {
                        Type = "integer",
                        Format = "int32"
                    }
                },
                ["SampleList2"] = new OpenApiSchema
                {
                    Type = "array",
                    ReadOnly = true,
                    Items = new OpenApiSchema
                    {
                        Type = "string"
                    }
                }
            }
        };

        [JsonProperty("sampleList")]
        public IList<int> SampleIntList { get; }

        public new IList<string> SampleList2 { get; }
    }
}