// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using Microsoft.OpenApiSpecification.Core.Serialization;
using Microsoft.OpenApiSpecification.Generation.ReferenceRegistries;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Microsoft.OpenApiSpecification.Generation.Tests.ReferenceRegistryTests
{
    [TestClass]
    public class ReferenceRegistryTest
    {
        private const string TestValidationDirectory = "ReferenceRegistryTests/TestValidation";

        [TestMethod]
        public void GenerateSchemaFromDictionaryTypeShouldSucceed()
        {
            // Arrange
            var referenceRegistryManager = new ReferenceRegistryManager();

            // Act
            var returnedSchema =
                referenceRegistryManager.FindOrAddSchemaReference(typeof(IDictionary<string, SampleType>));

            // Assert
            var actualSchema = JsonConvert.SerializeObject(
                returnedSchema,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()});

            var expectedSchema = File.ReadAllText(
                Path.Combine(
                    TestValidationDirectory,
                    "DictionaryTypeSchema.json"));

            var actualReferences = JsonConvert.SerializeObject(
                referenceRegistryManager.SchemaReferenceRegistry.References,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()});

            var expectedReferences = File.ReadAllText(
                Path.Combine(
                    TestValidationDirectory,
                    "NestedObjectTypeReferences.json"));

            Assert.IsTrue(TestHelper.AreJsonEqual(expectedSchema, actualSchema));
            Assert.IsTrue(TestHelper.AreJsonEqual(expectedReferences, actualReferences));
        }

        [TestMethod]
        public void GenerateSchemaFromEnumTypeShouldSucceed()
        {
            // Arrange
            var referenceRegistryManager = new ReferenceRegistryManager();

            // Act
            var returnedSchema = referenceRegistryManager.FindOrAddSchemaReference(typeof(SampleEnum));

            // Assert
            var actualSchema = JsonConvert.SerializeObject(
                returnedSchema,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()});

            var expectedSchema = File.ReadAllText(
                Path.Combine(
                    TestValidationDirectory,
                    "EnumTypeSchema.json"));

            Assert.IsTrue(TestHelper.AreJsonEqual(expectedSchema, actualSchema));

            var actualReferences = JsonConvert.SerializeObject(
                referenceRegistryManager.SchemaReferenceRegistry.References,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()});

            var expectedReferences = File.ReadAllText(
                Path.Combine(
                    TestValidationDirectory,
                    "EmptyTypeReferences.json"));

            Assert.IsTrue(TestHelper.AreJsonEqual(expectedReferences, actualReferences));
        }

        [TestMethod]
        public void GenerateSchemaFromGenericInterfaceShouldSucceed()
        {
            var referenceRegistryManager = new ReferenceRegistryManager();

            var returnedSchema = referenceRegistryManager.FindOrAddSchemaReference(typeof(ISampleGenericType<string>));

            var actualSchema = JsonConvert.SerializeObject(
                returnedSchema,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()});

            var expectedSchema = File.ReadAllText(
                Path.Combine(
                    TestValidationDirectory,
                    "GenericInterfaceSchema.json"));

            var actualReferences = JsonConvert.SerializeObject(
                referenceRegistryManager.SchemaReferenceRegistry.References,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()});

            var expectedReferences = File.ReadAllText(
                Path.Combine(
                    TestValidationDirectory,
                    "GenericInterfaceReferences.json"));

            Assert.IsTrue(TestHelper.AreJsonEqual(expectedSchema, actualSchema));
            Assert.IsTrue(TestHelper.AreJsonEqual(expectedReferences, actualReferences));
        }

        [TestMethod]
        public void GenerateSchemaFromGenericTypeShouldSucceed()
        {
            var referenceRegistryManager = new ReferenceRegistryManager();

            var returnedSchema = referenceRegistryManager.FindOrAddSchemaReference(typeof(SampleGenericType<string>));

            var actualSchema = JsonConvert.SerializeObject(
                returnedSchema,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()});

            var expectedSchema = File.ReadAllText(
                Path.Combine(
                    TestValidationDirectory,
                    "GenericTypeSchema.json"));

            var actualReferences = JsonConvert.SerializeObject(
                referenceRegistryManager.SchemaReferenceRegistry.References,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()});

            var expectedReferences = File.ReadAllText(
                Path.Combine(
                    TestValidationDirectory,
                    "GenericTypeReferences.json"));

            Assert.IsTrue(TestHelper.AreJsonEqual(expectedSchema, actualSchema));
            Assert.IsTrue(TestHelper.AreJsonEqual(expectedReferences, actualReferences));
        }

        [TestMethod]
        public void GenerateSchemaFromNestedObjectTypeShouldSucceed()
        {
            // Arrange
            var referenceRegistryManager = new ReferenceRegistryManager();

            // Act
            var returnedSchema = referenceRegistryManager.FindOrAddSchemaReference(typeof(SampleType));

            // Assert
            var actualSchema = JsonConvert.SerializeObject(
                returnedSchema,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()});

            var expectedSchema = File.ReadAllText(
                Path.Combine(
                    TestValidationDirectory,
                    "NestedObjectTypeSchema.json"));

            var actualReferences = JsonConvert.SerializeObject(
                referenceRegistryManager.SchemaReferenceRegistry.References,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()});

            var expectedReferences = File.ReadAllText(
                Path.Combine(
                    TestValidationDirectory,
                    "NestedObjectTypeReferences.json"));

            Assert.IsTrue(TestHelper.AreJsonEqual(expectedSchema, actualSchema));
            Assert.IsTrue(TestHelper.AreJsonEqual(expectedReferences, actualReferences));
        }

        [TestMethod]
        public void GenerateSchemaFromObjectListTypeShouldSucceed()
        {
            // Arrange
            var referenceRegistryManager = new ReferenceRegistryManager();

            // Act
            var returnedSchema = referenceRegistryManager.FindOrAddSchemaReference(typeof(IList<SampleType>));

            // Assert
            var actualSchema = JsonConvert.SerializeObject(
                returnedSchema,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()});

            var expectedSchema = File.ReadAllText(
                Path.Combine(
                    TestValidationDirectory,
                    "ObjectListTypeSchema.json"));

            var actualReferences = JsonConvert.SerializeObject(
                referenceRegistryManager.SchemaReferenceRegistry.References,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()});

            var expectedReferences = File.ReadAllText(
                Path.Combine(
                    TestValidationDirectory,
                    "NestedObjectTypeReferences.json"));

            Assert.IsTrue(TestHelper.AreJsonEqual(expectedSchema, actualSchema));
            Assert.IsTrue(TestHelper.AreJsonEqual(expectedReferences, actualReferences));
        }

        [TestMethod]
        public void GenerateSchemaFromObjectTypeShouldSucceed()
        {
            // Arrange
            var referenceRegistryManager = new ReferenceRegistryManager();

            // Act
            var returnedSchema = referenceRegistryManager.FindOrAddSchemaReference(typeof(SampleInnerType));

            // Assert
            var actualSchema = JsonConvert.SerializeObject(
                returnedSchema,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()});

            var expectedSchema = File.ReadAllText(
                Path.Combine(
                    TestValidationDirectory,
                    "ObjectTypeSchema.json"));

            Assert.IsTrue(TestHelper.AreJsonEqual(expectedSchema, actualSchema));

            var actualReferences = JsonConvert.SerializeObject(
                referenceRegistryManager.SchemaReferenceRegistry.References,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()});

            var expectedReferences = File.ReadAllText(
                Path.Combine(
                    TestValidationDirectory,
                    "InnerObjectTypeReferences.json"));

            Assert.IsTrue(TestHelper.AreJsonEqual(expectedReferences, actualReferences));
        }

        [TestMethod]
        public void GenerateSchemaFromSelfReferencingTypeShouldSucceed()
        {
            var referenceRegistryManager = new ReferenceRegistryManager();

            var returnedSchema = referenceRegistryManager.FindOrAddSchemaReference(typeof(SampleSelfReferencingType));

            var actualSchema = JsonConvert.SerializeObject(
                returnedSchema,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()});

            var expectedSchema = File.ReadAllText(
                Path.Combine(
                    TestValidationDirectory,
                    "SelfReferencingTypeSchema.json"));

            var actualReferences = JsonConvert.SerializeObject(
                referenceRegistryManager.SchemaReferenceRegistry.References,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()});

            var expectedReferences = File.ReadAllText(
                Path.Combine(
                    TestValidationDirectory,
                    "SelfReferencingTypeReferences.json"));

            Assert.IsTrue(TestHelper.AreJsonEqual(expectedSchema, actualSchema));
            Assert.IsTrue(TestHelper.AreJsonEqual(expectedReferences, actualReferences));
        }

        [TestMethod]
        public void GenerateSchemaFromSimpleTypeShouldSucceed()
        {
            // Arrange
            var referenceRegistryManager = new ReferenceRegistryManager();

            // Act
            var returnedSchema = referenceRegistryManager.FindOrAddSchemaReference(typeof(string));

            // Assert
            var actualSchema = JsonConvert.SerializeObject(
                returnedSchema,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()});

            var expectedSchema = File.ReadAllText(
                Path.Combine(
                    TestValidationDirectory,
                    "SimpleTypeSchema.json"));

            Assert.IsTrue(TestHelper.AreJsonEqual(expectedSchema, actualSchema));

            var actualReferences = JsonConvert.SerializeObject(
                referenceRegistryManager.SchemaReferenceRegistry.References,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()});

            var expectedReferences = File.ReadAllText(
                Path.Combine(
                    TestValidationDirectory,
                    "EmptyTypeReferences.json"));

            Assert.IsTrue(TestHelper.AreJsonEqual(expectedReferences, actualReferences));
        }

        [TestMethod]
        public void GenerateSchemaFromTypeInheritedFromGenericInterfaceShouldSucceed()
        {
            var referenceRegistryManager = new ReferenceRegistryManager();

            var returnedSchema = referenceRegistryManager.FindOrAddSchemaReference(typeof(SampleInheritFromGenericType));

            var actualSchema = JsonConvert.SerializeObject(
                returnedSchema,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()});

            var expectedSchema = File.ReadAllText(
                Path.Combine(
                    TestValidationDirectory,
                    "InheritFromGenericInterfaceSchema.json"));

            var actualReferences = JsonConvert.SerializeObject(
                referenceRegistryManager.SchemaReferenceRegistry.References,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()});

            var expectedReferences = File.ReadAllText(
                Path.Combine(
                    TestValidationDirectory,
                    "InheritFromGenericInterfaceReferences.json"));

            Assert.IsTrue(TestHelper.AreJsonEqual(expectedSchema, actualSchema));
            Assert.IsTrue(TestHelper.AreJsonEqual(expectedReferences, actualReferences));
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

        internal interface ISampleGenericType<T>
        {
            T SamplePropertyGeneric { get; set; }

            string SamplePropertyString { get; set; }
        }

        internal class SampleInheritFromGenericType : ISampleGenericType<SampleType>
        {
            public SampleType SamplePropertyGeneric { get; set; }

            public string SamplePropertyString { get; set; }
        }
    }
}