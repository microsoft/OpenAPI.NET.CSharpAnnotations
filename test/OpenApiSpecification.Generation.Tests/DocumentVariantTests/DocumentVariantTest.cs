// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using FluentAssertions;
using Microsoft.OpenApiSpecification.Core.Models;
using Microsoft.OpenApiSpecification.Core.Serialization;
using Microsoft.OpenApiSpecification.Generation.Models;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApiSpecification.Generation.Tests.DocumentVariantTests
{
    [Collection("DefaultSettings")]
    public class DocumentVariantTest
    {
        private const string TestFilesDirectory = "DocumentVariantTests/TestFiles";
        private const string TestValidationDirectory = "DocumentVariantTests/TestValidation";
        private const string TestValidationWithCommonAnnotationsDirectory =
            "DocumentVariantTests/TestValidation/AnnotationsWithCommonAnnotations";

        private readonly ITestOutputHelper _output;

        public DocumentVariantTest(ITestOutputHelper output)
        {
            _output = output;
        }

        private static IEnumerable<object[]> GetTestCasesForGenerateDocumentMultipleVariantsShouldSucceed()
        {
            // One document variant tag name
            yield return new object[]
            {
                "One document variant tag name",
                Path.Combine(TestFilesDirectory, "Annotation.xml"),
                new List<string>
                {
                    Path.Combine(
                        TestFilesDirectory,
                        "OpenApiSpecification.UnitTestSamples.DotNetFrameworkController.dll")
                },
                Path.Combine(
                    TestFilesDirectory,
                    "ConfigOneDocumentVariantTag.xml"),
                9,
                new Dictionary<DocumentVariantInfo, string>()
                {
                    [ DocumentVariantInfo.Default ] = Path.Combine(TestValidationDirectory, "AnnotationDefaultVariant.json"),
                    [ new DocumentVariantInfo
                    {
                        // Only contains info from operations with swagger tags with title "Group1" in Annotation.xml 
                        Categorizer = "swagger",
                        Title = "Group1"
                    } ] = Path.Combine(TestValidationDirectory, "AnnotationVariantSwaggerGroup1.json"),
                    [ new DocumentVariantInfo
                    {
                        // Only contains info from operations with swagger tags with title "Group2" in Annotation.xml 
                        Categorizer = "swagger",
                        Title = "Group2"
                    } ] = Path.Combine(TestValidationDirectory, "AnnotationVariantSwaggerGroup2.json"),
                },
            };

            // Multiple document variant tag names
            yield return new object[]
            {
                "Multiple document variant tag names",
                Path.Combine(TestFilesDirectory, "Annotation.xml"),
                new List<string>
                {
                    Path.Combine(
                        TestFilesDirectory,
                        "OpenApiSpecification.UnitTestSamples.DotNetFrameworkController.dll")
                },
                Path.Combine(
                    TestFilesDirectory,
                    "ConfigMultipleDocumentVariantTags.xml"),
                9,
                new Dictionary<DocumentVariantInfo, string>()
                {
                    [ DocumentVariantInfo.Default ] = Path.Combine(TestValidationDirectory, "AnnotationDefaultVariant.json"),
                    [ new DocumentVariantInfo
                    {
                        // Only contains info from operations with swagger tags with title "Group1" in Annotation.xml 
                        Categorizer = "swagger",
                        Title = "Group1"
                    } ] = Path.Combine(TestValidationDirectory, "AnnotationVariantSwaggerGroup1.json"),
                    [ new DocumentVariantInfo
                    {
                        // Only contains info from operations with swagger tags with title "Group2" in Annotation.xml 
                        Categorizer = "swagger",
                        Title = "Group2"
                    } ] = Path.Combine(TestValidationDirectory, "AnnotationVariantSwaggerGroup2.json"),
                    [ new DocumentVariantInfo
                    {
                        // Only contains info from operations with swagger tags with title "Group1" in Annotation.xml 
                        Categorizer = "swagger2",
                        Title = "Group1"
                    } ] = Path.Combine(TestValidationDirectory, "AnnotationVariantSwagger2Group1.json"),
                },
            };

            // Multiple document variant tag names with common annotations
            yield return new object[]
            {
                "Multiple document variant tag names with common annotations",
                Path.Combine(TestFilesDirectory, "Annotation.xml"),
                new List<string>
                {
                    Path.Combine(
                        TestFilesDirectory,
                        "OpenApiSpecification.UnitTestSamples.DotNetFrameworkController.dll")
                },
                Path.Combine(
                    TestFilesDirectory,
                    "ConfigMultipleDocumentVariantTagsWithCommonAnnotations.xml"),
                9,
                new Dictionary<DocumentVariantInfo, string>()
                {
                    [ DocumentVariantInfo.Default ] = Path.Combine(TestValidationWithCommonAnnotationsDirectory, "AnnotationDefaultVariant.json"),
                    [ new DocumentVariantInfo
                    {
                        // Only contains info from operations with swagger tags with title "Group1" in Annotation.xml 
                        Categorizer = "swagger",
                        Title = "Group1"
                    } ] = Path.Combine(TestValidationWithCommonAnnotationsDirectory, "AnnotationVariantSwaggerGroup1.json"),
                    [ new DocumentVariantInfo
                    {
                        // Only contains info from operations with swagger tags with title "Group2" in Annotation.xml 
                        Categorizer = "swagger",
                        Title = "Group2"
                    } ] = Path.Combine(TestValidationWithCommonAnnotationsDirectory, "AnnotationVariantSwaggerGroup2.json"),
                    [ new DocumentVariantInfo
                    {
                        // Only contains info from operations with swagger tags with title "Group1" in Annotation.xml 
                        Categorizer = "swagger2",
                        Title = "Group1"
                    } ] = Path.Combine(TestValidationWithCommonAnnotationsDirectory, "AnnotationVariantSwagger2Group1.json"),
                },
            };
        }

        [Theory]
        [MemberData(nameof(GetTestCasesForGenerateDocumentMultipleVariantsShouldSucceed))]
        public void GenerateDocumentMultipleVariantsShouldSucceed(
            string testCaseName,
            string inputXmlFile,
            IList<string> inputBinaryFiles,
            string configXmlFile,
            int expectedPathGenerationResultsCount,
            IDictionary<DocumentVariantInfo, string> documentVariantInfoToExpectedJsonFileMap )
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
            result.Should().NotBeNull();
            result.GenerationStatus.Should().Be(GenerationStatus.Success);

            result.PathGenerationResults.Count.Should().Be(expectedPathGenerationResultsCount);
            result.Documents.Keys.Should()
                .BeEquivalentTo(documentVariantInfoToExpectedJsonFileMap.Keys);

            var actualDocuments = new List<OpenApiV3SpecificationDocument>();
            var expectedDocuments = new List<OpenApiV3SpecificationDocument>();

            // Default Document Variant
            foreach (var documentVariantInfoToExpectedJsonFile in documentVariantInfoToExpectedJsonFileMap)
            {
                var documentVariantInfo = documentVariantInfoToExpectedJsonFile.Key;
                var expectedJsonFile = documentVariantInfoToExpectedJsonFile.Value;

                OpenApiV3SpecificationDocument specificationDocument;

                result.Documents.TryGetValue(documentVariantInfo, out specificationDocument);

                var actualDocumentAsString = JsonConvert.SerializeObject(specificationDocument);

                _output.WriteLine(JsonConvert.SerializeObject(documentVariantInfo));
                _output.WriteLine(actualDocumentAsString);

                actualDocuments.Add(JsonConvert.DeserializeObject<OpenApiV3SpecificationDocument>(actualDocumentAsString));

                expectedDocuments.Add(JsonConvert.DeserializeObject<OpenApiV3SpecificationDocument>(File.ReadAllText(expectedJsonFile)));
            }

            actualDocuments.Should().BeEquivalentTo(expectedDocuments);
        }
    }
}