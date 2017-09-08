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
        public void DuplicateResponseCodeShouldPassGeneration()
        {
            var path = Path.Combine(TestFilesDirectory, "AnnotationWithDuplicateResponseTag.xml");
            var document = XDocument.Load(path);
            var generator = new OpenApiDocumentGenerator();

            var result = generator.GenerateV3Documents(
                document,
                new List<string> {Path.Combine(TestFilesDirectory, "NativeXml.exe")});

            Assert.IsNotNull(result);

            Assert.IsTrue(result.GenerationStatus == GenerationStatus.Success);
            Assert.IsNotNull(result.MainDocument);
            Assert.AreEqual(1, result.PathGenerationResults.Count);

            var failedPaths = result.PathGenerationResults.Where(p => p.GenerationStatus == GenerationStatus.Failure);

            Assert.IsFalse(failedPaths.Any());

            var actualDocument = JsonConvert.SerializeObject(
                result.MainDocument,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()}
            );

            var expectedDocument = File.ReadAllText(
                Path.Combine(
                    TestValidationDirectory,
                    "AnnotationWithDuplicateResponseTag.Json"));

            Assert.IsTrue(TestHelper.AreJsonEqual(expectedDocument, actualDocument));
        }

        [TestMethod]
        public void GenerateV3DocumentShouldSucceed()
        {
            var path = Path.Combine(TestFilesDirectory, "Annotation.xml");
            var document = XDocument.Load(path);

            var generator = new OpenApiDocumentGenerator();

            var result = generator.GenerateV3Documents(
                document,
                new List<string> {Path.Combine(TestFilesDirectory, "NativeXml.exe")});

            Assert.IsNotNull(result);
            Assert.IsTrue(result.GenerationStatus == GenerationStatus.Success);
            Assert.IsNotNull(result.MainDocument);
            Assert.AreEqual(7, result.PathGenerationResults.Count);

            var actualDocument = JsonConvert.SerializeObject(
                result.MainDocument,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()}
            );

            var expectedDocument =
                File.ReadAllText(Path.Combine(TestValidationDirectory, "GenerationSucceed.Json"));

            Assert.IsTrue(TestHelper.AreJsonEqual(expectedDocument, actualDocument));
        }

        [TestMethod]
        public void InvalidUriShouldFailGeneration()
        {
            var path = Path.Combine(TestFilesDirectory, "AnnotationInvalidUri.xml");

            var document = XDocument.Load(path);

            var generator = new OpenApiDocumentGenerator();

            var result = generator.GenerateV3Documents(
                document,
                new List<string> {Path.Combine(TestFilesDirectory, "NativeXml.exe")});

            Assert.IsNotNull(result);
            Assert.IsTrue(result.GenerationStatus == GenerationStatus.Failure);
            Assert.IsNotNull(result.MainDocument);
            Assert.AreEqual(7, result.PathGenerationResults.Count);

            var failedPaths = result.PathGenerationResults.Where(p => p.GenerationStatus == GenerationStatus.Failure);

            Assert.IsTrue(failedPaths.Count() == 1);
            Assert.IsTrue(failedPaths.First().Path == "http://{host}/V1/entities");
            Assert.IsTrue(
                failedPaths.First().Message ==
                string.Format(SpecificationGenerationMessages.InvalidUrl, "http://{host}/V1/entities"));

            var actualDocument = JsonConvert.SerializeObject(
                result.MainDocument,
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

            var result = generator.GenerateV3Documents(
                document,
                new List<string> {Path.Combine(TestFilesDirectory, "NativeXml.exe")});

            Assert.IsNotNull(result);
            Assert.IsTrue(result.GenerationStatus == GenerationStatus.Failure);
            Assert.IsNotNull(result.MainDocument);
            Assert.AreEqual(7, result.PathGenerationResults.Count);

            var failedPaths = result.PathGenerationResults.Where(p => p.GenerationStatus == GenerationStatus.Failure);

            var actualDocument = JsonConvert.SerializeObject(
                result.MainDocument,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()}
            );

            var expectedDocument = File.ReadAllText(
                Path.Combine(
                    TestValidationDirectory,
                    "AnnotationInvalidVerb.Json"));

            Assert.IsTrue(failedPaths.Count() == 1);
            Assert.IsTrue(failedPaths.First().Path == "/V1/entities/{id}");
            Assert.IsTrue(
                failedPaths.First().Message ==
                string.Format(SpecificationGenerationMessages.InvalidHttpMethod, "Invalid"));

            Assert.IsTrue(TestHelper.AreJsonEqual(expectedDocument, actualDocument));
        }

        [TestMethod]
        public void NoOperationsToParseShouldReturnEmptyDocument()
        {
            var path = Path.Combine(TestFilesDirectory, "AnnotationNoOperationsToParse.xml");

            var document = XDocument.Load(path);

            var generator = new OpenApiDocumentGenerator();

            var result = generator.GenerateV3Documents(document, new List<string>());

            Assert.IsNotNull(result);
            Assert.IsTrue(result.GenerationStatus == GenerationStatus.Success);
            Assert.IsNull(result.MainDocument);
            Assert.IsNotNull(result.PathGenerationResults);
            Assert.IsTrue(result.PathGenerationResults.Count == 1);
            Assert.IsNull(result.PathGenerationResults.First().Path);
            Assert.IsTrue(result.PathGenerationResults.First().GenerationStatus == GenerationStatus.Success);
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
            var result = generator.GenerateV3Documents(
                document,
                new List<string> {Path.Combine(TestFilesDirectory, "NativeXml.exe")});

            Assert.IsNotNull(result);
            Assert.AreEqual(GenerationStatus.Success, result.GenerationStatus);
            Assert.IsNotNull(result.MainDocument);
            Assert.AreEqual(1, result.PathGenerationResults.Count);

            var failedPaths = result.PathGenerationResults.Where(p => p.GenerationStatus == GenerationStatus.Failure)
                .ToList();

            var actualDocument = JsonConvert.SerializeObject(
                result.MainDocument,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()}
            );

            var expectedDocument = File.ReadAllText(
                Path.Combine(
                    TestValidationDirectory,
                    "AnnotationParamNoTypeSpecified.Json"));

            Assert.AreEqual(0, failedPaths.Count);
            Assert.IsTrue(TestHelper.AreJsonEqual(expectedDocument, actualDocument));
        }

        [TestMethod]
        public void UndocumentedGenericShouldFailGeneration()
        {
            var path = Path.Combine(TestFilesDirectory, "AnnotationWithUndocumentedGeneric.xml");
            var document = XDocument.Load(path);
            var generator = new OpenApiDocumentGenerator();
            var result = generator.GenerateV3Documents(
                document,
                new List<string> {Path.Combine(TestFilesDirectory, "NativeXml.exe")});

            Assert.IsNotNull(result);
            Assert.AreEqual(GenerationStatus.Failure, result.GenerationStatus);
            Assert.IsNull(result.MainDocument);
            Assert.AreEqual(1, result.PathGenerationResults.Count);

            var failedPaths = result.PathGenerationResults.Where(p => p.GenerationStatus == GenerationStatus.Failure)
                .ToList();

            Assert.AreEqual(1, failedPaths.Count);
            Assert.AreEqual(SpecificationGenerationMessages.UndocumentedGenericType, failedPaths.First().Message);
        }

        [TestMethod]
        public void UndocumentedPathParametersShouldFailGeneration()
        {
            var path = Path.Combine(TestFilesDirectory, "AnnotationUndocumentedPathParameters.xml");
            var document = XDocument.Load(path);
            var generator = new OpenApiDocumentGenerator();
            var result = generator.GenerateV3Documents(
                document,
                new List<string> {Path.Combine(TestFilesDirectory, "NativeXml.exe")});

            Assert.IsNotNull(result);
            Assert.AreEqual(GenerationStatus.Failure, result.GenerationStatus);
            Assert.IsNotNull(result.MainDocument);
            Assert.AreEqual(7, result.PathGenerationResults.Count);

            var failedPaths = result.PathGenerationResults.Where(p => p.GenerationStatus == GenerationStatus.Failure)
                .ToList();

            var actualDocument = JsonConvert.SerializeObject(
                result.MainDocument,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()}
            );

            var expectedDocument = File.ReadAllText(
                Path.Combine(
                    TestValidationDirectory,
                    "AnnotationUndocumentedPathParameters.Json"));

            Assert.AreEqual(1, failedPaths.Count);
            Assert.AreEqual("/V1/entities/{id}", failedPaths.First().Path);
            Assert.AreEqual(
                string.Format(SpecificationGenerationMessages.UndocumentedPathParameter, "id", "/V1/entities/{id}"),
                failedPaths.First().Message);

            Assert.IsTrue(TestHelper.AreJsonEqual(expectedDocument, actualDocument));
        }

        [TestMethod]
        public void UnorderedGenericShouldFailGeneration()
        {
            var path = Path.Combine(TestFilesDirectory, "AnnotationWithUnorderedGeneric.xml");
            var document = XDocument.Load(path);
            var generator = new OpenApiDocumentGenerator();
            var result = generator.GenerateV3Documents(document, new List<string>());

            Assert.IsNotNull(result);
            Assert.AreEqual(GenerationStatus.Failure, result.GenerationStatus);
            Assert.IsNull(result.MainDocument);
            Assert.AreEqual(1, result.PathGenerationResults.Count);

            var failedPaths = result.PathGenerationResults.Where(p => p.GenerationStatus == GenerationStatus.Failure)
                .ToList();

            Assert.AreEqual(1, failedPaths.Count());
            Assert.AreEqual(SpecificationGenerationMessages.UnorderedGenericType, failedPaths.First().Message);
        }
    }
}