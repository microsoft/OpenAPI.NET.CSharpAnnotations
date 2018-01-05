// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using FluentAssertions;
using Microsoft.OpenApi.CSharpComment.Reader.Models;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Readers;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.CSharpComment.Reader.Tests.OperationConfigTests
{
    [Collection("DefaultSettings")]
    public class OperationConfigTest
    {
        private const string InputDirectory = "OperationConfigTests/Input";
        private const string OutputDirectory = "OperationConfigTests/Output";

        private readonly ITestOutputHelper _output;

        public OperationConfigTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [MemberData(nameof(GetTestCasesForGenerateDocumentWithOperationConfigShouldSucceed))]
        public void GenerateDocumentWithOperationConfigShouldSucceed(
            string testCaseName,
            string inputXmlFile,
            IList<string> inputBinaryFiles,
            string configXmlFile,
            OpenApiSpecVersion openApiSpecVersion,
            int expectedOperationGenerationResultsCount,
            string expectedJsonFile)
        {
            _output.WriteLine(testCaseName);

            var document = XDocument.Load(inputXmlFile);
            var configDocument = XDocument.Load(configXmlFile);

            var generator = new OpenApiDocumentGenerator();

            var result = generator.GenerateOpenApiDocuments(
                document,
                inputBinaryFiles,
                configDocument,
                openApiSpecVersion);

            result.Should().NotBeNull();
            result.GenerationStatus.Should().Be(GenerationStatus.Success);
            result.MainDocument.Should().NotBeNull();
            result.OperationGenerationResults.Count.Should().Be(expectedOperationGenerationResultsCount);

            // Document-level generation should succeed.
            result.DocumentGenerationResult.Should()
                .BeEquivalentTo(
                    new DocumentGenerationResult
                    {
                        GenerationStatus = GenerationStatus.Success
                    });

            var actualDocument = result.MainDocument.SerializeAsJson(openApiSpecVersion);

            var expectedDocument = File.ReadAllText(expectedJsonFile);

            _output.WriteLine(actualDocument);

            var openApiStringReader = new OpenApiStringReader();
            openApiStringReader.Read(actualDocument, out var _)
                .Should()
                .BeEquivalentTo(
                    openApiStringReader.Read(expectedDocument, out var _),
                    o => o.WithStrictOrdering());
        }

        public static IEnumerable<object[]> GetTestCasesForGenerateDocumentWithOperationConfigShouldSucceed()
        {
            // No operation section in config files
            yield return new object[]
            {
                "No operation section in config file",
                Path.Combine(InputDirectory, "Annotation.xml"),
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.SampleApis.dll")
                },
                Path.Combine(
                    InputDirectory,
                    "ConfigNoOperation.xml"),
                OpenApiSpecVersion.OpenApi3_0,
                7,
                Path.Combine(
                    OutputDirectory,
                    "AnnotationNoOperationConfig.json")
            };

            // Blank operation section in config files
            yield return new object[]
            {
                "Blank operation section in config file",
                Path.Combine(InputDirectory, "Annotation.xml"),
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.SampleApis.dll")
                },
                Path.Combine(
                    InputDirectory,
                    "ConfigBlankOperation.xml"),
                OpenApiSpecVersion.OpenApi3_0,
                7,
                Path.Combine(
                    OutputDirectory,
                    "AnnotationBlankOperationConfig.json")
            };

            // Add annotations to all operations
            yield return new object[]
            {
                "Add annotations to all operations",
                Path.Combine(InputDirectory, "Annotation.xml"),
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.SampleApis.dll")
                },
                Path.Combine(
                    InputDirectory,
                    "ConfigApplyToAllOperations.xml"),
                OpenApiSpecVersion.OpenApi3_0,
                7,
                Path.Combine(
                    OutputDirectory,
                    "AnnotationApplyToAllOperations.json")
            };

            // Add annotations to some operations
            yield return new object[]
            {
                "Add annotations to some operations",
                Path.Combine(InputDirectory, "Annotation.xml"),
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.SampleApis.dll")
                },
                Path.Combine(
                    InputDirectory,
                    "ConfigApplyToSomeOperations.xml"),
                OpenApiSpecVersion.OpenApi3_0,
                7,
                Path.Combine(
                    OutputDirectory,
                    "AnnotationApplyToSomeOperations.json")
            };

            // Add annotations that should be partially overridden
            yield return new object[]
            {
                "Add annotations that should be partially overridden",
                Path.Combine(InputDirectory, "AnnotationSomeMissingSummary.xml"),
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.SampleApis.dll")
                },
                Path.Combine(
                    InputDirectory,
                    "ConfigOverridden.xml"),
                OpenApiSpecVersion.OpenApi3_0,
                7,
                Path.Combine(
                    OutputDirectory,
                    "AnnotationOverridden.json")
            };
        }
    }
}