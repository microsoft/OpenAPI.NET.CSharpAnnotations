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
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApiSpecification.Generation.Tests
{
    public class InternalOpenApiDocumentGeneratorTest
    {
        private const string TestFilesDirectory = "InternalOpenApiDocumentGeneratorTests/TestFiles";
        private const string TestValidationDirectory = "InternalOpenApiDocumentGeneratorTests/TestValidation";

        private readonly ITestOutputHelper _output;

        public InternalOpenApiDocumentGeneratorTest(ITestOutputHelper output)
        {
            this._output = output;
        }

        private static IEnumerable<object[]> GetTestCasesForInvalidDocumentationShouldFailGeneration()
        {
            // Invalid Verb
            yield return new object[]
            {
                Path.Combine(TestFilesDirectory, "AnnotationInvalidVerb.xml"),
                new List<string> {Path.Combine(TestFilesDirectory, "OpenApiSpecification.UnitTestSamples.DotNetFrameworkController.dll")},
                7,
                Path.Combine(
                    TestValidationDirectory,
                    "AnnotationInvalidVerb.Json"),
                new List<PathGenerationResult>
                {
                    new PathGenerationResult(
                        "/V1/samples/{id}",
                        string.Format(SpecificationGenerationMessages.InvalidHttpMethod, "Invalid"),
                        GenerationStatus.Failure)
                }
            };

            // Invalid Uri
            yield return new object[]
            {
                Path.Combine(TestFilesDirectory, "AnnotationInvalidUri.xml"),
                new List<string> {Path.Combine(TestFilesDirectory, "OpenApiSpecification.UnitTestSamples.DotNetFrameworkController.dll")},
                7,
                Path.Combine(
                    TestValidationDirectory,
                    "AnnotationInvalidUri.Json"),
                new List<PathGenerationResult>
                {
                    new PathGenerationResult(
                        "http://{host}:9000/V1/samples/{id}?queryBool={queryBool}",
                        string.Format(SpecificationGenerationMessages.InvalidUrl, "http://{host}:9000/V1/samples/{id}?queryBool={queryBool}"),
                        GenerationStatus.Failure)
                }
            };

            // Undocumented Path Parameters
            yield return new object[]
            {
                Path.Combine(TestFilesDirectory, "AnnotationUndocumentedPathParameters.xml"),
                new List<string> {Path.Combine(TestFilesDirectory, "OpenApiSpecification.UnitTestSamples.DotNetFrameworkController.dll")},
                7,
                Path.Combine(
                    TestValidationDirectory,
                    "AnnotationUndocumentedPathParameters.Json"),
                new List<PathGenerationResult>
                {
                    new PathGenerationResult(
                        "/V1/samples/{id}",
                        string.Format(SpecificationGenerationMessages.UndocumentedPathParameter, "id", "/V1/samples/{id}"),
                        GenerationStatus.Failure)
                }
            };

            // Undocumented Generics
            yield return new object[]
            {
                Path.Combine(TestFilesDirectory, "AnnotationUndocumentedGeneric.xml"),
                new List<string> {Path.Combine(TestFilesDirectory, "OpenApiSpecification.UnitTestSamples.DotNetFrameworkController.dll")},
                7,
                Path.Combine(
                    TestValidationDirectory,
                    "AnnotationUndocumentedGeneric.Json"),
                new List<PathGenerationResult>
                {
                    new PathGenerationResult(
                        "/V2/samples/",
                        SpecificationGenerationMessages.UndocumentedGenericType,
                        GenerationStatus.Failure),
                }
            };

            // Incorrect Order for Generics
            yield return new object[]
            {
                Path.Combine(TestFilesDirectory, "AnnotationIncorrectlyOrderedGeneric.xml"),
                new List<string> {Path.Combine(TestFilesDirectory, "OpenApiSpecification.UnitTestSamples.DotNetFrameworkController.dll")},
                7,
                Path.Combine(
                    TestValidationDirectory,
                    "AnnotationIncorrectlyOrderedGeneric.Json"),
                new List<PathGenerationResult>
                {
                    new PathGenerationResult(
                        "/V2/samples/",
                        SpecificationGenerationMessages.UnorderedGenericType,
                        GenerationStatus.Failure)
                }
            };
        }

        private static IEnumerable<object[]> GetTestCasesForValidDocumentationShouldPassGeneration()
        {
            // Standard, original valid XML document
            yield return new object[]
            {
                Path.Combine(TestFilesDirectory, "Annotation.xml"),
                new List<string> {Path.Combine(TestFilesDirectory, "OpenApiSpecification.UnitTestSamples.DotNetFrameworkController.dll")},
                7,
                Path.Combine(
                    TestValidationDirectory,
                    "Annotation.Json")
            };

            // Valid XML document but with one duplicate response tag.
            // The duplicate response tag should simply be ignored.
            yield return new object[]
            {
                Path.Combine(TestFilesDirectory, "AnnotationDuplicateResponseTag.xml"),
                new List<string> {Path.Combine(TestFilesDirectory, "OpenApiSpecification.UnitTestSamples.DotNetFrameworkController.dll")},
                7,
                Path.Combine(
                    TestValidationDirectory,
                    "AnnotationDuplicateResponseTag.Json")
            };

            // Valid XML document but with one parameter without specified type.
            // The type should simply default to string.
            yield return new object[]
            {
                Path.Combine(TestFilesDirectory, "AnnotationParamNoTypeSpecified.xml"),
                new List<string> {Path.Combine(TestFilesDirectory, "OpenApiSpecification.UnitTestSamples.DotNetFrameworkController.dll")},
                7,
                Path.Combine(
                    TestValidationDirectory,
                    "AnnotationParamNoTypeSpecified.Json")
            };
        }

        [Theory]
        [MemberData(nameof(GetTestCasesForInvalidDocumentationShouldFailGeneration))]
        public void InvalidDocumentationShouldFailGeneration(
            string inputXmlFile,
            IList<string> inputBinaryFiles,
            int expectedPathGenerationResultsCount,
            string expectedJsonFile,
            IEnumerable<PathGenerationResult> expectedFailedPathGenerationResults)
        {
            var document = XDocument.Load(inputXmlFile);

            var generator = new OpenApiDocumentGenerator();

            var result = generator.GenerateV3Documents(
                document,
                inputBinaryFiles);

            Assert.NotNull(result);
            Assert.Equal(GenerationStatus.Failure, result.GenerationStatus);
            Assert.NotNull(result.MainDocument);
            Assert.Equal(expectedPathGenerationResultsCount, result.PathGenerationResults.Count);

            var failedPaths = result.PathGenerationResults.Where(
                p => p.GenerationStatus == GenerationStatus.Failure);

            var actualDocument = JsonConvert.SerializeObject(
                result.MainDocument,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()}
            );

            var expectedDocument = File.ReadAllText(expectedJsonFile);

            Assert.Equal(expectedFailedPathGenerationResults, failedPaths);

            Assert.True(TestHelper.AreJsonEqual(expectedDocument, actualDocument));
        }

        [Fact]
        public void NoOperationsToParseShouldReturnEmptyDocument()
        {
            var path = Path.Combine(TestFilesDirectory, "AnnotationNoOperationsToParse.xml");

            var document = XDocument.Load(path);

            var generator = new OpenApiDocumentGenerator();

            var result = generator.GenerateV3Documents(document, new List<string>());

            Assert.NotNull(result);
            Assert.True(result.GenerationStatus == GenerationStatus.Success);
            Assert.Null(result.MainDocument);
            Assert.NotNull(result.PathGenerationResults);
            Assert.True(result.PathGenerationResults.Count == 1);
            Assert.Null(result.PathGenerationResults.First().Path);
            Assert.True(result.PathGenerationResults.First().GenerationStatus == GenerationStatus.Success);
            Assert.True(
                result.PathGenerationResults.First().Message ==
                SpecificationGenerationMessages.NoOperationElementFoundToParse);
        }

        [Theory]
        [MemberData(nameof(GetTestCasesForValidDocumentationShouldPassGeneration))]
        public void ValidDocumentationShouldPassGeneration(
            string inputXmlFile,
            IList<string> inputBinaryFiles,
            int expectedPathGenerationResultsCount,
            string expectedJsonFile)
        {
            var document = XDocument.Load(inputXmlFile);

            var generator = new OpenApiDocumentGenerator();

            var result = generator.GenerateV3Documents(
                document,
                inputBinaryFiles);

            Assert.NotNull(result);
            Assert.True(result.GenerationStatus == GenerationStatus.Success);
            Assert.NotNull(result.MainDocument);
            Assert.Equal(expectedPathGenerationResultsCount, result.PathGenerationResults.Count);

            var actualDocument = JsonConvert.SerializeObject(
                result.MainDocument,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()}
            );

            var expectedDocument = File.ReadAllText(expectedJsonFile);

            Assert.True(TestHelper.AreJsonEqual(expectedDocument, actualDocument));
        }
    }
}