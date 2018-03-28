// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using FluentAssertions;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Readers;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.OperationConfigTests
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
            IList<string> inputXmlFiles,
            IList<string> inputBinaryFiles,
            string configXmlFile,
            int expectedOperationGenerationResultsCount,
            string expectedJsonFile)
        {
            _output.WriteLine(testCaseName);

            var documents = new List<XDocument>();
            documents.AddRange(inputXmlFiles.Select(XDocument.Load));
            var configDocument = XDocument.Load(configXmlFile);

            var generator = new OpenApiGenerator();
            var input = new OpenApiGeneratorConfig(documents, inputBinaryFiles, "1.0.0")
            {
                AdvancedConfigurationXmlDocument = configDocument
            };

            GenerationDiagnostic result;

            var openApiDocuments = generator.GenerateDocuments(
                input,
                out result);

            result.Should().NotBeNull();
            result.DocumentGenerationDiagnostic.Errors.Count.Should().Be(0);
            openApiDocuments[DocumentVariantInfo.Default].Should().NotBeNull();
            result.OperationGenerationDiagnostics.Count.Should().Be(expectedOperationGenerationResultsCount);

            var actualDocument = openApiDocuments[DocumentVariantInfo.Default]
                .SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
            var expectedDocument = File.ReadAllText(expectedJsonFile);

            _output.WriteLine(actualDocument);

            var openApiStringReader = new OpenApiStringReader();

            var actualDeserializedDocument = openApiStringReader.Read(
                actualDocument,
                out OpenApiDiagnostic diagnostic);

            diagnostic.Errors.Count.Should().Be(0);

            actualDeserializedDocument
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
                new List<string>
                {
                    Path.Combine(InputDirectory, "Annotation.xml"),
                    Path.Combine(InputDirectory, "Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.Contracts.xml")
                },
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.SampleApis.dll"),
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.Contracts.dll")
                },
                Path.Combine(
                    InputDirectory,
                    "ConfigNoOperation.xml"),
                7,
                Path.Combine(
                    OutputDirectory,
                    "AnnotationNoOperationConfig.json")
            };

            // Blank operation section in config files
            yield return new object[]
            {
                "Blank operation section in config file",
                new List<string>
                {
                    Path.Combine(InputDirectory, "Annotation.xml"),
                    Path.Combine(InputDirectory, "Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.Contracts.xml")
                },
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.SampleApis.dll"),
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.Contracts.dll")
                },
                Path.Combine(
                    InputDirectory,
                    "ConfigBlankOperation.xml"),
                7,
                Path.Combine(
                    OutputDirectory,
                    "AnnotationBlankOperationConfig.json")
            };

            // Add annotations to all operations
            yield return new object[]
            {
                "Add annotations to all operations",
                new List<string>
                {
                    Path.Combine(InputDirectory, "Annotation.xml"),
                    Path.Combine(InputDirectory, "Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.Contracts.xml")
                },
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.SampleApis.dll"),
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.Contracts.dll")
                },
                Path.Combine(
                    InputDirectory,
                    "ConfigApplyToAllOperations.xml"),
                7,
                Path.Combine(
                    OutputDirectory,
                    "AnnotationApplyToAllOperations.json")
            };

            // Add annotations to some operations
            yield return new object[]
            {
                "Add annotations to some operations",
                new List<string>
                {
                    Path.Combine(InputDirectory, "Annotation.xml"),
                    Path.Combine(InputDirectory, "Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.Contracts.xml")
                },
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.SampleApis.dll"),
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.Contracts.dll")
                },
                Path.Combine(
                    InputDirectory,
                    "ConfigApplyToSomeOperations.xml"),
                7,
                Path.Combine(
                    OutputDirectory,
                    "AnnotationApplyToSomeOperations.json")
            };

            // Add annotations that should be partially overridden
            yield return new object[]
            {
                "Add annotations that should be partially overridden",
                new List<string>
                {
                    Path.Combine(InputDirectory, "AnnotationSomeMissingSummary.xml"),
                    Path.Combine(InputDirectory, "Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.Contracts.xml")
                },
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.SampleApis.dll"),
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.Contracts.dll")
                },
                Path.Combine(
                    InputDirectory,
                    "ConfigOverridden.xml"),
                7,
                Path.Combine(
                    OutputDirectory,
                    "AnnotationOverridden.json")
            };
        }
    }
}