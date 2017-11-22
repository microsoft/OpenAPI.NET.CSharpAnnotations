// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using FluentAssertions;
using Microsoft.OpenApi.CSharpComment.Reader.Models;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.CSharpComment.Reader.Tests.DocumentVariantTests
{
    [Collection("DefaultSettings")]
    public class DocumentVariantTest
    {
        private readonly ITestOutputHelper _output;

        public DocumentVariantTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [MemberData(
            nameof(DocumentVariantTestCases.GetTestCasesForGenerateDocumentMultipleVariantsShouldFail),
            MemberType = typeof(DocumentVariantTestCases))]
        public void GenerateDocumentMultipleVariantsShouldFail(
            string testCaseName,
            string inputXmlFile,
            IList<string> inputBinaryFiles,
            string configXmlFile,
            int expectedPathGenerationResultsCount,
            IDictionary<DocumentVariantInfo, string> documentVariantInfoToExpectedJsonFileMap,
            List<PathGenerationResult> expectedFailedPathGenerationResults)
        {
            _output.WriteLine(testCaseName);

            // Arrange
            var path = inputXmlFile;
            var document = XDocument.Load(path);

            var configPath = configXmlFile;
            var configDocument = XDocument.Load(configPath);

            var generator = new OpenApiDocumentGenerator();

            // Act
            var result = generator.GenerateV3Documents(
                document,
                inputBinaryFiles,
                configDocument);

            // Assert
            _output.WriteLine(
                JsonConvert.SerializeObject(
                    result.ToDocumentGenerationResultWithDocumentAsString()));

            result.Should().NotBeNull();
            result.GenerationStatus.Should().Be(GenerationStatus.Failure);
            result.PathGenerationResults.Count.Should().Be(expectedPathGenerationResultsCount);
            var failedPaths = result.PathGenerationResults.Where(
                    p => p.GenerationStatus == GenerationStatus.Failure)
                .ToList();

            failedPaths.Should().BeEquivalentTo(expectedFailedPathGenerationResults);

            result.Documents.Keys.Should()
                .BeEquivalentTo(documentVariantInfoToExpectedJsonFileMap.Keys);

            var actualDocuments = new List<OpenApiDocument>();
            var expectedDocuments = new List<OpenApiDocument>();

            foreach (var documentVariantInfoToExpectedJsonFile in documentVariantInfoToExpectedJsonFileMap)
            {
                // Verify each document variant against a json file content.
                var documentVariantInfo = documentVariantInfoToExpectedJsonFile.Key;
                var expectedJsonFile = documentVariantInfoToExpectedJsonFile.Value;

                result.Documents.TryGetValue(documentVariantInfo, out var specificationDocument);

                var actualDocumentAsString = specificationDocument.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0_0);

                _output.WriteLine(JsonConvert.SerializeObject(documentVariantInfo));
                _output.WriteLine(actualDocumentAsString);

                var openApiStringReader = new OpenApiStringReader();
                
                actualDocuments.Add(
                    openApiStringReader.Read(actualDocumentAsString, out var _));

                expectedDocuments.Add(
                    openApiStringReader.Read(File.ReadAllText(expectedJsonFile), out var _));
            }

            actualDocuments.Should().BeEquivalentTo(expectedDocuments);
        }

        [Theory]
        [MemberData(
            nameof(DocumentVariantTestCases.GetTestCasesForGenerateDocumentMultipleVariantsShouldSucceed),
            MemberType = typeof(DocumentVariantTestCases))]
        public void GenerateDocumentMultipleVariantsShouldSucceed(
            string testCaseName,
            string inputXmlFile,
            IList<string> inputBinaryFiles,
            string configXmlFile,
            int expectedPathGenerationResultsCount,
            IDictionary<DocumentVariantInfo, string> documentVariantInfoToExpectedJsonFileMap)
        {
            _output.WriteLine(testCaseName);

            // Arrange
            var path = inputXmlFile;
            var document = XDocument.Load(path);

            var configPath = configXmlFile;
            var configDocument = XDocument.Load(configPath);

            var generator = new OpenApiDocumentGenerator();

            // Act
            var result = generator.GenerateV3Documents(
                document,
                inputBinaryFiles,
                configDocument);

            // Assert
            _output.WriteLine(
                JsonConvert.SerializeObject(
                    result.ToDocumentGenerationResultWithDocumentAsString()));

            result.Should().NotBeNull();
            result.GenerationStatus.Should().Be(GenerationStatus.Success);

            result.PathGenerationResults.Count.Should().Be(expectedPathGenerationResultsCount);
            result.Documents.Keys.Should()
                .BeEquivalentTo(documentVariantInfoToExpectedJsonFileMap.Keys);

            var actualDocuments = new List<OpenApiDocument>();
            var expectedDocuments = new List<OpenApiDocument>();

            foreach (var documentVariantInfoToExpectedJsonFile in documentVariantInfoToExpectedJsonFileMap)
            {
                // Verify each document variant against a json file content.
                var documentVariantInfo = documentVariantInfoToExpectedJsonFile.Key;
                var expectedJsonFile = documentVariantInfoToExpectedJsonFile.Value;

                result.Documents.TryGetValue(documentVariantInfo, out var specificationDocument);

                var actualDocumentAsString = specificationDocument.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0_0);

                _output.WriteLine(JsonConvert.SerializeObject(documentVariantInfo));
                _output.WriteLine(actualDocumentAsString);
                
                var openApiStringReader = new OpenApiStringReader();

                actualDocuments.Add(
                    openApiStringReader.Read(actualDocumentAsString, out var _));

                expectedDocuments.Add(
                    openApiStringReader.Read(File.ReadAllText(expectedJsonFile), out var _));
            }

            actualDocuments.Should().BeEquivalentTo(expectedDocuments);
        }
    }
}