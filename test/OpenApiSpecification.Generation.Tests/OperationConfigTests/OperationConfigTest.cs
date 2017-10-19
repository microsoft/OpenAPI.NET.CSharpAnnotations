// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using FluentAssertions;
using Microsoft.OpenApi.CSharpComment.Reader.Models;
using Microsoft.OpenApiSpecification.Core.Models;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.CSharpComment.Reader.Tests.OperationConfigTests
{
    [Collection("DefaultSettings")]
    public class OperationConfigTest
    {
        private const string TestFilesDirectory = "OperationConfigTests/TestFiles";
        private const string TestValidationDirectory = "OperationConfigTests/TestValidation";

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
            int expectedPathGenerationResultsCount,
            string expectedJsonFile)
        {
            _output.WriteLine(testCaseName);

            var document = XDocument.Load(inputXmlFile);
            var configDocument = XDocument.Load(configXmlFile);

            var generator = new OpenApiDocumentGenerator();

            var result = generator.GenerateV3Documents(
                document,
                inputBinaryFiles,
                configDocument);

            result.Should().NotBeNull();
            result.GenerationStatus.Should().Be(GenerationStatus.Success);
            result.MainDocument.Should().NotBeNull();
            result.PathGenerationResults.Count.Should().Be(expectedPathGenerationResultsCount);

            var actualDocument = JsonConvert.SerializeObject(result.MainDocument);

            var expectedDocument = File.ReadAllText(expectedJsonFile);

            _output.WriteLine(actualDocument);

            JsonConvert.DeserializeObject<OpenApiV3SpecificationDocument>(actualDocument)
                .Should()
                .BeEquivalentTo(
                    JsonConvert.DeserializeObject<OpenApiV3SpecificationDocument>(expectedDocument),
                    o => o.WithStrictOrdering());
        }

        private static IEnumerable<object[]> GetTestCasesForGenerateDocumentWithOperationConfigShouldSucceed()
        {
            // No operation section in config files
            yield return new object[]
            {
                "No operation section in config file",
                Path.Combine(TestFilesDirectory, "Annotation.xml"),
                new List<string>
                {
                    Path.Combine(
                        TestFilesDirectory,
                        "OpenApiSpecification.UnitTestSamples.DotNetFrameworkController.dll")
                },
                Path.Combine(
                    TestFilesDirectory,
                    "ConfigNoOperation.xml"),
                7,
                Path.Combine(
                    TestValidationDirectory,
                    "AnnotationNoOperationConfig.json")
            };

            // Blank operation section in config files
            yield return new object[]
            {
                "Blank operation section in config file",
                Path.Combine(TestFilesDirectory, "Annotation.xml"),
                new List<string>
                {
                    Path.Combine(
                        TestFilesDirectory,
                        "OpenApiSpecification.UnitTestSamples.DotNetFrameworkController.dll")
                },
                Path.Combine(
                    TestFilesDirectory,
                    "ConfigBlankOperation.xml"),
                7,
                Path.Combine(
                    TestValidationDirectory,
                    "AnnotationBlankOperationConfig.json")
            };

            // Add annotations to all operations
            yield return new object[]
            {
                "Add annotations to all operations",
                Path.Combine(TestFilesDirectory, "Annotation.xml"),
                new List<string>
                {
                    Path.Combine(
                        TestFilesDirectory,
                        "OpenApiSpecification.UnitTestSamples.DotNetFrameworkController.dll")
                },
                Path.Combine(
                    TestFilesDirectory,
                    "ConfigApplyToAllOperations.xml"),
                7,
                Path.Combine(
                    TestValidationDirectory,
                    "AnnotationApplyToAllOperations.json")
            };

            // Add annotations to some operations
            yield return new object[]
            {
                "Add annotations to some operations",
                Path.Combine(TestFilesDirectory, "Annotation.xml"),
                new List<string>
                {
                    Path.Combine(
                        TestFilesDirectory,
                        "OpenApiSpecification.UnitTestSamples.DotNetFrameworkController.dll")
                },
                Path.Combine(
                    TestFilesDirectory,
                    "ConfigApplyToSomeOperations.xml"),
                7,
                Path.Combine(
                    TestValidationDirectory,
                    "AnnotationApplyToSomeOperations.json")
            };

            // Add annotations that should be partially overridden
            yield return new object[]
            {
                "Add annotations that should be partially overridden",
                Path.Combine(TestFilesDirectory, "AnnotationSomeMissingSummary.xml"),
                new List<string>
                {
                    Path.Combine(
                        TestFilesDirectory,
                        "OpenApiSpecification.UnitTestSamples.DotNetFrameworkController.dll")
                },
                Path.Combine(
                    TestFilesDirectory,
                    "ConfigOverridden.xml"),
                7,
                Path.Combine(
                    TestValidationDirectory,
                    "AnnotationOverridden.json")
            };
        }
    }
}