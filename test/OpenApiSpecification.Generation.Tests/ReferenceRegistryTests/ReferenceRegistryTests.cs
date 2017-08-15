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
    public class ReferenceRegistryTests
    {
        private const string TestValidationDirectory = "ReferenceRegistryTests/TestValidation";

        [TestMethod]
        public void GenerateSchemaFromDictionaryTypeShouldSucceed()
        {
            var referenceRegistryManager = new ReferenceRegistryManager();

            var returnedSchema = referenceRegistryManager.FindOrAddSchemaReference(typeof(IDictionary<string, SampleType>));

            var actualSchema = JsonConvert.SerializeObject(
                returnedSchema,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()});

            var expectedSchema = File.ReadAllText(
                Path.Combine(
                    TestValidationDirectory,
                    "DictionaryTypeSchema.json"));

            Assert.IsTrue(TestHelper.AreJsonEqual(expectedSchema, actualSchema));

            var actualReferences = JsonConvert.SerializeObject(
                referenceRegistryManager.SchemaReferenceRegistry.References,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()});

            var expectedReferences = File.ReadAllText(
                Path.Combine(
                    TestValidationDirectory,
                    "NestedObjectTypeReferences.json"));

            Assert.IsTrue(TestHelper.AreJsonEqual(expectedReferences, actualReferences));
        }

        [TestMethod]
        public void GenerateSchemaFromEnumTypeShouldSucceed()
        {
            var referenceRegistryManager = new ReferenceRegistryManager();

            var returnedSchema = referenceRegistryManager.FindOrAddSchemaReference(typeof(SampleEnum));

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
        public void GenerateSchemaFromNestedObjectTypeShouldSucceed()
        {
            var referenceRegistryManager = new ReferenceRegistryManager();

            var returnedSchema = referenceRegistryManager.FindOrAddSchemaReference(typeof(SampleType));

            var actualSchema = JsonConvert.SerializeObject(
                returnedSchema,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()});

            var expectedSchema = File.ReadAllText(
                Path.Combine(
                    TestValidationDirectory,
                    "NestedObjectTypeSchema.json"));

            Assert.IsTrue(TestHelper.AreJsonEqual(expectedSchema, actualSchema));

            var actualReferences = JsonConvert.SerializeObject(
                referenceRegistryManager.SchemaReferenceRegistry.References,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()});

            var expectedReferences = File.ReadAllText(
                Path.Combine(
                    TestValidationDirectory,
                    "NestedObjectTypeReferences.json"));

            Assert.IsTrue(TestHelper.AreJsonEqual(expectedReferences, actualReferences));
        }

        [TestMethod]
        public void GenerateSchemaFromObjectListTypeShouldSucceed()
        {
            var referenceRegistryManager = new ReferenceRegistryManager();

            var returnedSchema = referenceRegistryManager.FindOrAddSchemaReference(typeof(IList<SampleType>));

            var actualSchema = JsonConvert.SerializeObject(
                returnedSchema,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()});

            var expectedSchema = File.ReadAllText(
                Path.Combine(
                    TestValidationDirectory,
                    "ObjectListTypeSchema.json"));

            Assert.IsTrue(TestHelper.AreJsonEqual(expectedSchema, actualSchema));

            var actualReferences = JsonConvert.SerializeObject(
                referenceRegistryManager.SchemaReferenceRegistry.References,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()});

            var expectedReferences = File.ReadAllText(
                Path.Combine(
                    TestValidationDirectory,
                    "NestedObjectTypeReferences.json"));

            Assert.IsTrue(TestHelper.AreJsonEqual(expectedReferences, actualReferences));
        }

        [TestMethod]
        public void GenerateSchemaFromObjectTypeShouldSucceed()
        {
            var referenceRegistryManager = new ReferenceRegistryManager();

            var returnedSchema = referenceRegistryManager.FindOrAddSchemaReference(typeof(SampleInnerType));

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
        public void GenerateSchemaFromSimpleTypeShouldSucceed()
        {
            var referenceRegistryManager = new ReferenceRegistryManager();

            var returnedSchema = referenceRegistryManager.FindOrAddSchemaReference(typeof(string));

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
            public Dictionary<string, SampleInnerType> SamplePropertyDictionaryStringObject { get; } = new Dictionary<string, SampleInnerType>();

            public Dictionary<string, string> SamplePropertyDictionaryStringString { get; } = new Dictionary<string, string>();

            public SampleEnum SamplePropertyEnum { get; set; }

            public IDictionary<string, SampleInnerType> SamplePropertyIDictionaryStringObject { get; } = new Dictionary<string, SampleInnerType>();

            public IDictionary<string, string> SamplePropertyIDictionaryStringString { get; } = new Dictionary<string, string>();

            public IList<SampleInnerType> SamplePropertyIListObject { get; } = new List<SampleInnerType>();

            public int SamplePropertyInt { get; set; }

            public IList<SampleEnum> SamplePropertyListEnum { get; } = new List<SampleEnum>();

            public List<SampleInnerType> SamplePropertyListObject { get; } = new List<SampleInnerType>();

            public IList<string> SamplePropertyListString { get; } = new List<string>();

            public SampleInnerType SamplePropertyObject { get; set; }

            public SampleInnerType SamplePropertyReadonlyObject { get; } = new SampleInnerType();

            public string SamplePropertyString { get; set; }
        }
    }
}