// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using FluentAssertions;
using Microsoft.OpenApi.CSharpComment.Reader.Extensions;
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
            nameof(DocumentVariantTestCases.GetTestCasesForGenerateDocumentMultipleVariantsShouldYieldWarning),
            MemberType = typeof(DocumentVariantTestCases))]
        public void GenerateDocumentMultipleVariantsShouldYieldWarning(
            string testCaseName,
            IList<string> inputXmlFiles,
            IList<string> inputBinaryFiles,
            string configXmlFile,
            OpenApiSpecVersion openApiSpecVersion,
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

            var generator = new CSharpCommentOpenApiGenerator();

            var input = new CSharpCommentOpenApiGeneratorConfig(documents, inputBinaryFiles, openApiSpecVersion)
            {
                AdvancedConfigurationXmlDocument = configDocument
            };

            GenerationDiagnostic result;

            // Act
            var openApiDocuments = generator.GenerateDocuments(input, out result);

            // Assert
            _output.WriteLine(
                JsonConvert.SerializeObject(
                    openApiDocuments.ToSerializedOpenApiDocuments(),
                    new DictionaryJsonConverter<DocumentVariantInfo, string>()));

            result.Should().NotBeNull();
            result.GenerationStatus.Should().Be(GenerationStatus.Warning);

            // All operation generations should succeed.
            result.OperationGenerationDiagnostics.Count(r => r.GenerationStatus == GenerationStatus.Success)
                .Should()
                .Be(expectedOperationGenerationResultsCount);

            // Document generation should yield warning as expected in the test cases.
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

                _output.WriteLine(JsonConvert.SerializeObject(documentVariantInfo));
                _output.WriteLine(actualDocumentAsString);

                var openApiStringReader = new OpenApiStringReader();

                var actualDocument = openApiStringReader.Read(actualDocumentAsString, out var _);
                var expectedDocument = openApiStringReader.Read(File.ReadAllText(expectedJsonFile), out var _);

                actualDocument.Should().BeEquivalentTo(expectedDocument);

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
            OpenApiSpecVersion openApiSpecVersion,
            int expectedOperationGenerationResultsCount,
            IDictionary<DocumentVariantInfo, string> documentVariantInfoToExpectedJsonFileMap)
        {
            _output.WriteLine(testCaseName);

            // Arrange
            var documents = new List<XDocument>();
            documents.AddRange(inputXmlFiles.Select(XDocument.Load));

            var configPath = configXmlFile;
            var configDocument = XDocument.Load(configPath);

            var generator = new CSharpCommentOpenApiGenerator();
            var input = new CSharpCommentOpenApiGeneratorConfig(documents, inputBinaryFiles, openApiSpecVersion)
            {
                AdvancedConfigurationXmlDocument = configDocument
            };
            GenerationDiagnostic result;

            // Act
            var openApiDocuments = generator.GenerateDocuments(input, out result);

            // Assert
            _output.WriteLine(
                JsonConvert.SerializeObject(
                    openApiDocuments.ToSerializedOpenApiDocuments(),
                    new DictionaryJsonConverter<DocumentVariantInfo, string>()));

            result.Should().NotBeNull();
            result.GenerationStatus.Should().Be(GenerationStatus.Success);

            result.OperationGenerationDiagnostics.Count.Should().Be(expectedOperationGenerationResultsCount);
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

                var actualDocumentAsString = specificationDocument.SerializeAsJson(openApiSpecVersion);

                _output.WriteLine(JsonConvert.SerializeObject(documentVariantInfo));
                _output.WriteLine(actualDocumentAsString);

                var openApiStringReader = new OpenApiStringReader();

                var actualDocument = openApiStringReader.Read(actualDocumentAsString, out var _);
                var expectedDocument = openApiStringReader.Read(File.ReadAllText(expectedJsonFile), out var _);

                actualDocument.Should().BeEquivalentTo(expectedDocument);

                // Bug in fluent assertion method. Comparing the array of documents yields incorrect result.
                // Root cause unknown. This should be enabled once that bug is resolved.
                //actualDocuments.Add(actualDocument);

                //expectedDocuments.Add(expectedDocument);
            }

            //actualDocuments.Should().BeEquivalentTo(expectedDocuments);
        }
    }
}