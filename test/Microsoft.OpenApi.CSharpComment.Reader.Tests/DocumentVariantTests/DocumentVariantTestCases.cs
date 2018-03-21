// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.IO;
using Microsoft.OpenApi.CSharpComment.Reader.Exceptions;
using Microsoft.OpenApi.CSharpComment.Reader.Models;
using Newtonsoft.Json;

namespace Microsoft.OpenApi.CSharpComment.Reader.Tests.DocumentVariantTests
{
    internal static class DocumentVariantTestCases
    {
        private const string InputDirectory = "DocumentVariantTests/Input";
        private const string OutputDirectory = "DocumentVariantTests/Output";

        private const string CommonAnnotationsDirectory =
            "AnnotationsWithCommonAnnotations";

        public static IEnumerable<object[]> GetTestCasesForGenerateDocumentMultipleVariantsShouldYieldFailure()
        {
            // Document variant info inside document annotation that self-conflicts.
            yield return new object[]
            {
                "Document variant info inside document annotation that self-conflicts",
                new List<string>
                {
                    Path.Combine(InputDirectory, "AnnotationWithVariantAttributesConflictWithSelf.xml"),
                    Path.Combine(InputDirectory, "Microsoft.OpenApi.CSharpComment.Reader.Tests.Contracts.xml")
                },
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.SampleApis.dll"),
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.Contracts.dll")
                },
                Path.Combine(
                    InputDirectory,
                    "ConfigOneDocumentVariantTagSwaggerNoOptions.xml"),
                OpenApiSpecVersion.OpenApi3_0,
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
                    }] = Path.Combine(OutputDirectory, "AnnotationVariantSwaggerGroup1.json"),
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
                        }
                    }
                }
            };

            // Document variant info inside document annotation that conflicts with the config.
            yield return new object[]
            {
                "Document variant info inside document annotation that conflicts with the config",
                new List<string>
                {
                    Path.Combine(InputDirectory, "AnnotationWithVariantAttributesConflictWithConfig.xml"),
                    Path.Combine(InputDirectory, "Microsoft.OpenApi.CSharpComment.Reader.Tests.Contracts.xml")
                },
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.SampleApis.dll"),
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.Contracts.dll")
                },
                Path.Combine(
                    InputDirectory,
                    "ConfigOneDocumentVariantTag.xml"),
                OpenApiSpecVersion.OpenApi3_0,
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
                    }] = Path.Combine(OutputDirectory, "AnnotationVariantSwaggerGroup2.json"),
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
                        }
                    }
                }
            };

            // Multiple document variant tag names with common annotations
            yield return new object[]
            {
                "Multiple document variant tag names with common annotations",
                new List<string>
                {
                    Path.Combine(InputDirectory, "Annotation.xml"),
                    Path.Combine(InputDirectory, "Microsoft.OpenApi.CSharpComment.Reader.Tests.Contracts.xml")
                },
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.SampleApis.dll"),
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.Contracts.dll")
                },
                Path.Combine(
                    InputDirectory,
                    "ConfigMultipleDocumentVariantTagsWithCommonAnnotations.xml"),
                OpenApiSpecVersion.OpenApi3_0,
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
                        "AnnotationVariantSwaggerGroup2.json"),
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
                }
            };
        }

        public static IEnumerable<object[]> GetTestCasesForGenerateDocumentMultipleVariantsShouldSucceed()
        {
            // One document variant tag name
            yield return new object[]
            {
                "One document variant tag name",
                new List<string>
                {
                    Path.Combine(InputDirectory, "Annotation.xml"),
                    Path.Combine(InputDirectory, "Microsoft.OpenApi.CSharpComment.Reader.Tests.Contracts.xml")
                },
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.SampleApis.dll"),
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.Contracts.dll")
                },
                Path.Combine(
                    InputDirectory,
                    "ConfigOneDocumentVariantTag.xml"),
                OpenApiSpecVersion.OpenApi3_0,
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
                    }] = Path.Combine(OutputDirectory, "AnnotationVariantSwaggerGroup2.json"),
                },
            };

            // One document variant tag name with no option tags.
            // Everything should still be as if the option tags were present, except that the attributes
            // should not be populated.
            yield return new object[]
            {
                "One document variant tag name with no option tags.",
                new List<string>
                {
                    Path.Combine(InputDirectory, "Annotation.xml"),
                    Path.Combine(InputDirectory, "Microsoft.OpenApi.CSharpComment.Reader.Tests.Contracts.xml")
                },
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.SampleApis.dll"),
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.Contracts.dll")
                },
                Path.Combine(
                    InputDirectory,
                    "ConfigOneDocumentVariantTagSwaggerNoOptions.xml"),
                OpenApiSpecVersion.OpenApi3_0,
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
                        Title = "Group2",
                    }] = Path.Combine(OutputDirectory, "AnnotationVariantSwaggerGroup2.json"),
                },
            };

            // Document variant info inside document annotation with no options in the config
            // The result should succeed and be populated with the information from the annotation.
            yield return new object[]
            {
                "Document variant info inside document annotation with no options in the config",
                new List<string>
                {
                    Path.Combine(InputDirectory, "AnnotationWithVariantAttributes.xml"),
                    Path.Combine(InputDirectory, "Microsoft.OpenApi.CSharpComment.Reader.Tests.Contracts.xml")
                },
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.SampleApis.dll"),
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.Contracts.dll")
                },
                Path.Combine(
                    InputDirectory,
                    "ConfigOneDocumentVariantTagSwaggerNoOptions.xml"),
                OpenApiSpecVersion.OpenApi3_0,
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
                    }] = Path.Combine(
                        OutputDirectory,
                        "AnnotationVariantSwaggerGroup2.json"),
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
                    Path.Combine(InputDirectory, "Microsoft.OpenApi.CSharpComment.Reader.Tests.Contracts.xml")
                },
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.SampleApis.dll"),
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.Contracts.dll")
                },
                Path.Combine(
                    InputDirectory,
                    "ConfigOneDocumentVariantTag.xml"),
                OpenApiSpecVersion.OpenApi3_0,
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
                        "AnnotationVariantSwaggerGroup2.json"),
                }
            };
        }
    }
}