// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.OpenApiSpecification.Core.Serialization;
using Microsoft.OpenApiSpecification.Generation.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Microsoft.OpenApiSpecification.Generation.Tests
{
    [TestClass]
    public class InternalOpenApiDocumentGeneratorTests
    {
        private const string TestFilesDirectory = "TestFiles";
        private const string TestValidationDirectory = "TestValidation";

        [TestMethod]
        public void GenerateV3DocumentShouldSucceed()
        {
            var path = Path.Combine(TestFilesDirectory, "Annotation.xml");
            var document = XDocument.Load(path);

            var generator = new OpenApiDocumentGenerator();

            var result = generator.GenerateV3Document(document, new List<string>());

            Assert.IsNotNull(result);
            Assert.IsTrue(result.GenerationStatus == GenerationStatus.Success);
            Assert.IsNotNull(result.OpenApiSpecificationV3Document);
            Assert.AreEqual(7, result.PathGenerationResults.Count);

            var actualDocument = JsonConvert.SerializeObject(
                result.OpenApiSpecificationV3Document,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()}
            );

            var expectedDocument = File.ReadAllText(Path.Combine(TestValidationDirectory, "Success.Json"));

            Assert.IsTrue(TestHelper.AreJsonEqual(expectedDocument, actualDocument));
        }

        [TestMethod]
        public void InvalidUriShouldFailGeneration()
        {
            var path = Path.Combine(TestFilesDirectory, "AnnotationInvalidUri.xml");

            var document = XDocument.Load(path);

            var generator = new OpenApiDocumentGenerator();

            var result = generator.GenerateV3Document(document, new List<string>());

            Assert.IsNotNull(result);
            Assert.IsTrue(result.GenerationStatus == GenerationStatus.Failure);
            Assert.IsNotNull(result.OpenApiSpecificationV3Document);
            Assert.AreEqual(7, result.PathGenerationResults.Count);

            var failedPaths = result.PathGenerationResults.Where(p => p.Status == GenerationStatus.Failure);

            Assert.IsTrue(failedPaths.Count() == 1);
            Assert.IsTrue(failedPaths.First().Path == "http://{host}/V1/entities");
            Assert.IsTrue(
                failedPaths.First().Message ==
                string.Format(SpecificationGenerationMessages.InvalidUrl, "http://{host}/V1/entities"));

            var actualDocument = JsonConvert.SerializeObject(
                result.OpenApiSpecificationV3Document,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()}
            );

            var expectedDocument = File.ReadAllText(
                Path.Combine(
                    TestValidationDirectory,
                    "AnnotationInvalidUri.Json"));

            Assert.IsTrue(TestHelper.AreJsonEqual(expectedDocument, actualDocument));
        }

        [TestMethod]
        public void InvalidVerbShouldFailGeneration()
        {
            var path = Path.Combine(TestFilesDirectory, "AnnotationInvalidVerb.xml");

            var document = XDocument.Load(path);

            var generator = new OpenApiDocumentGenerator();

            var result = generator.GenerateV3Document(document, new List<string>());

            Assert.IsNotNull(result);
            Assert.IsTrue(result.GenerationStatus == GenerationStatus.Failure);
            Assert.IsNotNull(result.OpenApiSpecificationV3Document);
            Assert.AreEqual(7, result.PathGenerationResults.Count);

            var failedPaths = result.PathGenerationResults.Where(p => p.Status == GenerationStatus.Failure);

            Assert.IsTrue(failedPaths.Count() == 1);
            Assert.IsTrue(failedPaths.First().Path == "/V1/entities/{id}");
            Assert.IsTrue(
                failedPaths.First().Message ==
                string.Format(SpecificationGenerationMessages.InvalidHttpMethod, "Invalid"));

            var actualDocument = JsonConvert.SerializeObject(
                result.OpenApiSpecificationV3Document,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()}
            );

            var expectedDocument = File.ReadAllText(
                Path.Combine(
                    TestValidationDirectory,
                    "AnnotationInvalidVerb.Json"));

            Assert.IsTrue(TestHelper.AreJsonEqual(expectedDocument, actualDocument));
        }

        [TestMethod]
        public void NoOperationsToParseShouldReturnEmptyDocument()
        {
            var path = Path.Combine(TestFilesDirectory, "AnnotationNoOperationsToParse.xml");

            var document = XDocument.Load(path);

            var generator = new OpenApiDocumentGenerator();

            var result = generator.GenerateV3Document(document, new List<string>());

            Assert.IsNotNull(result);
            Assert.IsTrue(result.GenerationStatus == GenerationStatus.Success);
            Assert.IsNull(result.OpenApiSpecificationV3Document);
            Assert.IsNotNull(result.PathGenerationResults);
            Assert.IsTrue(result.PathGenerationResults.Count == 1);
            Assert.IsNull(result.PathGenerationResults.First().Path);
            Assert.IsTrue(result.PathGenerationResults.First().Status == GenerationStatus.Success);
            Assert.IsTrue(
                result.PathGenerationResults.First().Message ==
                SpecificationGenerationMessages.NoOperationElementFoundToParse);
        }

        [TestMethod]
        public void ParamNoTypeSpecifiedShouldDefaultToStringType()
        {
            // Path param id doesn't have type documented in the annotation xml.
            var path = Path.Combine(TestFilesDirectory, "AnnotationParamNoTypeSpecified.xml");
            var document = XDocument.Load(path);
            var generator = new OpenApiDocumentGenerator();
            var result = generator.GenerateV3Document(document, new List<string>());

            Assert.IsNotNull(result);
            Assert.IsTrue(result.GenerationStatus == GenerationStatus.Success);
            Assert.IsNotNull(result.OpenApiSpecificationV3Document);
            Assert.AreEqual(1, result.PathGenerationResults.Count);

            var failedPaths = result.PathGenerationResults.Where(p => p.Status == GenerationStatus.Failure);

            Assert.IsTrue(failedPaths.Count() == 0);

            var actualDocument = JsonConvert.SerializeObject(
                result.OpenApiSpecificationV3Document,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()}
            );

            var expectedDocument = File.ReadAllText(
                Path.Combine(
                    TestValidationDirectory,
                    "AnnotationParamNoTypeSpecified.Json"));

            Assert.IsTrue(TestHelper.AreJsonEqual(expectedDocument, actualDocument));
        }

        [TestMethod]
        public void UndocumentedPathParametersShouldFailGeneration()
        {
            var path = Path.Combine(TestFilesDirectory, "AnnotationUndocumentedPathParameters.xml");
            var document = XDocument.Load(path);
            var generator = new OpenApiDocumentGenerator();
            var result = generator.GenerateV3Document(document, new List<string>());

            Assert.IsNotNull(result);
            Assert.IsTrue(result.GenerationStatus == GenerationStatus.Failure);
            Assert.IsNotNull(result.OpenApiSpecificationV3Document);
            Assert.AreEqual(7, result.PathGenerationResults.Count);

            var failedPaths = result.PathGenerationResults.Where(p => p.Status == GenerationStatus.Failure);

            Assert.IsTrue(failedPaths.Count() == 1);
            Assert.IsTrue(failedPaths.First().Path == "/V1/entities/{id}");
            Assert.IsTrue(
                failedPaths.First().Message ==
                string.Format(SpecificationGenerationMessages.UndocumentedPathParameter, "id", "/V1/entities/{id}"));

            var actualDocument = JsonConvert.SerializeObject(
                result.OpenApiSpecificationV3Document,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()}
            );

            var expectedDocument = File.ReadAllText(
                Path.Combine(
                    TestValidationDirectory,
                    "AnnotationUndocumentedPathParameters.Json"));

            Assert.IsTrue(TestHelper.AreJsonEqual(expectedDocument, actualDocument));
        }
    }
}