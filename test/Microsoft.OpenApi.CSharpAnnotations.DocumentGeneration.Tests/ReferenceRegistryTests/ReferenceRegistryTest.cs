// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Exceptions;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Extensions;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.ReferenceRegistries;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.ReferenceRegistryTests.Types;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.ReferenceRegistryTests
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
        [MemberData(nameof(GetTestCasesForGenerateSchemaFromTypeShouldFail))]
        public void GenerateSchemaFromTypeShouldFail(
            Type type,
            string expectedExceptionMessage)
        {
            var referenceRegistryManager =
                new ReferenceRegistryManager(
                    new OpenApiDocumentGenerationSettings(
                        new SchemaGenerationSettings(new DefaultPropertyNameResolver())));

            Action action = () => referenceRegistryManager.FindOrAddSchemaReference(type);
            action.Should().Throw<DocumentationException>(expectedExceptionMessage);
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
            var referenceRegistryManager =
                 new ReferenceRegistryManager(
                     new OpenApiDocumentGenerationSettings(
                         new SchemaGenerationSettings(new DefaultPropertyNameResolver())));

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

        [Theory]
        [MemberData(nameof(GetTestCasesForGenerateSchemaCustomSettingsShouldSucceed))]
        public void GenerateSchemaCustomSettingsShouldSucceed(
            Type type,
            OpenApiDocumentGenerationSettings generationSettings,
            OpenApiSchema expectedSchema,
            IDictionary<string, OpenApiSchema> expectedReferences)
        {
            _output.WriteLine(type.ToString());

            // Arrange
            var referenceRegistryManager =
                new ReferenceRegistryManager(generationSettings);

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

        public static IEnumerable<object[]> GetTestCasesForGenerateSchemaCustomSettingsShouldSucceed()
        {
            yield return new object[]
            {
                typeof(SampleType2),
                new OpenApiDocumentGenerationSettings(new SchemaGenerationSettings(new CamelCasePropertyNameResolver())),
                new OpenApiSchema
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.Schema,
                        Id = typeof(SampleType2).ToString().SanitizeClassName()
                    }
                },
                new Dictionary<string, OpenApiSchema>
                {
                    [typeof(SampleType2).ToString().SanitizeClassName()]
                    = SampleType2.schema,
                }
            };
        }

        public static IEnumerable<object[]> GetTestCasesForGenerateSchemaFromTypeShouldFail()
        {
            yield return new object[]
            {
                typeof(SampleTypeWithDuplicateProperty),
                string.Format(SpecificationGenerationMessages.AddingSchemaReferenceFailed,
                    typeof(SampleTypeWithDuplicateProperty).ToString().SanitizeClassName(),
                    string.Format(
                        SpecificationGenerationMessages.DuplicateProperty,
                        "sampleList",
                        typeof(SampleTypeWithDuplicateProperty)))
            };
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
                }
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
                }
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
                }
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

            //Child and Base class types
            yield return new object[]
            {
                typeof(SampleChildType1),
                new OpenApiSchema
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.Schema,
                        Id = typeof(SampleChildType1).ToString().SanitizeClassName()
                    }
                },
                new Dictionary<string, OpenApiSchema>
                {
                    [typeof(SampleChildType1).ToString().SanitizeClassName()]
                    = SampleChildType1.schema,
                    [typeof(SampleChildType2).ToString().SanitizeClassName()]
                    = SampleChildType2.schema,
                    [typeof(SampleBaseType2).ToString().SanitizeClassName()]
                    = SampleBaseType2.schema
                }
            };

            //Type with JObject Attribute
            yield return new object[]
            {
                typeof(SampleTypeWithJObjectAttribute),
                new OpenApiSchema
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.Schema,
                        Id = typeof(SampleTypeWithJObjectAttribute).ToString().SanitizeClassName()
                    }
                },
                new Dictionary<string, OpenApiSchema>
                {
                    [typeof(SampleTypeWithJObjectAttribute).ToString().SanitizeClassName()]
                    = SampleTypeWithJObjectAttribute.schema
                }
            };

        }
    }
}