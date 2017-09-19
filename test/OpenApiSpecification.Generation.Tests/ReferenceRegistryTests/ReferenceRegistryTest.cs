// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Microsoft.OpenApiSpecification.Core.Models;
using Microsoft.OpenApiSpecification.Generation.ReferenceRegistries;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApiSpecification.Generation.Tests.ReferenceRegistryTests
{
    [Collection("DefaultSettings")]
    public class ReferenceRegistryTest
    {
        private const string SimpleTypeSubDirectory = "SimpleTypes";
        private const string TestValidationDirectory = "ReferenceRegistryTests/TestValidation";

        private readonly ITestOutputHelper _output;

        public ReferenceRegistryTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [MemberData(nameof(GetTestCasesForGenerateSchemaFromTypeShouldSucceed))]
        public void GenerateSchemaFromTypeShouldSucceed(
            Type type,
            string expectedSchemaFileName,
            string expectedReferencesFileName)
        {
            _output.WriteLine(type.ToString());

            // Arrange
            var referenceRegistryManager = new ReferenceRegistryManager();

            // Act
            var returnedSchema =
                referenceRegistryManager.FindOrAddSchemaReference(type);

            // Assert
            var actualSchema = JsonConvert.SerializeObject(
                returnedSchema);

            var expectedSchema = File.ReadAllText(
                expectedSchemaFileName);

            var actualReferences = JsonConvert.SerializeObject(
                referenceRegistryManager.SchemaReferenceRegistry.References);

            var expectedReferences = File.ReadAllText(
                expectedReferencesFileName);

            _output.WriteLine(actualSchema);
            _output.WriteLine(actualReferences);

            JsonConvert.DeserializeObject<Schema>(actualSchema)
                .Should()
                .BeEquivalentTo(JsonConvert.DeserializeObject<Schema>(expectedSchema));

            JsonConvert.DeserializeObject<IDictionary<string, Schema>>(actualReferences)
                .Should()
                .BeEquivalentTo(
                    JsonConvert.DeserializeObject<IDictionary<string, Schema>>(expectedReferences));
        }

        private static IEnumerable<object[]> GetTestCasesForGenerateSchemaFromTypeShouldSucceed()
        {
            // Simple types

            yield return new object[]
            {
                typeof(bool),
                Path.Combine(TestValidationDirectory, SimpleTypeSubDirectory, "BoolTypeSchema.json"),
                Path.Combine(TestValidationDirectory, SimpleTypeSubDirectory, "EmptyTypeReferences.json")
            };
            yield return new object[]
            {
                typeof(byte),
                Path.Combine(TestValidationDirectory, SimpleTypeSubDirectory, "ByteTypeSchema.json"),
                Path.Combine(TestValidationDirectory, SimpleTypeSubDirectory, "EmptyTypeReferences.json")
            };
            yield return new object[]
            {
                typeof(char),
                Path.Combine(TestValidationDirectory, SimpleTypeSubDirectory, "CharTypeSchema.json"),
                Path.Combine(TestValidationDirectory, SimpleTypeSubDirectory, "EmptyTypeReferences.json")
            };
            yield return new object[]
            {
                typeof(string),
                Path.Combine(TestValidationDirectory, SimpleTypeSubDirectory, "StringTypeSchema.json"),
                Path.Combine(TestValidationDirectory, SimpleTypeSubDirectory, "EmptyTypeReferences.json")
            };
            yield return new object[]
            {
                typeof(int),
                Path.Combine(TestValidationDirectory, SimpleTypeSubDirectory, "IntTypeSchema.json"),
                Path.Combine(TestValidationDirectory, SimpleTypeSubDirectory, "EmptyTypeReferences.json")
            };
            yield return new object[]
            {
                typeof(Guid),
                Path.Combine(TestValidationDirectory, SimpleTypeSubDirectory, "GuidTypeSchema.json"),
                Path.Combine(TestValidationDirectory, SimpleTypeSubDirectory, "EmptyTypeReferences.json")
            };
            yield return new object[]
            {
                typeof(DateTimeOffset),
                Path.Combine(TestValidationDirectory, SimpleTypeSubDirectory, "DateTimeTypeSchema.json"),
                Path.Combine(TestValidationDirectory, SimpleTypeSubDirectory, "EmptyTypeReferences.json")
            };

            // Enum types
            yield return new object[]
            {
                typeof(SampleEnum),
                Path.Combine(TestValidationDirectory, "EnumTypeSchema.json"),
                Path.Combine(TestValidationDirectory, "EmptyTypeReferences.json")
            };

            // Object types
            yield return new object[]
            {
                typeof(SampleInnerType),
                Path.Combine(TestValidationDirectory, "ObjectTypeSchema.json"),
                Path.Combine(TestValidationDirectory, "InnerObjectTypeReferences.json")
            };
            yield return new object[]
            {
                typeof(SampleType),
                Path.Combine(TestValidationDirectory, "NestedObjectTypeSchema.json"),
                Path.Combine(TestValidationDirectory, "NestedObjectTypeReferences.json")
            };
            yield return new object[]
            {
                typeof(SampleSelfReferencingType),
                Path.Combine(TestValidationDirectory, "SelfReferencingTypeSchema.json"),
                Path.Combine(TestValidationDirectory, "SelfReferencingTypeReferences.json")
            };

            // Enumerable types
            yield return new object[]
            {
                typeof(IList<SampleType>),
                Path.Combine(TestValidationDirectory, "ObjectListTypeSchema.json"),
                Path.Combine(TestValidationDirectory, "NestedObjectTypeReferences.json")
            };
            yield return new object[]
            {
                typeof(IDictionary<string, SampleType>),
                Path.Combine(TestValidationDirectory, "DictionaryTypeSchema.json"),
                Path.Combine(TestValidationDirectory, "NestedObjectTypeReferences.json")
            };

            // Generic types
            yield return new object[]
            {
                typeof(SampleGenericType<string>),
                Path.Combine(TestValidationDirectory, "GenericTypeSchema.json"),
                Path.Combine(TestValidationDirectory, "GenericTypeReferences.json")
            };
            yield return new object[]
            {
                typeof(ISampleGenericType<string, SampleType>),
                Path.Combine(TestValidationDirectory, "GenericInterfaceSchema.json"),
                Path.Combine(TestValidationDirectory, "GenericInterfaceReferences.json")
            };
            yield return new object[]
            {
                typeof(
                    ISampleGenericType<
                        ISampleGenericType<
                            string,
                            ISampleGenericType<string, SampleType>>,
                        SampleType>),
                Path.Combine(TestValidationDirectory, "NestedGenericSchema.json"),
                Path.Combine(TestValidationDirectory, "NestedGenericReferences.json")
            };
            yield return new object[]
            {
                typeof(SampleInheritFromGenericType),
                Path.Combine(TestValidationDirectory, "InheritFromGenericInterfaceSchema.json"),
                Path.Combine(TestValidationDirectory, "InheritFromGenericInterfaceReferences.json")
            };
        }

        internal enum SampleEnum
        {
            SampleEnumValueFirst = 0,
            SampleEnumValueSecond = 1
        }

        internal class SampleInnerType
        {
            public SampleEnum SamplePropertyInnerEnum { get; set; }

            public IList<int> SamplePropertyInnerListInt { get; } = new List<int>();

            public string SamplePropertyInnerString { get; set; }
        }

        internal class SampleType
        {
            public Dictionary<string, SampleInnerType> SamplePropertyDictionaryStringObject { get; } =
                new Dictionary<string, SampleInnerType>();

            public Dictionary<string, string> SamplePropertyDictionaryStringString { get; } =
                new Dictionary<string, string>();

            public SampleEnum SamplePropertyEnum { get; set; }

            public IDictionary<string, SampleInnerType> SamplePropertyIDictionaryStringObject { get; } =
                new Dictionary<string, SampleInnerType>();

            public IDictionary<string, string> SamplePropertyIDictionaryStringString { get; } =
                new Dictionary<string, string>();

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

        internal class SampleSelfReferencingType
        {
            public IList<SampleSelfReferencingType> SampleSelfReferencingListProperty { get; set; }

            public SampleSelfReferencingType SampleSelfReferencingProperty { get; set; }
        }

        internal class SampleGenericType<T>
        {
            public T SamplePropertyGeneric { get; set; }

            public string SamplePropertyString { get; set; }
        }

        internal interface ISampleGenericType<T1, T2>
        {
            T1 SamplePropertyGenericTypeT1 { get; set; }

            T2 SamplePropertyGenericTypeT2 { get; set; }

            string SamplePropertyString { get; set; }
        }

        internal class SampleInheritFromGenericType : ISampleGenericType<SampleType, SampleInnerType>
        {
            public SampleType SamplePropertyGenericTypeT1 { get; set; }

            public SampleInnerType SamplePropertyGenericTypeT2 { get; set; }

            public string SamplePropertyString { get; set; }
        }
    }
}