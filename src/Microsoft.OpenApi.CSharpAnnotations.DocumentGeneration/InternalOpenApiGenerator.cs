// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.DocumentConfigFilters;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.DocumentFilters;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Exceptions;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Extensions;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.OperationConfigFilters;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.OperationFilters;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.PostProcessingDocumentFilters;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.PreprocessingOperationFilters;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.ReferenceRegistries;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration
{
    /// <summary>
    /// Provides functionality to parse xml documentation and contract assemblies into OpenAPI documents.
    /// </summary>
    internal class InternalOpenApiGenerator
    {
        private readonly IList<DocumentFilter> _documentFilters;
        private readonly IList<DocumentConfigFilter> _documentConfigFilters;
        private readonly IList<OperationFilter> _operationFilters;
        private readonly IList<OperationConfigFilter> _operationConfigFilters;
        private readonly IList<PreProcessingOperationFilter> _preProcessingOperationFilters;
        private readonly IList<PostProcessingDocumentFilter> _postProcessingDocumentFilters;

        /// <summary>
        /// Creates a new instance of <see cref="InternalOpenApiGenerator"/>.
        /// </summary>
        /// <param name="openApiGeneratorFilterConfig">The configuration encapsulating all the filters
        /// that will be applied while generating/processing OpenAPI document from C# annotations.</param>
        public InternalOpenApiGenerator(OpenApiGeneratorFilterConfig openApiGeneratorFilterConfig)
        {
            var openApiGeneratorFilterConfig1 = openApiGeneratorFilterConfig;

            _documentFilters = FetchType<DocumentFilter>(openApiGeneratorFilterConfig1.Filters);
            _documentConfigFilters = FetchType<DocumentConfigFilter>(openApiGeneratorFilterConfig1.Filters);
            _operationFilters = FetchType<OperationFilter>(openApiGeneratorFilterConfig1.Filters);
            _operationConfigFilters = FetchType<OperationConfigFilter>(openApiGeneratorFilterConfig1.Filters);
            _preProcessingOperationFilters = FetchType<PreProcessingOperationFilter>(
                openApiGeneratorFilterConfig1.Filters);
            _postProcessingDocumentFilters = FetchType<PostProcessingDocumentFilter>(
                openApiGeneratorFilterConfig1.Filters);
        }

        /// <summary>
        /// Add operation and update the operation filter settings based on the given document variant info.
        /// </summary>
        private void AddOperation(
            IDictionary<DocumentVariantInfo, OpenApiDocument> specificationDocuments,
            IDictionary<DocumentVariantInfo, ReferenceRegistryManager> referenceRegistryManagerMap,
            IList<GenerationError> operationGenerationErrors,
            DocumentVariantInfo documentVariantInfo,
            XElement operationElement,
            XElement operationConfigElement,
            TypeFetcher typeFetcher)
        {
            var paths = new OpenApiPaths();

            foreach (var preprocessingOperationFilter in _preProcessingOperationFilters)
            {
                try
                {
                    preprocessingOperationFilter.Apply(
                        paths,
                        operationElement,
                        new PreProcessingOperationFilterSettings());
                }
                catch (Exception e)
                {
                    operationGenerationErrors.Add(
                        new GenerationError
                        {
                            ExceptionType = e.GetType().Name,
                            Message = e.Message
                        }
                    );
                }
            }

            if (!referenceRegistryManagerMap.ContainsKey(documentVariantInfo))
            {
                referenceRegistryManagerMap[documentVariantInfo] = new ReferenceRegistryManager();
            }

            foreach (var pathToPathItem in paths)
            {
                var path = pathToPathItem.Key;
                var pathItem = pathToPathItem.Value;

                foreach (var operationMethodToOperation in pathItem.Operations)
                {
                    var operationMethod = operationMethodToOperation.Key;
                    var operation = operationMethodToOperation.Value;

                    var operationFilterSettings = new OperationFilterSettings
                    {
                        TypeFetcher = typeFetcher,
                        ReferenceRegistryManager = referenceRegistryManagerMap[documentVariantInfo],
                        Path = path,
                        OperationMethod = operationMethod.ToString()
                    };

                    // Apply all the operation-related filters to extract information related to the operation.
                    // It is important that these are applied before the config filters below
                    // since the config filters may rely on information generated from operation filters.
                    foreach (var operationFilter in _operationFilters)
                    {
                        try
                        {
                            operationFilter.Apply(
                                operation,
                                operationElement,
                                operationFilterSettings);
                        }
                        catch (Exception e)
                        {
                            operationGenerationErrors.Add(
                                new GenerationError
                                {
                                    ExceptionType = e.GetType().Name,
                                    Message = e.Message
                                }
                            );
                        }
                    }

                    if (operationConfigElement != null)
                    {
                        // Apply the config-related filters to extract information from the config xml
                        // that can be applied to the operations.
                        foreach (var configFilter in _operationConfigFilters)
                        {
                            try
                            {
                                configFilter.Apply(
                                    operation,
                                    operationConfigElement,
                                    new OperationConfigFilterSettings
                                    {
                                        OperationFilterSettings = operationFilterSettings,
                                        OperationFilters = _operationFilters
                                    });
                            }
                            catch (Exception e)
                            {
                                operationGenerationErrors.Add(
                                    new GenerationError
                                    {
                                        ExceptionType = e.GetType().Name,
                                        Message = e.Message
                                    }
                                );
                            }
                        }
                    }

                    // Add the processed operation to the specification document.
                    if (!specificationDocuments.ContainsKey(documentVariantInfo))
                    {
                        specificationDocuments.Add(
                            documentVariantInfo,
                            new OpenApiDocument
                            {
                                Components = new OpenApiComponents()
                            });
                    }

                    // Copy operations from local Paths object to the Paths in the specification document.
                    var documentPaths = specificationDocuments[documentVariantInfo].Paths;

                    if (!documentPaths.ContainsKey(path))
                    {
                        documentPaths.Add(
                            path,
                            new OpenApiPathItem
                            {
                                Operations =
                                {
                                    [operationMethod] = operation
                                }
                            });
                    }
                    else
                    {
                        if (documentPaths[path].Operations.ContainsKey(operationMethod))
                        {
                            throw new DuplicateOperationException(path, operationMethod.ToString());
                        }

                        documentPaths[path].Operations.Add(operationMethod, operation);
                    }
                }
            }
        }

        /// <summary>
        /// Takes in annotation xml document and returns the OpenAPI document generation result
        /// which contains OpenAPI specification document(s).
        /// </summary>
        /// <param name="annotationXmlDocuments">The list of XDocuments representing annotation xmls.</param>
        /// <param name="contractAssemblyPaths">The contract assembly paths.</param>
        /// <param name="configurationXml">The serialized XDocument representing the generation configuration.</param>
        /// <param name="openApiDocumentVersion">The version of the OpenAPI document.</param>
        /// <param name="generationDiagnostic">A string representing serialized version of
        /// <see cref="GenerationDiagnostic"/>>
        /// </param>
        /// <returns>
        /// Dictionary mapping document variant metadata to their respective OpenAPI document.
        /// </returns>
        public IDictionary<DocumentVariantInfo, OpenApiDocument> GenerateOpenApiDocuments(
            IList<XDocument> annotationXmlDocuments,
            IList<string> contractAssemblyPaths,
            string configurationXml,
            string openApiDocumentVersion,
            out GenerationDiagnostic generationDiagnostic)
        {
            IDictionary<DocumentVariantInfo, OpenApiDocument> openApiDocuments
                = new Dictionary<DocumentVariantInfo, OpenApiDocument>();

            var operationElements = new List<XElement>();

            foreach (var annotationXmlDocument in annotationXmlDocuments)
            {
                operationElements.AddRange(
                    annotationXmlDocument.XPathSelectElements("//doc/members/member[url and verb]"));
            }

            XElement operationConfigElement = null;

            XElement documentConfigElement = null;

            var documentVariantElementNames = new List<string>();

            if (!string.IsNullOrWhiteSpace(configurationXml))
            {
                var configurationXmlDocument = XDocument.Parse(configurationXml);

                operationConfigElement = configurationXmlDocument.XPathSelectElement("//configuration/operation");
                documentConfigElement = configurationXmlDocument.XPathSelectElement("//configuration/document");
                documentVariantElementNames = configurationXmlDocument
                    .XPathSelectElements("//configuration/document/variant/name")
                    .Select(variantName => variantName.Value)
                    .ToList();
            }

            if (!operationElements.Any())
            {
                generationDiagnostic = new GenerationDiagnostic()
                {
                    DocumentGenerationDiagnostic = new DocumentGenerationDiagnostic
                    {
                        Errors =
                        {
                            new GenerationError
                            {
                                Message = SpecificationGenerationMessages.NoOperationElementFoundToParse
                            }
                        }
                    }
                };

                return openApiDocuments;
            }

            try
            {
                generationDiagnostic = new GenerationDiagnostic();
                var documentGenerationDiagnostic = new DocumentGenerationDiagnostic();

                if (documentVariantElementNames?.Count > 1)
                {
                    documentGenerationDiagnostic.Errors.Add(new GenerationError
                    {
                        Message = string.Format(
                            SpecificationGenerationMessages.MoreThanOneVariantNameNotAllowed,
                            documentVariantElementNames.First())
                    });
                }

                var typeFetcher = new TypeFetcher(contractAssemblyPaths);

                var operationGenerationDiagnostics = GenerateSpecificationDocuments(
                    typeFetcher,
                    operationElements,
                    operationConfigElement,
                    documentVariantElementNames.FirstOrDefault(),
                    out var documents);

                foreach (var operationGenerationDiagnostic in operationGenerationDiagnostics)
                {
                    generationDiagnostic.OperationGenerationDiagnostics.Add(
                        new OperationGenerationDiagnostic(operationGenerationDiagnostic));
                }

                foreach (var variantInfoDocumentValuePair in documents)
                {
                    var openApiDocument = variantInfoDocumentValuePair.Value;

                    foreach (var documentFilter in _documentFilters)
                    {
                        try
                        {
                            documentFilter.Apply(
                                openApiDocument,
                                annotationXmlDocuments,
                                new DocumentFilterSettings
                                {
                                    TypeFetcher = typeFetcher,
                                    OpenApiDocumentVersion = openApiDocumentVersion
                                });
                        }
                        catch (Exception e)
                        {
                            documentGenerationDiagnostic.Errors.Add(
                                new GenerationError
                                {
                                    ExceptionType = e.GetType().Name,
                                    Message = e.Message
                                });
                        }
                    }

                    foreach (var filter in _postProcessingDocumentFilters)
                    {
                        filter.Apply(
                            openApiDocument,
                            new PostProcessingDocumentFilterSettings()
                            {
                                OperationGenerationDiagnostics = operationGenerationDiagnostics
                            });
                    }
                }

                if (documentConfigElement != null)
                {
                    foreach (var documentConfigFilter in _documentConfigFilters)
                    {
                        try
                        {
                            documentConfigFilter.Apply(
                                documents,
                                documentConfigElement,
                                annotationXmlDocuments,
                                new DocumentConfigFilterSettings());
                        }
                        catch (Exception e)
                        {
                            documentGenerationDiagnostic.Errors.Add(
                                new GenerationError
                                {
                                    ExceptionType = e.GetType().Name,
                                    Message = e.Message
                                });
                        }
                    }
                }

                var failedOperations = generationDiagnostic.OperationGenerationDiagnostics
                    .Where(i => i.Errors.Count > 0);

                if (failedOperations.Any())
                {
                    var totalOperationsCount = generationDiagnostic.OperationGenerationDiagnostics.Count();

                    var exception = new UnableToGenerateAllOperationsException(
                        totalOperationsCount - failedOperations.Count(), totalOperationsCount);

                    documentGenerationDiagnostic.Errors.Add(
                        new GenerationError
                        {
                            ExceptionType = exception.GetType().Name,
                            Message = exception.Message
                        });
                }

                generationDiagnostic.DocumentGenerationDiagnostic = documentGenerationDiagnostic;
                return documents;
            }
            catch (Exception e)
            {
                generationDiagnostic = new GenerationDiagnostic
                {
                    DocumentGenerationDiagnostic =
                        new DocumentGenerationDiagnostic
                        {
                            Errors =
                            {
                                new GenerationError
                                {
                                    ExceptionType = e.GetType().Name,
                                    Message = string.Format(SpecificationGenerationMessages.UnexpectedError, e)
                                }
                            }
                        }
                };

                return openApiDocuments;
            }
        }

        /// <summary>
        /// Populate the specification documents for all document variant infos.
        /// </summary>
        /// <returns>The operation generation results from populating the specification documents.</returns>
        private IList<OperationGenerationDiagnostic> GenerateSpecificationDocuments(
            TypeFetcher typeFetcher,
            IList<XElement> operationElements,
            XElement operationConfigElement,
            string documentVariantElementName,
            out IDictionary<DocumentVariantInfo, OpenApiDocument> specificationDocuments)
        {
            specificationDocuments = new Dictionary<DocumentVariantInfo, OpenApiDocument>();

            var operationGenerationResults = new List<OperationGenerationDiagnostic>();

            var referenceRegistryManagerMap = new Dictionary<DocumentVariantInfo, ReferenceRegistryManager>();

            foreach (var operationElement in operationElements)
            {
                string url;
                OperationType operationMethod;

                try
                {
                    url = OperationHandler.GetUrl(operationElement);
                }
                catch (InvalidUrlException e)
                {
                    operationGenerationResults.Add(
                        new OperationGenerationDiagnostic
                        {
                            Errors =
                            {
                                new GenerationError
                                {
                                    ExceptionType = e.GetType().Name,
                                    Message = e.Message
                                }
                            },
                            OperationMethod = SpecificationGenerationMessages.OperationMethodNotParsedGivenUrlIsInvalid,
                            Path = e.Url
                        });

                    continue;
                }

                try
                {
                    operationMethod = OperationHandler.GetOperationMethod(url, operationElement);
                }
                catch (InvalidVerbException e)
                {
                    operationGenerationResults.Add(
                        new OperationGenerationDiagnostic
                        {
                            Errors =
                            {
                                new GenerationError
                                {
                                    ExceptionType = e.GetType().Name,
                                    Message = e.Message
                                }
                            },
                            OperationMethod = e.Verb,
                            Path = url
                        });

                    continue;
                }

                try
                {
                    var operationGenerationErrors = new List<GenerationError>();

                    AddOperation(
                        specificationDocuments,
                        referenceRegistryManagerMap,
                        operationGenerationErrors,
                        DocumentVariantInfo.Default,
                        operationElement,
                        operationConfigElement,
                        typeFetcher);

                    var customElements = operationElement.Descendants(documentVariantElementName);
                    foreach (var customElement in customElements)
                    {
                        var documentVariantInfo = new DocumentVariantInfo
                        {
                            Categorizer = customElement.Name.LocalName.Trim(),
                            Title = customElement.Value.Trim()
                        };

                        AddOperation(
                            specificationDocuments,
                            referenceRegistryManagerMap,
                            operationGenerationErrors,
                            documentVariantInfo,
                            operationElement,
                            operationConfigElement,
                            typeFetcher);
                    }

                    var operationGenerationResult = new OperationGenerationDiagnostic
                    {
                        OperationMethod = operationMethod.ToString(),
                        Path = url
                    };

                    if (operationGenerationErrors.Any())
                    {
                        foreach (var error in operationGenerationErrors)
                        {
                            operationGenerationResult.Errors.Add(new GenerationError(error));
                        }
                    }

                    operationGenerationResults.Add(operationGenerationResult);
                }
                catch (Exception e)
                {
                    operationGenerationResults.Add(
                        new OperationGenerationDiagnostic
                        {
                            Errors =
                            {
                                new GenerationError
                                {
                                    ExceptionType = e.GetType().Name,
                                    Message = e.Message
                                }
                            },
                            OperationMethod = operationMethod.ToString(),
                            Path = url
                        });
                }
            }

            foreach (var documentVariantInfo in specificationDocuments.Keys)
            {
                referenceRegistryManagerMap[documentVariantInfo]
                    .SchemaReferenceRegistry.References.CopyInto(
                        specificationDocuments[documentVariantInfo].Components.Schemas);
            }

            return operationGenerationResults;
        }

        private List<T> FetchType<T>(FilterSet filters) where T : IFilter
        {
            var typedFilers = filters.Where(i => i.FilterType == typeof(T));
            var test = new List<T>();

            foreach (var filter in typedFilers)
            {
                if (filter is T filter1)
                {
                    test.Add(filter1);
                }
            }

            return test;
        }
    }
}