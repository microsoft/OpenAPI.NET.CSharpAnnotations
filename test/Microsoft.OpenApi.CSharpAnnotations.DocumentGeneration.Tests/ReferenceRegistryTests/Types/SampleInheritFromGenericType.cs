// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Extensions;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.ReferenceRegistryTests.Types
{
    internal class SampleInheritFromGenericType : ISampleGenericInterfaceType<SampleType, SampleInnerType>
    {
        public static readonly OpenApiSchema schema = new OpenApiSchema()
        {
            Type = "object",
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["SamplePropertyGenericTypeT1"] = new OpenApiSchema
                {
                    Reference = new OpenApiReference()
                    {
                        Type = ReferenceType.Schema,
                        Id = typeof(SampleType).ToString().SanitizeClassName()
                    }
                },
                ["SamplePropertyGenericTypeT2"] = new OpenApiSchema
                {
                    Reference = new OpenApiReference()
                    {
                        Type = ReferenceType.Schema,
                        Id = typeof(SampleInnerType).ToString().SanitizeClassName()
                    }
                },
                ["SamplePropertyString"] = new OpenApiSchema()
                {
                    Type = "string"
                }
            },
        };

        public SampleType SamplePropertyGenericTypeT1 { get; set; }

        public SampleInnerType SamplePropertyGenericTypeT2 { get; set; }

        public string SamplePropertyString { get; set; }
    }
}