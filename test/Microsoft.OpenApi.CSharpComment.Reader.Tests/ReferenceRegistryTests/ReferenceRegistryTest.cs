// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.CSharpComment.Reader.Extensions;
using Microsoft.OpenApi.CSharpComment.Reader.ReferenceRegistries;
using Microsoft.OpenApi.CSharpComment.Reader.Tests.ReferenceRegistryTests.Types;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.CSharpComment.Reader.Tests.ReferenceRegistryTests
{
    [Collection("DefaultSettings")]
    public class ReferenceRegistryTest
    {
        private readonly ITestOutputHelper _output;

        public ReferenceRegistryTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [MemberData(nameof(GetTestCasesForGenerateSchemaFromTypeShouldSucceed))]
        public void GenerateSchemaFromTypeShouldSucceed(
            Type type,
            OpenApiSchema expectedSchema,
            IDictionary<string, OpenApiSchema> expectedReferences)
        {
            _output.WriteLine(type.ToString());

            // Arrange
            var referenceRegistryManager = new ReferenceRegistryManager();

            // Act
            var returnedSchema =
                referenceRegistryManager.FindOrAddSchemaReference(type);

            // Assert
            var actualSchema = returnedSchema;
            var actualReferences = referenceRegistryManager.SchemaReferenceRegistry.References;

            _output.WriteLine(actualSchema.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0));
            foreach (var reference in actualReferences)
            {
                _output.WriteLine(reference.Key);
                _output.WriteLine(reference.Value.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0));
            }

            actualSchema.Should().BeEquivalentTo(expectedSchema);
            actualReferences.Should().BeEquivalentTo(expectedReferences);
        }

        public static IEnumerable<object[]> GetTestCasesForGenerateSchemaFromTypeShouldSucceed()
        {
            // Simple types
            yield return new object[]
            {
                typeof(bool),
                new OpenApiSchema
                {
                    Type = "boolean"
                },
                new Dictionary<string, OpenApiSchema>()
            };

            yield return new object[]
            {
                typeof(byte),
                new OpenApiSchema
                {
                    Type = "string",
                    Format = "byte"
                },
                new Dictionary<string, OpenApiSchema>()
            };
            yield return new object[]
            {
                typeof(char),
                new OpenApiSchema
                {
                    Type = "string",
                    MinLength = 1,
                    MaxLength = 1
                },
                new Dictionary<string, OpenApiSchema>()
            };
            yield return new object[]
            {
                typeof(string),
                new OpenApiSchema
                {
                    Type = "string"
                },
                new Dictionary<string, OpenApiSchema>()
            };
            yield return new object[]
            {
                typeof(int),
                new OpenApiSchema
                {
                    Type = "integer",
                    Format = "int32"
                },
                new Dictionary<string, OpenApiSchema>()
            };
            yield return new object[]
            {
                typeof(Guid),
                new OpenApiSchema
                {
                    Type = "string",
                    Format = "uuid",
                    MinLength = 36,
                    MaxLength = 36,
                    Example = new OpenApiString("00000000-0000-0000-0000-000000000000")
                },
                new Dictionary<string, OpenApiSchema>()
            };
            yield return new object[]
            {
                typeof(DateTimeOffset),
                new OpenApiSchema
                {
                    Type = "string",
                    Format = "date-time"
                },
                new Dictionary<string, OpenApiSchema>()
            };

            // Enum types
            yield return new object[]
            {
                typeof(SampleEnum),
                new OpenApiSchema
                {
                    Type = "string",
                    Enum = new List<IOpenApiAny>
                    {
                        new OpenApiString(SampleEnum.SampleEnumValueFirst.ToString()),
                        new OpenApiString(SampleEnum.SampleEnumValueSecond.ToString())
                    }
                },
                new Dictionary<string, OpenApiSchema>()
            };

            // Object types
            yield return new object[]
            {
                typeof(SampleInnerType),
                new OpenApiSchema
                {
                    Reference = new OpenApiReference
                    {
                        Id = typeof(SampleInnerType).ToString().SanitizeClassName(),
                        Type = ReferenceType.Schema
                    }
                },
                new Dictionary<string, OpenApiSchema>
                {
                    [typeof(SampleInnerType).ToString().SanitizeClassName()]
                    = SampleInnerType.schema
                }
            };

            yield return new object[]
            {
                typeof(SampleType),
                new OpenApiSchema
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.Schema,
                        Id = typeof(SampleType).ToString().SanitizeClassName()
                    }
                },
                new Dictionary<string, OpenApiSchema>
                {
                    [typeof(SampleType).ToString().SanitizeClassName()]
                    = SampleType.schema,
                    [typeof(SampleInnerType).ToString().SanitizeClassName()]
                    = SampleInnerType.schema
                }
            };
            yield return new object[]
            {
                typeof(SampleSelfReferencingType),
                new OpenApiSchema
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.Schema,
                        Id = typeof(SampleSelfReferencingType).ToString().SanitizeClassName()
                    }
                },
                new Dictionary<string, OpenApiSchema>
                {
                    [typeof(SampleSelfReferencingType).ToString().SanitizeClassName()]
                    = SampleSelfReferencingType.schema
                }
            };

            // Collection types
            yield return new object[]
            {
                typeof(IList<SampleType>),
                new OpenApiSchema
                {
                    Type = "array",
                    Items = new OpenApiSchema
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.Schema,
                            Id = typeof(SampleType).ToString().SanitizeClassName()
                        }
                    }
                },
                new Dictionary<string, OpenApiSchema>
                {
                    [typeof(SampleInnerType).ToString().SanitizeClassName()]
                    = SampleInnerType.schema,
                    [typeof(SampleType).ToString().SanitizeClassName()]
                    = SampleType.schema
                },
            };
            yield return new object[]
            {
                typeof(IDictionary<string, SampleType>),
                new OpenApiSchema
                {
                    Type = "object",
                    AdditionalProperties = new OpenApiSchema
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.Schema,
                            Id = typeof(SampleType).ToString().SanitizeClassName()
                        }
                    }
                },
                new Dictionary<string, OpenApiSchema>
                {
                    [typeof(SampleInnerType).ToString().SanitizeClassName()]
                    = SampleInnerType.schema,
                    [typeof(SampleType).ToString().SanitizeClassName()]
                    = SampleType.schema
                },
            };

            // Generic types
            yield return new object[]
            {
                typeof(SampleGenericType<string>),
                new OpenApiSchema
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.Schema,
                        Id = typeof(SampleGenericType<string>).ToString().SanitizeClassName()
                    }
                },
                new Dictionary<string, OpenApiSchema>
                {
                    [typeof(SampleGenericType<string>).ToString().SanitizeClassName()]
                    = SampleGenericType<string>.schemaString
                },
            };
            yield return new object[]
            {
                typeof(ISampleGenericInterfaceType<string, SampleType>),
                new OpenApiSchema
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.Schema,
                        Id = typeof(ISampleGenericInterfaceType<string, SampleType>).ToString().SanitizeClassName()
                    }
                },
                new Dictionary<string, OpenApiSchema>
                {
                    [typeof(ISampleGenericInterfaceType<string, SampleType>).ToString().SanitizeClassName()]
                    = SampleGenericInterfaceType.schemaInnerNestedInterface,
                    [typeof(SampleType).ToString().SanitizeClassName()]
                    = SampleType.schema,
                    [typeof(SampleInnerType).ToString().SanitizeClassName()]
                    = SampleInnerType.schema
                }
            };
            yield return new object[]
            {
                typeof(
                    ISampleGenericInterfaceType<
                        ISampleGenericInterfaceType<
                            string,
                            ISampleGenericInterfaceType<string, SampleType>>,
                        SampleType>),
                new OpenApiSchema
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.Schema,
                        Id =
                            typeof(
                                    ISampleGenericInterfaceType<
                                        ISampleGenericInterfaceType<
                                            string,
                                            ISampleGenericInterfaceType<string, SampleType>>,
                                        SampleType>).ToString()
                                .SanitizeClassName()
                    }
                },
                new Dictionary<string, OpenApiSchema>
                {
                    [typeof(
                            ISampleGenericInterfaceType<
                                ISampleGenericInterfaceType<
                                    string,
                                    ISampleGenericInterfaceType<string, SampleType>>,
                                SampleType>).ToString()
                        .SanitizeClassName()]
                    = SampleGenericInterfaceType.schemaOuterNestedInterface,
                    [typeof(ISampleGenericInterfaceType<
                            string,
                            ISampleGenericInterfaceType<string, SampleType>>).ToString()
                        .SanitizeClassName()]
                    = SampleGenericInterfaceType.schemaMiddleNestedInterface,
                    [typeof(ISampleGenericInterfaceType<string, SampleType>).ToString().SanitizeClassName()]
                    = SampleGenericInterfaceType.schemaInnerNestedInterface,
                    [typeof(SampleType).ToString().SanitizeClassName()]
                    = SampleType.schema,
                    [typeof(SampleInnerType).ToString().SanitizeClassName()]
                    = SampleInnerType.schema
                }
            };
            yield return new object[]
            {
                typeof(SampleInheritFromGenericType),
                new OpenApiSchema
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.Schema,
                        Id = typeof(SampleInheritFromGenericType).ToString().SanitizeClassName()
                    }
                },
                new Dictionary<string, OpenApiSchema>
                {
                    [typeof(SampleInheritFromGenericType).ToString().SanitizeClassName()]
                    = SampleInheritFromGenericType.schema,
                    [typeof(SampleType).ToString().SanitizeClassName()]
                    = SampleType.schema,
                    [typeof(SampleInnerType).ToString().SanitizeClassName()]
                    = SampleInnerType.schema
                }
            };
        }
    }
}