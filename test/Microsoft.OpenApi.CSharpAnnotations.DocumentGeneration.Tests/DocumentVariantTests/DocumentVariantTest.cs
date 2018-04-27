// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using FluentAssertions;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.DocumentVariantTests
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
            nameof(DocumentVariantTestCases.GetTestCasesForGenerateDocumentMultipleVariantsShouldYieldFailure),
            MemberType = typeof(DocumentVariantTestCases))]
        public void GenerateDocumentMultipleVariantsShouldYieldFailure(
            string testCaseName,
            IList<string> inputXmlFiles,
            IList<string> inputBinaryFiles,
            string configXmlFile,
            int expectedOperationGenerationResultsCount,
            IDictionary<DocumentVariantInfo, string> documentVariantInfoToExpectedJsonFileMap,
            DocumentGenerationDiagnostic expectedDocumentGenerationResult)
        {
            _output.WriteLine(testCaseName);

            // Arrange
            var documents = new List<XDocument>();
            documents.AddRange(inputXmlFiles.Select(XDocument.Load));

            var configPath = configXmlFile;
            var configDocument = XDocument.Load(configPath);

            var generator = new OpenApiGenerator();

            var input = new OpenApiGeneratorConfig(documents, inputBinaryFiles, "1.0.0", FilterSetVersion.V1)
            {
                AdvancedConfigurationXmlDocument = configDocument
            };

            GenerationDiagnostic result;

            // Act
            var openApiDocuments = generator.GenerateDocuments(input, out result);

            result.Should().NotBeNull();

            // All operation generations should succeed.
            result.OperationGenerationDiagnostics.Count(r => r.Errors.Count == 0)
                .Should()
                .Be(expectedOperationGenerationResultsCount);

            // Document generation should yield failure as expected in the test cases.
            result.DocumentGenerationDiagnostic.Should().BeEquivalentTo(expectedDocumentGenerationResult);

            openApiDocuments.Keys.Should()
                .BeEquivalentTo(documentVariantInfoToExpectedJsonFileMap.Keys);

            var actualDocuments = new List<OpenApiDocument>();
            var expectedDocuments = new List<OpenApiDocument>();

            foreach (var documentVariantInfoToExpectedJsonFile in documentVariantInfoToExpectedJsonFileMap)
            {
                // Verify each document variant against a json file content.
                var documentVariantInfo = documentVariantInfoToExpectedJsonFile.Key;
                var expectedJsonFile = documentVariantInfoToExpectedJsonFile.Value;

                openApiDocuments.TryGetValue(documentVariantInfo, out var specificationDocument);

                var actualDocumentAsString = specificationDocument.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

                _output.WriteLine(actualDocumentAsString);

                var openApiStringReader = new OpenApiStringReader();

                var actualDeserializedDocument = openApiStringReader.Read(
                    actualDocumentAsString,
                    out OpenApiDiagnostic diagnostic);

                diagnostic.Errors.Count.Should().Be(0);

                actualDeserializedDocument
                    .Should()
                    .BeEquivalentTo(openApiStringReader.Read(File.ReadAllText(expectedJsonFile), out var _));

                // Bug in fluent assertion method. Comparing the array of documents yields incorrect result.
                // Root cause unknown. This should be enabled once that bug is resolved.
                //actualDocuments.Add(
                //    openApiStringReader.Read(actualDocumentAsString, out var _));

                //expectedDocuments.Add(
                //    openApiStringReader.Read(File.ReadAllText(expectedJsonFile), out var _));
            }

            //actualDocuments.Should().BeEquivalentTo(expectedDocuments);
        }

        [Theory]
        [MemberData(
            nameof(DocumentVariantTestCases.GetTestCasesForGenerateDocumentMultipleVariantsShouldSucceed),
            MemberType = typeof(DocumentVariantTestCases))]
        public void GenerateDocumentMultipleVariantsShouldSucceed(
            string testCaseName,
            IList<string> inputXmlFiles,
            IList<string> inputBinaryFiles,
            string configXmlFile,
            int expectedOperationGenerationResultsCount,
            IDictionary<DocumentVariantInfo, string> documentVariantInfoToExpectedJsonFileMap)
        {
            _output.WriteLine(testCaseName);

            // Arrange
            var documents = new List<XDocument>();
            documents.AddRange(inputXmlFiles.Select(XDocument.Load));

            var configPath = configXmlFile;
            var configDocument = XDocument.Load(configPath);

            var generator = new OpenApiGenerator();
            var input = new OpenApiGeneratorConfig(documents, inputBinaryFiles, "1.0.0", FilterSetVersion.V1)
            {
                AdvancedConfigurationXmlDocument = configDocument
            };
            GenerationDiagnostic result;

            // Act
            var openApiDocuments = generator.GenerateDocuments(input, out result);

            result.Should().NotBeNull();
            result.DocumentGenerationDiagnostic.Errors.Count.Should().Be(0);
            result.OperationGenerationDiagnostics.Count(r => r.Errors.Count == 0)
                .Should()
                .Be(expectedOperationGenerationResultsCount);

            openApiDocuments.Keys.Should()
                .BeEquivalentTo(documentVariantInfoToExpectedJsonFileMap.Keys);

            var actualDocuments = new List<OpenApiDocument>();
            var expectedDocuments = new List<OpenApiDocument>();

            foreach (var documentVariantInfoToExpectedJsonFile in documentVariantInfoToExpectedJsonFileMap)
            {
                // Verify each document variant against a json file content.
                var documentVariantInfo = documentVariantInfoToExpectedJsonFile.Key;
                var expectedJsonFile = documentVariantInfoToExpectedJsonFile.Value;

                openApiDocuments.TryGetValue(documentVariantInfo, out var specificationDocument);

                var actualDocumentAsString = specificationDocument.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

                _output.WriteLine(actualDocumentAsString);

                var openApiStringReader = new OpenApiStringReader();

                var actualDeserializedDocument = openApiStringReader.Read(
                    actualDocumentAsString,
                    out OpenApiDiagnostic diagnostic);

                diagnostic.Errors.Count.Should().Be(0);

                actualDeserializedDocument
                    .Should()
                    .BeEquivalentTo(openApiStringReader.Read(File.ReadAllText(expectedJsonFile), out var _));

                // Bug in fluent assertion method. Comparing the array of documents yields incorrect result.
                // Root cause unknown. This should be enabled once that bug is resolved.
                //actualDocuments.Add(actualDocument);

                //expectedDocuments.Add(expectedDocument);
            }

            //actualDocuments.Should().BeEquivalentTo(expectedDocuments);
        }
    }
}