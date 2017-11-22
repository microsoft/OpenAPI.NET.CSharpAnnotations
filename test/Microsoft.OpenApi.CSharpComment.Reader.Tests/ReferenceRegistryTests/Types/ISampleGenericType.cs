// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.CSharpComment.Reader.Extensions;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpComment.Reader.Tests.ReferenceRegistryTests.Types
{
    internal interface ISampleGenericInterfaceType<T1, T2>
    { 
        T1 SamplePropertyGenericTypeT1 { get; set; }

        T2 SamplePropertyGenericTypeT2 { get; set; }

        string SamplePropertyString { get; set; }
    }

    internal class SampleGenericInterfaceType
    {
        public static readonly OpenApiSchema schemaInnerNestedInterface = new OpenApiSchema
        {
            Type = "object",
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["SamplePropertyGenericTypeT1"] = new OpenApiSchema
                {
                    Type = "string"
                },
                ["SamplePropertyGenericTypeT2"] = new OpenApiSchema
                {
                    Reference =  new OpenApiReference()
                    {
                        Type = ReferenceType.Schema,
                        Id = typeof(SampleType).ToString().SanitizeClassName()
                    }
                },
                ["SamplePropertyString"] = new OpenApiSchema()
                {
                    Type = "string"
                }
            },
        };

        public static readonly OpenApiSchema schemaOuterNestedInterface = new OpenApiSchema()
        {
            Type = "object",
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["SamplePropertyGenericTypeT1"] = new OpenApiSchema
                {
                    Reference = new OpenApiReference()
                    {
                        Type = ReferenceType.Schema,
                        Id = typeof(ISampleGenericInterfaceType<
                                string,
                                ISampleGenericInterfaceType<string, SampleType>>).ToString()
                            .SanitizeClassName()
                    }
                },
                ["SamplePropertyGenericTypeT2"] = new OpenApiSchema
                {
                    Reference = new OpenApiReference()
                    {
                        Type = ReferenceType.Schema,
                        Id = typeof(SampleType).ToString().SanitizeClassName()
                    }
                },
                ["SamplePropertyString"] = new OpenApiSchema()
                {
                    Type = "string"
                }
            },
        };

        public static readonly OpenApiSchema schemaMiddleNestedInterface = new OpenApiSchema()
        {
            Type = "object",
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["SamplePropertyGenericTypeT1"] = new OpenApiSchema
                {
                    Type = "string"
                },
                ["SamplePropertyGenericTypeT2"] = new OpenApiSchema
                {
                    Reference = new OpenApiReference()
                    {
                        Type = ReferenceType.Schema,
                        Id = typeof(ISampleGenericInterfaceType<string, SampleType>).ToString().SanitizeClassName()
                    }
                },
                ["SamplePropertyString"] = new OpenApiSchema()
                {
                    Type = "string"
                }
            },
        };
        
    }
}