// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.ReferenceRegistryTests.Types
{
    internal class SampleTypeWithNullableEnum
    {
        public static readonly OpenApiSchema schema = new OpenApiSchema
        {
            Type = "object",
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["SamplePropertyInnerEnum"] = new OpenApiSchema
                {
                    Type = "string",
                    Enum = new List<IOpenApiAny>
                    {
                        new OpenApiString(SampleEnum.SampleEnumValueFirst.ToString()),
                        new OpenApiString(SampleEnum.SampleEnumValueSecond.ToString())
                    },
                    Nullable = true
                },
                ["SamplePropertyInnerListInt"] = new OpenApiSchema
                {
                    Type = "array",
                    Items = new OpenApiSchema
                    {
                        Type = "integer",
                        Format = "int32"
                    }
                },
                ["SamplePropertyInnerString"] = new OpenApiSchema
                {
                    Type = "string"
                }
            }
        };

        [JsonIgnore]
        public string SampleIgnoredPropertyInnerString { get; set; }

        public SampleEnum? SamplePropertyInnerEnum { get; set; }

        public IList<int> SamplePropertyInnerListInt { get; } = new List<int>();

        public string SamplePropertyInnerString { get; set; }
    }
}
