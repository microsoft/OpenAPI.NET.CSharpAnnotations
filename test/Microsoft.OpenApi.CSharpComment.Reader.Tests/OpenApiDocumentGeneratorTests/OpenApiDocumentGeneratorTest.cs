// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using FluentAssertions;
using Microsoft.OpenApi.CSharpComment.Reader.Exceptions;
using Microsoft.OpenApi.CSharpComment.Reader.Models;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.CSharpComment.Reader.Tests.OpenApiDocumentGeneratorTests
{
    [Collection("DefaultSettings")]
    public class OpenApiDocumentGeneratorTest
    {
        private const string InputDirectory = "OpenApiDocumentGeneratorTests/Input";
        private const string OutputDirectory = "OpenApiDocumentGeneratorTests/Output";

        private readonly ITestOutputHelper _output;

        public OpenApiDocumentGeneratorTest(ITestOutputHelper output)
        {
            _output = output;
        }

        public static IEnumerable<object[]> GetTestCasesForInvalidDocumentationShouldYieldFailure()
        {
            // Invalid Verb
            yield return new object[]
            {
                "Invalid Verb",
                Path.Combine(InputDirectory, "AnnotationInvalidVerb.xml"),
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.SampleApis.dll")
                },
                OpenApiSpecVersion.OpenApi3_0,
                9,
                Path.Combine(
                    OutputDirectory,
                    "AnnotationInvalidVerb.Json"),
                new List<OperationGenerationResult>
                {
                    new OperationGenerationResult
                    {
                        OperationMethod = "Invalid",
                        Path = "/V1/samples/{id}",
                        Errors =
                        {
                            new GenerationError
                            {
                                ExceptionType = typeof(InvalidVerbException).Name,
                                Message = string.Format(SpecificationGenerationMessages.InvalidHttpMethod, "Invalid"),
                            }
                        },
                        GenerationStatus = GenerationStatus.Failure
                    }
                }
            };

            // Invalid Uri
            yield return new object[]
            {
                "Invalid Uri",
                Path.Combine(InputDirectory, "AnnotationInvalidUri.xml"),
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.SampleApis.dll")
                },
                OpenApiSpecVersion.OpenApi3_0,
                9,
                Path.Combine(
                    OutputDirectory,
                    "AnnotationInvalidUri.Json"),
                new List<OperationGenerationResult>
                {
                    new OperationGenerationResult
                    {
                        OperationMethod = SpecificationGenerationMessages.OperationMethodNotParsedGivenUrlIsInvalid,
                        Path = "http://{host}:9000/V1/samples/{id}?queryBool={queryBool}",
                        Errors =
                        {
                            new GenerationError
                            {
                                ExceptionType = typeof(InvalidUrlException).Name,
                                Message = string.Format(
                                    SpecificationGenerationMessages.InvalidUrl,
                                    "http://{host}:9000/V1/samples/{id}?queryBool={queryBool}",
                                    SpecificationGenerationMessages.MalformattedUrl),
                            }
                        },
                        GenerationStatus = GenerationStatus.Failure
                    }
                }
            };
        }

        public static IEnumerable<object[]> GetTestCasesForInvalidDocumentationShouldYieldWarning()
        {
            // Parameters that have no in attributes and not present in the URL.
            yield return new object[]
            {
                "Parameters Without In Attribute And Not Present In URL",
                Path.Combine(InputDirectory, "AnnotationParamWithoutInNotPresentInUrl.xml"),
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.SampleApis.dll")
                },
                OpenApiSpecVersion.OpenApi3_0,
                9,
                Path.Combine(
                    OutputDirectory,
                    "AnnotationParamWithoutInNotPresentInUrl.Json"),
                new List<OperationGenerationResult>
                {
                    new OperationGenerationResult
                    {
                        OperationMethod = OperationType.Get.ToString(),
                        Path = "/V1/samples/{id}",
                        Errors =
                        {
                            new GenerationError
                            {
                                ExceptionType = typeof(MissingInAttributeException).Name,
                                Message = string.Format(
                                    SpecificationGenerationMessages.MissingInAttribute,
                                    string.Join(", ", new List<string> {"sampleHeaderParam2", "sampleHeaderParam3"})),
                            }
                        },
                        GenerationStatus = GenerationStatus.Warning
                    }
                }
            };

            // Conflicting Path and Query Parameters
            yield return new object[]
            {
                "Conflicting Path and Query Parameters",
                Path.Combine(InputDirectory, "AnnotationConflictingPathAndQueryParameters.xml"),
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.SampleApis.dll")
                },
                OpenApiSpecVersion.OpenApi3_0,
                9,
                Path.Combine(
                    OutputDirectory,
                    "AnnotationConflictingPathAndQueryParameters.Json"),
                new List<OperationGenerationResult>
                {
                    new OperationGenerationResult
                    {
                        OperationMethod = OperationType.Get.ToString(),
                        Path = "/V1/samples/{id}",
                        Errors =
                        {
                            new GenerationError
                            {
                                ExceptionType = typeof(ConflictingPathAndQueryParametersException).Name,
                                Message = string.Format(
                                    SpecificationGenerationMessages.ConflictingPathAndQueryParameters,
                                    "id",
                                    "http://localhost:9000/V1/samples/{id}?queryBool={queryBool}&id={id}"),
                            }
                        },
                        GenerationStatus = GenerationStatus.Warning
                    }
                }
            };

            // Path parameter in the URL is not documented in any param elements.
            yield return new object[]
            {
                "Path Parameter Undocumented",
                Path.Combine(InputDirectory, "AnnotationUndocumentedPathParam.xml"),
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.SampleApis.dll")
                },
                OpenApiSpecVersion.OpenApi3_0,
                9,
                Path.Combine(
                    OutputDirectory,
                    "AnnotationUndocumentedPathParam.Json"),
                new List<OperationGenerationResult>
                {
                    new OperationGenerationResult
                    {
                        OperationMethod = OperationType.Get.ToString(),
                        Path = "/V1/samples/{id}",
                        Errors =
                        {
                            new GenerationError
                            {
                                ExceptionType = typeof(UndocumentedPathParameterException).Name,
                                Message = string.Format(
                                    SpecificationGenerationMessages.UndocumentedPathParameter,
                                    "id",
                                    "http://localhost:9000/V1/samples/{id}?queryBool={queryBool}"),
                            }
                        },
                        GenerationStatus = GenerationStatus.Warning
                    }
                }
            };

            // Undocumented Generics
            yield return new object[]
            {
                "Undocumented Generics",
                Path.Combine(InputDirectory, "AnnotationUndocumentedGeneric.xml"),
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.SampleApis.dll")
                },
                OpenApiSpecVersion.OpenApi3_0,
                9,
                Path.Combine(
                    OutputDirectory,
                    "AnnotationUndocumentedGeneric.Json"),
                new List<OperationGenerationResult>
                {
                    new OperationGenerationResult
                    {
                        OperationMethod = OperationType.Get.ToString(),
                        Path = "/V3/samples/{id}",
                        Errors =
                        {
                            new GenerationError
                            {
                                ExceptionType = typeof(UndocumentedGenericTypeException).Name,
                                Message = SpecificationGenerationMessages.UndocumentedGenericType,
                            }
                        },
                        GenerationStatus = GenerationStatus.Warning
                    }
                }
            };

            // Incorrect Order for Generics
            yield return new object[]
            {
                "Incorrect Order for Generics",
                Path.Combine(InputDirectory, "AnnotationIncorrectlyOrderedGeneric.xml"),
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.SampleApis.dll")
                },
                OpenApiSpecVersion.OpenApi3_0,
                9,
                Path.Combine(
                    OutputDirectory,
                    "AnnotationIncorrectlyOrderedGeneric.Json"),
                new List<OperationGenerationResult>
                {
                    new OperationGenerationResult
                    {
                        OperationMethod = OperationType.Get.ToString(),
                        Path = "/V3/samples/",
                        Errors =
                        {
                            new GenerationError
                            {
                                ExceptionType = typeof(UnorderedGenericTypeException).Name,
                                Message = SpecificationGenerationMessages.UnorderedGenericType,
                            }
                        },
                        GenerationStatus = GenerationStatus.Warning
                    }
                }
            };
        }

        public static IEnumerable<object[]> GetTestCasesForValidDocumentationShouldReturnCorrectDocument()
        {
            // Standard, original valid XML document
            yield return new object[]
            {
                "Standard valid XML document",
                Path.Combine(InputDirectory, "Annotation.xml"),
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.SampleApis.dll")
                },
                OpenApiSpecVersion.OpenApi3_0,
                9,
                Path.Combine(
                    OutputDirectory,
                    "Annotation.Json")
            };

            // Valid XML document but with parameters that have no in attributes but are present in the URL.
            yield return new object[]
            {
                "Parameters Without In Attribute But Present In URL",
                Path.Combine(InputDirectory, "AnnotationParamWithoutInButPresentInUrl.xml"),
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.SampleApis.dll")
                },
                OpenApiSpecVersion.OpenApi3_0,
                9,
                Path.Combine(
                    OutputDirectory,
                    "AnnotationParamWithoutInButPresentInUrl.Json")
            };

            // Valid XML document but with one parameter without specified type.
            // The type should simply default to string.
            yield return new object[]
            {
                "Unspecified Type Default to String",
                Path.Combine(InputDirectory, "AnnotationParamNoTypeSpecified.xml"),
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.SampleApis.dll")
                },
                OpenApiSpecVersion.OpenApi3_0,
                9,
                Path.Combine(
                    OutputDirectory,
                    "AnnotationParamNoTypeSpecified.Json")
            };

            // Valid XML document with multiple response types per response code.
            yield return new object[]
            {
                "Multiple Response Types Per Response Code",
                Path.Combine(InputDirectory, "AnnotationMultipleResponseTypes.xml"),
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.SampleApis.dll")
                },
                OpenApiSpecVersion.OpenApi3_0,
                9,
                Path.Combine(
                    OutputDirectory,
                    "AnnotationMultipleResponseTypes.Json")
            };

            // Valid XML document with multiple request types.
            yield return new object[]
            {
                "Multiple Request Types",
                Path.Combine(InputDirectory, "AnnotationMultipleRequestTypes.xml"),
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.SampleApis.dll")
                },
                OpenApiSpecVersion.OpenApi3_0,
                9,
                Path.Combine(
                    OutputDirectory,
                    "AnnotationMultipleRequestTypes.Json")
            };

            // Valid XML document with multiple request content types.
            yield return new object[]
            {
                "Multiple Request Media Types",
                Path.Combine(InputDirectory, "AnnotationMultipleRequestMediaTypes.xml"),
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.SampleApis.dll")
                },
                OpenApiSpecVersion.OpenApi3_0,
                9,
                Path.Combine(
                    OutputDirectory,
                    "AnnotationMultipleRequestMediaTypes.Json")
            };

            // Valid XML document with multiple response content types.
            yield return new object[]
            {
                "Multiple Response Media Types Per Response Code",
                Path.Combine(InputDirectory, "AnnotationMultipleResponseMediaTypes.xml"),
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.SampleApis.dll")
                },
                OpenApiSpecVersion.OpenApi3_0,
                9,
                Path.Combine(
                    OutputDirectory,
                    "AnnotationMultipleResponseMediaTypes.Json")
            };

            // Valid XML document with optional path parameters.
            yield return new object[]
            {
                "Optional Path Parameters",
                Path.Combine(InputDirectory, "AnnotationOptionalPathParametersBranching.xml"),
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.SampleApis.dll")
                },
                OpenApiSpecVersion.OpenApi3_0,
                9,
                Path.Combine(
                    OutputDirectory,
                    "AnnotationOptionalPathParametersBranching.Json")
            };

            // Valid XML document with alternative param tags.
            yield return new object[]
            {
                "Alternative Param Tags (i.e. queryParam, pathParam, header)",
                Path.Combine(InputDirectory, "AnnotationAlternativeParamTags.xml"),
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.SampleApis.dll")
                },
                OpenApiSpecVersion.OpenApi3_0,
                9,
                Path.Combine(
                    OutputDirectory,
                    "Annotation.Json")
            };

            // Valid XML document with array type in param tags.
            yield return new object[]
            {
                "Array Type in Param Tags",
                Path.Combine(InputDirectory, "AnnotationArrayInParamTags.xml"),
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.SampleApis.dll")
                },
                OpenApiSpecVersion.OpenApi3_0,
                9,
                Path.Combine(
                    OutputDirectory,
                    "AnnotationArrayInParamTags.Json")
            };
        }

        /// <summary>
        /// A short version of the <see cref="GetTestCasesForValidDocumentationShouldReturnCorrectDocument"/>
        /// so that we can simply test the serialization without wasting time on all test cases.
        /// </summary> 
        public static IEnumerable<object[]> GetTestCasesForValidDocumentationShouldReturnCorrectSerializedDocument()
        {
            // Standard, original valid XML document with JSON as output
            yield return new object[]
            {
                "Standard valid XML document (JSON)",
                Path.Combine(InputDirectory, "Annotation.xml"),
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.SampleApis.dll")
                },
                OpenApiSpecVersion.OpenApi3_0,
                OpenApiFormat.Json,
                9,
                Path.Combine(
                    OutputDirectory,
                    "Annotation.Json")
            };

            // Standard, original valid XML document with YAML as output
            yield return new object[]
            {
                "Standard valid XML document (YAML)",
                Path.Combine(InputDirectory, "Annotation.xml"),
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.SampleApis.dll")
                },
                OpenApiSpecVersion.OpenApi3_0,
                OpenApiFormat.Yaml,
                9,
                Path.Combine(
                    OutputDirectory,
                    "Annotation.Json")
            };
        }


        [Theory]
        [MemberData(nameof(GetTestCasesForInvalidDocumentationShouldYieldFailure))]
        public void InvalidDocumentationShouldYieldFailure(
            string testCaseName,
            string inputXmlFile,
            IList<string> inputBinaryFiles,
            OpenApiSpecVersion openApiSpecVersion,
            int expectedOperationGenerationResultsCount,
            string expectedJsonFile,
            IList<OperationGenerationResult> expectedFailureOperationGenerationResults)
        {
            _output.WriteLine(testCaseName);

            var document = XDocument.Load(inputXmlFile);

            var generator = new OpenApiDocumentGenerator();

            var result = generator.GenerateOpenApiDocuments(
                document,
                inputBinaryFiles,
                openApiSpecVersion);

            result.Should().NotBeNull();

            _output.WriteLine(
                JsonConvert.SerializeObject(
                    result.ToDocumentGenerationResultSerializedDocument(openApiSpecVersion, OpenApiFormat.Json)));

            result.GenerationStatus.Should().Be(GenerationStatus.Failure);
            result.MainDocument.Should().NotBeNull();
            result.OperationGenerationResults.Count.Should().Be(expectedOperationGenerationResultsCount);

            var failurePaths = result.OperationGenerationResults.Where(
                    p => p.GenerationStatus == GenerationStatus.Failure)
                .ToList();

            var actualDocument = result.MainDocument.SerializeAsJson(openApiSpecVersion);

            var expectedDocument = File.ReadAllText(expectedJsonFile);

            _output.WriteLine(actualDocument);

            failurePaths.Should().BeEquivalentTo(expectedFailureOperationGenerationResults);

            // We are doing serialization and deserialization to force the resulting actual document
            // to have the exact fields we will see in the resulting document based on the contract resolver.
            // Without serialization and deserialization, the actual document may have fields that should
            // not be present, such as empty list fields.
            var openApiStringReader = new OpenApiStringReader();
            openApiStringReader.Read(actualDocument, out var _)
                .Should()
                .BeEquivalentTo(openApiStringReader.Read(expectedDocument, out var _));
        }

        [Theory]
        [MemberData(nameof(GetTestCasesForInvalidDocumentationShouldYieldWarning))]
        public void InvalidDocumentationShouldYieldWarning(
            string testCaseName,
            string inputXmlFile,
            IList<string> inputBinaryFiles,
            OpenApiSpecVersion openApiSpecVersion,
            int expectedOperationGenerationResultsCount,
            string expectedJsonFile,
            IList<OperationGenerationResult> expectedWarningOperationGenerationResults)
        {
            _output.WriteLine(testCaseName);

            var document = XDocument.Load(inputXmlFile);

            var generator = new OpenApiDocumentGenerator();

            var result = generator.GenerateOpenApiDocuments(
                document,
                inputBinaryFiles,
                openApiSpecVersion);

            result.Should().NotBeNull();

            _output.WriteLine(
                JsonConvert.SerializeObject(
                    result.ToDocumentGenerationResultSerializedDocument(openApiSpecVersion, OpenApiFormat.Json)));

            result.GenerationStatus.Should().Be(GenerationStatus.Warning);
            result.MainDocument.Should().NotBeNull();
            result.OperationGenerationResults.Count.Should().Be(expectedOperationGenerationResultsCount);

            var warningPaths = result.OperationGenerationResults.Where(
                    p => p.GenerationStatus == GenerationStatus.Warning)
                .ToList();

            var actualDocument = result.MainDocument.SerializeAsJson(openApiSpecVersion);

            var expectedDocument = File.ReadAllText(expectedJsonFile);

            _output.WriteLine(actualDocument);

            warningPaths.Should().BeEquivalentTo(expectedWarningOperationGenerationResults);

            // We are doing serialization and deserialization to force the resulting actual document
            // to have the exact fields we will see in the resulting document based on the contract resolver.
            // Without serialization and deserialization, the actual document may have fields that should
            // not be present, such as empty list fields.
            var openApiStringReader = new OpenApiStringReader();
            openApiStringReader.Read(actualDocument, out var _)
                .Should()
                .BeEquivalentTo(openApiStringReader.Read(expectedDocument, out var _));
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0)]
        public void NoOperationsToParseShouldReturnEmptyDocument(OpenApiSpecVersion openApiSpecVersion)
        {
            var path = Path.Combine(InputDirectory, "AnnotationNoOperationsToParse.xml");

            var document = XDocument.Load(path);

            var generator = new OpenApiDocumentGenerator();

            var result = generator.GenerateOpenApiDocuments(
                document,
                new List<string>(),
                openApiSpecVersion);

            result.Should().NotBeNull();
            result.GenerationStatus.Should().Be(GenerationStatus.Warning);
            result.MainDocument.Should().BeNull();
            result.DocumentGenerationResult.Should()
                .BeEquivalentTo(
                    new DocumentGenerationResult
                    {
                        Errors =
                        {
                            new GenerationError
                            {
                                Message = SpecificationGenerationMessages.NoOperationElementFoundToParse,
                            }
                        },
                        GenerationStatus = GenerationStatus.Warning
                    }
                );
        }

        [Theory]
        [MemberData(nameof(GetTestCasesForValidDocumentationShouldReturnCorrectDocument))]
        public void ValidDocumentationShouldReturnCorrectDocument(
            string testCaseName,
            string inputXmlFile,
            IList<string> inputBinaryFiles,
            OpenApiSpecVersion openApiSpecVersion,
            int expectedOperationGenerationResultsCount,
            string expectedJsonFile)
        {
            _output.WriteLine(testCaseName);

            var document = XDocument.Load(inputXmlFile);

            var generator = new OpenApiDocumentGenerator();

            var result = generator.GenerateOpenApiDocuments(
                document,
                inputBinaryFiles,
                openApiSpecVersion);

            result.Should().NotBeNull();

            _output.WriteLine(
                JsonConvert.SerializeObject(
                    result.ToDocumentGenerationResultSerializedDocument(openApiSpecVersion, OpenApiFormat.Json)));

            result.GenerationStatus.Should().Be(GenerationStatus.Success);
            result.MainDocument.Should().NotBeNull();
            result.OperationGenerationResults.Count.Should().Be(expectedOperationGenerationResultsCount);

            var actualDocument = result.MainDocument.SerializeAsJson(openApiSpecVersion);

            var expectedDocument = File.ReadAllText(expectedJsonFile);

            _output.WriteLine(actualDocument);

            var openApiStringReader = new OpenApiStringReader();
            openApiStringReader.Read(actualDocument, out var _)
                .Should()
                .BeEquivalentTo(
                    openApiStringReader.Read(expectedDocument, out var _));
        }

        [Theory]
        [MemberData(nameof(GetTestCasesForValidDocumentationShouldReturnCorrectSerializedDocument))]
        public void ValidDocumentationShouldReturnCorrectSerializedDocument(
            string testCaseName,
            string inputXmlFile,
            IList<string> inputBinaryFiles,
            OpenApiSpecVersion openApiSpecVersion,
            OpenApiFormat openApiFormat,
            int expectedOperationGenerationResultsCount,
            string expectedJsonFile)
        {
            _output.WriteLine(testCaseName);

            var document = XDocument.Load(inputXmlFile);

            var generator = new OpenApiDocumentGenerator();

            var result = generator.GenerateSerializedOpenApiDocuments(
                document,
                inputBinaryFiles,
                openApiSpecVersion,
                openApiFormat);

            result.Should().NotBeNull();

            _output.WriteLine(JsonConvert.SerializeObject(result));

            result.GenerationStatus.Should().Be(GenerationStatus.Success);
            result.MainDocument.Should().NotBeNull();
            result.OperationGenerationResults.Count.Should().Be(expectedOperationGenerationResultsCount);

            var actualDocument = result.MainDocument;

            var expectedDocument = File.ReadAllText(expectedJsonFile);

            _output.WriteLine(actualDocument);

            var openApiStringReader = new OpenApiStringReader();
            openApiStringReader.Read(actualDocument, out var _)
                .Should()
                .BeEquivalentTo(
                    openApiStringReader.Read(expectedDocument, out var _));
        }
    }
}