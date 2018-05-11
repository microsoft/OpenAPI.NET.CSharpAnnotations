// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Extensions;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.ReferenceRegistryTests.Types
{
    internal class SampleType
    {
        public static readonly OpenApiSchema schema = new OpenApiSchema
        {
            Type = "object",
            Required = new HashSet<string>
            {
                "samplePropertyReadonlyObject",
                "samplePropertyStringRequired"
            },
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["SamplePropertyDictionaryStringObject"] = new OpenApiSchema
                {
                    Type = "object",
                    ReadOnly = true,
                    AdditionalProperties = new OpenApiSchema
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.Schema,
                            Id = typeof(SampleInnerType).ToString().SanitizeClassName()
                        }
                    }
                },
                ["SamplePropertyDictionaryStringString"] = new OpenApiSchema
                {
                    Type = "object",
                    ReadOnly = true,
                    AdditionalProperties = new OpenApiSchema
                    {
                        Type = "string"
                    }
                },
                ["SamplePropertyEnum"] = new OpenApiSchema
                {
                    Type = "string",
                    ReadOnly = false,
                    Enum = new List<IOpenApiAny>
                    {
                        new OpenApiString(SampleEnum.SampleEnumValueFirst.ToString()),
                        new OpenApiString(SampleEnum.SampleEnumValueSecond.ToString())
                    }
                },
                ["SamplePropertyIDictionaryStringObject"] = new OpenApiSchema
                {
                    Type = "object",
                    ReadOnly = true,
                    AdditionalProperties = new OpenApiSchema
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.Schema,
                            Id = typeof(SampleInnerType).ToString().SanitizeClassName()
                        }
                    }
                },
                ["SamplePropertyIDictionaryStringString"] = new OpenApiSchema
                {
                    Type = "object",
                    ReadOnly = true,
                    AdditionalProperties = new OpenApiSchema
                    {
                        Type = "string"
                    }
                },
                ["SamplePropertyIListObject"] = new OpenApiSchema
                {
                    Type = "array",
                    ReadOnly = true,
                    Items = new OpenApiSchema
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.Schema,
                            Id = typeof(SampleInnerType).ToString().SanitizeClassName()
                        }
                    }
                },
                ["SamplePropertyInt"] = new OpenApiSchema
                {
                    Type = "integer",
                    Format = "int32",
                    ReadOnly = false
                },
                ["SamplePropertyListEnum"] = new OpenApiSchema
                {
                    Type = "array",
                    Items = new OpenApiSchema
                    {
                        Type = "string",
                        Enum = new List<IOpenApiAny>
                        {
                            new OpenApiString(SampleEnum.SampleEnumValueFirst.ToString()),
                            new OpenApiString(SampleEnum.SampleEnumValueSecond.ToString())
                        }
                    },
                    ReadOnly = true
                },
                ["SamplePropertyListObject"] = new OpenApiSchema
                {
                    Type = "array",
                    Items = new OpenApiSchema
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.Schema,
                            Id = typeof(SampleInnerType).ToString().SanitizeClassName()
                        }
                    },
                    ReadOnly = true
                },
                ["SamplePropertyListString"] = new OpenApiSchema
                {
                    Type = "array",
                    Items = new OpenApiSchema
                    {
                        Type = "string"
                    },
                    ReadOnly = true
                },
                ["SamplePropertyObject"] = new OpenApiSchema
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.Schema,
                        Id = typeof(SampleInnerType).ToString().SanitizeClassName()
                    }
                },
                ["samplePropertyReadonlyObject"] = new OpenApiSchema
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.Schema,
                        Id = typeof(SampleInnerType).ToString().SanitizeClassName()
                    },
                    ReadOnly = true
                },
                ["samplePropertyStringNotRequired"] = new OpenApiSchema
                {
                    Type = "string"
                },
                ["samplePropertyStringRequired"] = new OpenApiSchema
                {
                    Type = "string"
                }
            }
        };

        public Dictionary<string, SampleInnerType> SamplePropertyDictionaryStringObject { get; } =
            new Dictionary<string, SampleInnerType>();

        public Dictionary<string, string> SamplePropertyDictionaryStringString { get; } =
            new Dictionary<string, string>();

        public SampleEnum SamplePropertyEnum { get; set; }

        public IDictionary<string, SampleInnerType> SamplePropertyIDictionaryStringObject { get; } =
            new Dictionary<string, SampleInnerType>();

        public IDictionary<string, string> SamplePropertyIDictionaryStringString { get; } =
            new Dictionary<string, string>();

        [JsonIgnore]
        public SampleInnerType SamplePropertyIgnoreObject { get; } = new SampleInnerType();

        public IList<SampleInnerType> SamplePropertyIListObject { get; } = new List<SampleInnerType>();

        public int SamplePropertyInt { get; set; }

        public IList<SampleEnum> SamplePropertyListEnum { get; } = new List<SampleEnum>();

        public List<SampleInnerType> SamplePropertyListObject { get; } = new List<SampleInnerType>();

        public IList<string> SamplePropertyListString { get; } = new List<string>();

        public SampleInnerType SamplePropertyObject { get; set; }

        [JsonProperty("samplePropertyReadonlyObject", Required = Required.Always)]
        public SampleInnerType SamplePropertyReadonlyObject { get; } = new SampleInnerType();

        [JsonProperty("samplePropertyStringNotRequired", Required = Required.Default)]
        public string SamplePropertyStringNotRequired { get; set; }

        [JsonProperty("samplePropertyStringRequired", Required = Required.Always)]
        public string SamplePropertyStringRequired { get; set; }
    }
}