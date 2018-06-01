// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.IO;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Exceptions;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Extensions;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.DocumentVariantTests
{
    internal static class DocumentVariantTestCases
    {
        private const string InputDirectory = "DocumentVariantTests/Input";
        private const string OutputDirectory = "DocumentVariantTests/Output";

        private const string CommonAnnotationsDirectory =
            "AnnotationsWithCommonAnnotations";

        private const string AnnotationsDuplicatePathDirectory =
            "AnnotationsWithDuplicatePaths";

        public static IEnumerable<object[]> GetTestCasesForGenerateDocumentMultipleVariantsShouldSucceed()
        {
            // One document variant tag name
            yield return new object[]
            {
                "One document variant tag name",
                new List<string>
                {
                    Path.Combine(InputDirectory, "Annotation.xml"),
                    Path.Combine(InputDirectory,
                        "Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.Contracts.xml")
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
                    "ConfigOneDocumentVariantTag.xml"),
                9,
                new Dictionary<DocumentVariantInfo, string>
                {
                    [DocumentVariantInfo.Default] =
                    Path.Combine(OutputDirectory, "AnnotationDefaultVariant.json"),
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
                    }] = Path.Combine(OutputDirectory, "AnnotationVariantSwaggerGroup1.json"),
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
                    }] = Path.Combine(OutputDirectory, "AnnotationVariantSwaggerGroup2.json")
                }
            };

            // One document variant tag name with no option tags.
            // Everything should still be as if the option tags were present, except that the attributes
            // should not be populated.
            yield return new object[]
            {
                "One document variant tag name with no option tags",
                new List<string>
                {
                    Path.Combine(InputDirectory, "Annotation.xml"),
                    Path.Combine(InputDirectory,
                        "Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.Contracts.xml")
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
                    "ConfigOneDocumentVariantTagSwaggerNoOptions.xml"),
                9,
                new Dictionary<DocumentVariantInfo, string>
                {
                    [DocumentVariantInfo.Default] =
                    Path.Combine(OutputDirectory, "AnnotationDefaultVariant.json"),
                    [new DocumentVariantInfo
                    {
                        // Only contains info from operations with swagger tags with title "Group1" in Annotation.xml 
                        Categorizer = "swagger",
                        Title = "Group1"
                    }] = Path.Combine(OutputDirectory, "AnnotationVariantSwaggerGroup1.json"),
                    [new DocumentVariantInfo
                    {
                        // Only contains info from operations with swagger tags with title "Group2" in Annotation.xml 
                        Categorizer = "swagger",
                        Title = "Group2"
                    }] = Path.Combine(OutputDirectory, "AnnotationVariantSwaggerGroup2.json")
                }
            };

            // Document variant info inside document annotation with no options in the config
            // The result should succeed and be populated with the information from the annotation.
            yield return new object[]
            {
                "Document variant info inside document annotation with no options in the config",
                new List<string>
                {
                    Path.Combine(InputDirectory, "AnnotationWithVariantAttributes.xml"),
                    Path.Combine(InputDirectory,
                        "Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.Contracts.xml")
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
                    "ConfigOneDocumentVariantTagSwaggerNoOptions.xml"),
                9,
                new Dictionary<DocumentVariantInfo, string>
                {
                    [DocumentVariantInfo.Default] = Path.Combine(
                        OutputDirectory,
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
                        OutputDirectory,
                        "AnnotationVariantSwaggerGroup1.json"),
                    [new DocumentVariantInfo
                    {
                        // Only contains info from operations with swagger tags with title "Group2" in Annotation.xml 
                        Categorizer = "swagger",
                        Title = "Group2"
                    }] = Path.Combine(
                        OutputDirectory,
                        "AnnotationVariantSwaggerGroup2.json")
                }
            };

            // Document variant info inside document annotation with redundant, non-conflicting information in the config
            // The result should succeed and be populated with the information from the config, which
            // is identical to the information in the annotation.
            yield return new object[]
            {
                "Document variant info inside document annotation with redundant, non-conflicting information in the config",
                new List<string>
                {
                    Path.Combine(InputDirectory, "AnnotationWithVariantAttributes.xml"),
                    Path.Combine(InputDirectory,
                        "Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.Contracts.xml")
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
                    "ConfigOneDocumentVariantTag.xml"),
                9,
                new Dictionary<DocumentVariantInfo, string>
                {
                    [DocumentVariantInfo.Default] = Path.Combine(
                        OutputDirectory,
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
                        OutputDirectory,
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
                        OutputDirectory,
                        "AnnotationVariantSwaggerGroup2.json")
                }
            };
        }

        public static IEnumerable<object[]> GetTestCasesForGenerateDocumentMultipleVariantsShouldYieldFailure()
        {
            // Document variant info inside document annotation that self-conflicts.
            yield return new object[]
            {
                "Document variant info inside document annotation that self-conflicts",
                new List<string>
                {
                    Path.Combine(InputDirectory, "AnnotationWithVariantAttributesConflictWithSelf.xml"),
                    Path.Combine(InputDirectory,
                        "Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.Contracts.xml")
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
                    "ConfigOneDocumentVariantTagSwaggerNoOptions.xml"),
                9,
                new Dictionary<DocumentVariantInfo, string>
                {
                    [DocumentVariantInfo.Default] =
                    Path.Combine(OutputDirectory, "AnnotationDefaultVariant.json"),
                    [new DocumentVariantInfo
                    {
                        // Only contains info from operations with swagger tags with title "Group1" in Annotation.xml
                        // Attributes that appear first in the operation show up in the document.
                        Categorizer = "swagger",
                        Title = "Group1",
                        Attributes =
                        {
                            ["security"] = "sg1",
                            ["version"] = "V2"
                        }
                    }] = Path.Combine(OutputDirectory, "AnnotationVariantSwaggerGroup1.json")
                },
                new DocumentGenerationDiagnostic
                {
                    Errors =
                    {
                        new GenerationError
                        {
                            ExceptionType = typeof(ConflictingDocumentVariantAttributesException).Name,
                            Message = string.Format(
                                SpecificationGenerationMessages.ConflictingDocumentVariantAttributes,
                                "swagger",
                                "Group1",
                                new Dictionary<string, string>
                                {
                                    ["security"] = "sg1",
                                    ["version"] = "V2"
                                }.ToSerializedString(),
                                new Dictionary<string, string>
                                {
                                    ["security"] = "sg1",
                                    ["version"] = "VConflict"
                                }.ToSerializedString())
                        }
                    }
                },
                new List<OperationGenerationDiagnostic>()
            };

            // Document variant info inside document annotation that conflicts with the config.
            yield return new object[]
            {
                "Document variant info inside document annotation that conflicts with the config",
                new List<string>
                {
                    Path.Combine(InputDirectory, "AnnotationWithVariantAttributesConflictWithConfig.xml"),
                    Path.Combine(InputDirectory,
                        "Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.Contracts.xml")
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
                    "ConfigOneDocumentVariantTag.xml"),
                9,
                new Dictionary<DocumentVariantInfo, string>
                {
                    [DocumentVariantInfo.Default] =
                    Path.Combine(OutputDirectory, "AnnotationDefaultVariant.json"),
                    [new DocumentVariantInfo
                    {
                        // Only contains info from operations with swagger tags with title "Group1" in Annotation.xml
                        // Attributes from the config file takes precedence when conflicts occurs.
                        Categorizer = "swagger",
                        Title = "Group1",
                        Attributes =
                        {
                            ["security"] = "sg1",
                            ["version"] = "V2"
                        }
                    }] = Path.Combine(OutputDirectory, "AnnotationVariantSwaggerGroup1.json"),
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
                    }] = Path.Combine(OutputDirectory, "AnnotationVariantSwaggerGroup2.json")
                },
                new DocumentGenerationDiagnostic
                {
                    Errors =
                    {
                        new GenerationError
                        {
                            ExceptionType = typeof(ConflictingDocumentVariantAttributesException).Name,
                            Message = string.Format(
                                SpecificationGenerationMessages.ConflictingDocumentVariantAttributes,
                                "swagger",
                                "Group1",
                                new Dictionary<string, string>
                                {
                                    ["security"] = "sg1",
                                    ["version"] = "V2"
                                }.ToSerializedString(),
                                new Dictionary<string, string>
                                {
                                    ["security"] = "sg1",
                                    ["version"] = "VConflict"
                                }.ToSerializedString())
                        }
                    }
                },
                new List<OperationGenerationDiagnostic>()
            };

            // Multiple document variant tag names with common annotations
            yield return new object[]
            {
                "Multiple document variant tag names with common annotations",
                new List<string>
                {
                    Path.Combine(InputDirectory, "Annotation.xml"),
                    Path.Combine(InputDirectory,
                        "Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.Contracts.xml")
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
                    "ConfigMultipleDocumentVariantTagsWithCommonAnnotations.xml"),
                9,
                new Dictionary<DocumentVariantInfo, string>
                {
                    [DocumentVariantInfo.Default] = Path.Combine(
                        OutputDirectory,
                        CommonAnnotationsDirectory,
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
                        OutputDirectory,
                        CommonAnnotationsDirectory,
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
                        OutputDirectory,
                        CommonAnnotationsDirectory,
                        "AnnotationVariantSwaggerGroup2.json")
                },
                new DocumentGenerationDiagnostic
                {
                    Errors =
                    {
                        new GenerationError
                        {
                            Message = string.Format(
                                SpecificationGenerationMessages.MoreThanOneVariantNameNotAllowed,
                                "swagger")
                        }
                    }
                },
                new List<OperationGenerationDiagnostic>()
            };

            // Document variant info with duplicate paths for a variant.
            yield return new object[]
            {
                "Document variant info with duplicate paths for a variant",
                new List<string>
                {
                    Path.Combine(InputDirectory, "AnnotationWithDuplicatePaths.xml"),
                    Path.Combine(InputDirectory,
                        "Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.Contracts.xml")
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
                    "ConfigOneDocumentVariantTagSwaggerNoOptions.xml"),
                3,
                new Dictionary<DocumentVariantInfo, string>
                {
                    [DocumentVariantInfo.Default] =
                    Path.Combine(OutputDirectory, AnnotationsDuplicatePathDirectory, "AnnotationDefaultVariant.json"),
                    [new DocumentVariantInfo
                    {
                        Categorizer = "swagger",
                        Title = "Group1"
                    }] = Path.Combine(OutputDirectory, AnnotationsDuplicatePathDirectory,
                        "AnnotationVariantSwaggerGroup1.json"),
                    [new DocumentVariantInfo
                    {
                        Categorizer = "swagger",
                        Title = "Group2"
                    }] = Path.Combine(OutputDirectory, AnnotationsDuplicatePathDirectory,
                        "AnnotationVariantSwaggerGroup2.json")
                },
                new DocumentGenerationDiagnostic
                {
                    Errors =
                    {
                        new GenerationError
                        {
                            ExceptionType = typeof(UnableToGenerateAllOperationsException).Name,
                            Message = string.Format(
                                SpecificationGenerationMessages.UnableToGenerateAllOperations,
                                3,
                                4)
                        }
                    }
                },
                new List<OperationGenerationDiagnostic>
                {
                    new OperationGenerationDiagnostic
                    {
                        OperationMethod = OperationType.Get.ToString(),
                        Path = "/V1/samples/{id}",
                        Errors =
                        {
                            new GenerationError
                            {
                                ExceptionType = typeof(DuplicateOperationException).Name,
                                Message = string.Format(
                                    SpecificationGenerationMessages.DuplicateOperation,
                                    OperationType.Get,
                                    "/V1/samples/{id}")
                            },
                            new GenerationError
                            {
                                ExceptionType = typeof(DuplicateOperationException).Name,
                                Message = string.Format(
                                    SpecificationGenerationMessages.DuplicateOperationWithVariantInfo,
                                    OperationType.Get,
                                    "/V1/samples/{id}",
                                    "Group1")
                            }
                        }
                    }
                }
            };
        }
    }
}