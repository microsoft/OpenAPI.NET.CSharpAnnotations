// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Extensions;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.ReferenceRegistryTests.Types
{
    internal class SampleChildType1 : SampleBaseType1, ISampleInterface
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
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.Schema,
                            Id = typeof(SampleChildType2).ToString().SanitizeClassName()
                        }
                    }
                },
                ["SampleStringList"] = new OpenApiSchema
                {
                    Type = "array",
                    ReadOnly = true,
                    Items = new OpenApiSchema
                    {
                        Type = "string"
                    }
                },
                ["SampleList2"] = new OpenApiSchema
                {
                    Type = "array",
                    ReadOnly = true,
                    Items = new OpenApiSchema
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.Schema,
                            Id = typeof(SampleBaseType2).ToString().SanitizeClassName()
                        }
                    }
                },
                ["SampleProperty"] = new OpenApiSchema
                {
                    ReadOnly = true,
                    Type = "string"
                }
            }
        };

        [JsonProperty("sampleList")]
        public new IList<SampleChildType2> SampleList { get; }

        // Conflicting property in Child class but its ignored so the property from base class should be used in schema.
        [JsonIgnore]
        public new IList<SampleChildType2> SampleList2 { get; }

        public string SampleProperty => throw new System.NotImplementedException();
    }
}