// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using Microsoft.OpenApiSpecification.Generation.Exceptions;
using Microsoft.OpenApiSpecification.Generation.Models;
using Newtonsoft.Json;

namespace Microsoft.OpenApiSpecification.Generation.Tests.DocumentVariantTests
{
    internal static class DocumentVariantTestCases
    {
        private const string TestFilesDirectory = "DocumentVariantTests/TestFiles";
        private const string TestValidationDirectory = "DocumentVariantTests/TestValidation";

        private const string TestValidationWithCommonAnnotationsDirectory =
            "DocumentVariantTests/TestValidation/AnnotationsWithCommonAnnotations";

        public static IEnumerable<object[]> GetTestCasesForGenerateDocumentMultipleVariantsShouldFail()
        {
            // Document variant info inside document annotation that self-conflicts.
            yield return new object[]
            {
                "Document variant info inside document annotation that self-conflicts",
                Path.Combine(TestFilesDirectory, "AnnotationWithVariantAttributesConflictWithSelf.xml"),
                new List<string>
                {
                    Path.Combine(
                        TestFilesDirectory,
                        "OpenApiSpecification.UnitTestSamples.DotNetFrameworkController.dll")
                },
                Path.Combine(
                    TestFilesDirectory,
                    "ConfigOneDocumentVariantTagSwagger2NoOptions.xml"),
                1,
                null,
                new List<PathGenerationResult>
                {
                    new PathGenerationResult
                    {
                        ExceptionType = typeof(ConflictingDocumentVariantAttributesException),
                        Message = string.Format(
                            SpecificationGenerationMessages.ConflictingDocumentVariantAttributes,
                            "swagger2",
                            "Group1",
                            JsonConvert.SerializeObject(
                                new Dictionary<string, string>
                                {
                                    ["security"] = "sg1",
                                    ["version"] = "V2"
                                }),
                            JsonConvert.SerializeObject(
                                new Dictionary<string, string>
                                {
                                    ["security"] = "sg1",
                                    ["version"] = "VConflict"
                                })),
                        GenerationStatus = GenerationStatus.Failure
                    }
                }
            };

            // Document variant info inside document annotation that conflicts with the config.
            yield return new object[]
            {
                "Document variant info inside document annotation that conflicts with the config",
                Path.Combine(TestFilesDirectory, "AnnotationWithVariantAttributesConflictWithConfig.xml"),
                new List<string>
                {
                    Path.Combine(
                        TestFilesDirectory,
                        "OpenApiSpecification.UnitTestSamples.DotNetFrameworkController.dll")
                },
                Path.Combine(
                    TestFilesDirectory,
                    "ConfigOneDocumentVariantTag.xml"),
                1,
                null,
                new List<PathGenerationResult>
                {
                    new PathGenerationResult
                    {
                        ExceptionType = typeof(ConflictingDocumentVariantAttributesException),
                        Message = string.Format(
                            SpecificationGenerationMessages.ConflictingDocumentVariantAttributes,
                            "swagger",
                            "Group1",
                            JsonConvert.SerializeObject(
                                new Dictionary<string, string>
                                {
                                    ["security"] = "sg1",
                                    ["version"] = "V2"
                                }),
                            JsonConvert.SerializeObject(
                                new Dictionary<string, string>
                                {
                                    ["security"] = "sg1",
                                    ["version"] = "VConflict"
                                })),
                        GenerationStatus = GenerationStatus.Failure
                    }
                }
            };
        }

        public static IEnumerable<object[]> GetTestCasesForGenerateDocumentMultipleVariantsShouldSucceed()
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
                new Dictionary<DocumentVariantInfo, string>
                {
                    [DocumentVariantInfo.Default] =
                    Path.Combine(TestValidationDirectory, "AnnotationDefaultVariant.json"),
                    [new DocumentVariantInfo
                    {
                        // Only contains info from operations with swagger tags with title "Group1" in Annotation.xml 
                        Categorizer = "swagger",
                        Title = "Group1",
                        Attributes =
                        {
                            ["security"] = "sg1",
                            ["version"] = "V2"
                        }
                    }] = Path.Combine(TestValidationDirectory, "AnnotationVariantSwaggerGroup1.json"),
                    [new DocumentVariantInfo
                    {
                        // Only contains info from operations with swagger tags with title "Group2" in Annotation.xml 
                        Categorizer = "swagger",
                        Title = "Group2",
                        Attributes =
                        {
                            ["security"] = "sgnotexist",
                            ["version"] = "V2"
                        }
                    }] = Path.Combine(TestValidationDirectory, "AnnotationVariantSwaggerGroup2.json"),
                },
            };

            // One document variant tag name with no option tags.
            // Everything should still be as if the option tags were present, except that the attributes
            // should not be populated.
            yield return new object[]
            {
                "One document variant tag name with no option tags.",
                Path.Combine(TestFilesDirectory, "Annotation.xml"),
                new List<string>
                {
                    Path.Combine(
                        TestFilesDirectory,
                        "OpenApiSpecification.UnitTestSamples.DotNetFrameworkController.dll")
                },
                Path.Combine(
                    TestFilesDirectory,
                    "ConfigOneDocumentVariantTagSwaggerNoOptions.xml"),
                9,
                new Dictionary<DocumentVariantInfo, string>
                {
                    [DocumentVariantInfo.Default] =
                    Path.Combine(TestValidationDirectory, "AnnotationDefaultVariant.json"),
                    [new DocumentVariantInfo
                    {
                        // Only contains info from operations with swagger tags with title "Group1" in Annotation.xml 
                        Categorizer = "swagger",
                        Title = "Group1"
                    }] = Path.Combine(TestValidationDirectory, "AnnotationVariantSwaggerGroup1.json"),
                    [new DocumentVariantInfo
                    {
                        // Only contains info from operations with swagger tags with title "Group2" in Annotation.xml 
                        Categorizer = "swagger",
                        Title = "Group2",
                    }] = Path.Combine(TestValidationDirectory, "AnnotationVariantSwaggerGroup2.json"),
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
                new Dictionary<DocumentVariantInfo, string>
                {
                    [DocumentVariantInfo.Default] =
                    Path.Combine(TestValidationDirectory, "AnnotationDefaultVariant.json"),
                    [new DocumentVariantInfo
                    {
                        // Only contains info from operations with swagger tags with title "Group1" in Annotation.xml 
                        Categorizer = "swagger",
                        Title = "Group1",
                        Attributes =
                        {
                            ["security"] = "sg1",
                            ["version"] = "V2"
                        }
                    }] = Path.Combine(TestValidationDirectory, "AnnotationVariantSwaggerGroup1.json"),
                    [new DocumentVariantInfo
                    {
                        // Only contains info from operations with swagger tags with title "Group2" in Annotation.xml 
                        Categorizer = "swagger",
                        Title = "Group2",
                        Attributes =
                        {
                            ["security"] = "sgnotexist",
                            ["version"] = "V2"
                        }
                    }] = Path.Combine(TestValidationDirectory, "AnnotationVariantSwaggerGroup2.json"),
                    [new DocumentVariantInfo
                    {
                        // Only contains info from operations with swagger tags with title "Group1" in Annotation.xml 
                        Categorizer = "swagger2",
                        Title = "Group1",
                        Attributes =
                        {
                            ["security"] = "sg1",
                            ["version"] = "V2"
                        }
                    }] = Path.Combine(TestValidationDirectory, "AnnotationVariantSwagger2Group1.json"),
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
                new Dictionary<DocumentVariantInfo, string>
                {
                    [DocumentVariantInfo.Default] = Path.Combine(
                        TestValidationWithCommonAnnotationsDirectory,
                        "AnnotationDefaultVariant.json"),
                    [new DocumentVariantInfo
                    {
                        // Only contains info from operations with swagger tags with title "Group1" in Annotation.xml 
                        Categorizer = "swagger",
                        Title = "Group1",
                        Attributes =
                        {
                            ["security"] = "sg1",
                            ["version"] = "V2"
                        }
                    }] = Path.Combine(
                        TestValidationWithCommonAnnotationsDirectory,
                        "AnnotationVariantSwaggerGroup1.json"),
                    [new DocumentVariantInfo
                    {
                        // Only contains info from operations with swagger tags with title "Group2" in Annotation.xml 
                        Categorizer = "swagger",
                        Title = "Group2",
                        Attributes =
                        {
                            ["security"] = "sgnotexist",
                            ["version"] = "V2"
                        }
                    }] = Path.Combine(
                        TestValidationWithCommonAnnotationsDirectory,
                        "AnnotationVariantSwaggerGroup2.json"),
                    [new DocumentVariantInfo
                    {
                        // Only contains info from operations with swagger tags with title "Group1" in Annotation.xml 
                        Categorizer = "swagger2",
                        Title = "Group1",
                        Attributes =
                        {
                            ["security"] = "sg1",
                            ["version"] = "V2"
                        }
                    }] = Path.Combine(
                        TestValidationWithCommonAnnotationsDirectory,
                        "AnnotationVariantSwagger2Group1.json"),
                },
            };

            // Document variant info inside document annotation with no options in the config
            // The result should succeed and be populated with the information from the annotation.
            yield return new object[]
            {
                "Document variant info inside document annotation with no options in the config",
                Path.Combine(TestFilesDirectory, "AnnotationWithVariantAttributes.xml"),
                new List<string>
                {
                    Path.Combine(
                        TestFilesDirectory,
                        "OpenApiSpecification.UnitTestSamples.DotNetFrameworkController.dll")
                },
                Path.Combine(
                    TestFilesDirectory,
                    "ConfigOneDocumentVariantTagSwaggerNoOptions.xml"),
                9,
                new Dictionary<DocumentVariantInfo, string>
                {
                    [DocumentVariantInfo.Default] = Path.Combine(
                        TestValidationDirectory,
                        "AnnotationDefaultVariant.json"),
                    [new DocumentVariantInfo
                    {
                        // Only contains info from operations with swagger tags with title "Group1" in Annotation.xml 
                        Categorizer = "swagger",
                        Title = "Group1",
                        Attributes =
                        {
                            ["security"] = "sg1",
                            ["version"] = "V2"
                        }
                    }] = Path.Combine(
                        TestValidationDirectory,
                        "AnnotationVariantSwaggerGroup1.json"),
                    [new DocumentVariantInfo
                    {
                        // Only contains info from operations with swagger tags with title "Group2" in Annotation.xml 
                        Categorizer = "swagger",
                        Title = "Group2",
                    }] = Path.Combine(
                        TestValidationDirectory,
                        "AnnotationVariantSwaggerGroup2.json"),
                }
            };

            // Document variant info inside document annotation with redundant, non-conflicting information in the config
            // The result should succeed and be populated with the information from the config, which
            // is identical to the information in the annotation.
            yield return new object[]
            {
                "Document variant info inside document annotation with redundant, non-conflicting information in the config",
                Path.Combine(TestFilesDirectory, "AnnotationWithVariantAttributes.xml"),
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
                new Dictionary<DocumentVariantInfo, string>
                {
                    [DocumentVariantInfo.Default] = Path.Combine(
                        TestValidationDirectory,
                        "AnnotationDefaultVariant.json"),
                    [new DocumentVariantInfo
                    {
                        // Only contains info from operations with swagger tags with title "Group1" in Annotation.xml 
                        Categorizer = "swagger",
                        Title = "Group1",
                        Attributes =
                        {
                            ["security"] = "sg1",
                            ["version"] = "V2"
                        }
                    }] = Path.Combine(
                        TestValidationDirectory,
                        "AnnotationVariantSwaggerGroup1.json"),
                    [new DocumentVariantInfo
                    {
                        // Only contains info from operations with swagger tags with title "Group2" in Annotation.xml 
                        Categorizer = "swagger",
                        Title = "Group2",
                        Attributes =
                        {
                            ["security"] = "sgnotexist",
                            ["version"] = "V2"
                        }
                    }] = Path.Combine(
                        TestValidationDirectory,
                        "AnnotationVariantSwaggerGroup2.json"),
                }
            };
        }
    }
}