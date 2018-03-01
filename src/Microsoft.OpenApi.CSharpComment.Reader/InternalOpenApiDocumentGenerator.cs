// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.OpenApi.CSharpComment.Reader.DocumentConfigFilters;
using Microsoft.OpenApi.CSharpComment.Reader.DocumentFilters;
using Microsoft.OpenApi.CSharpComment.Reader.Exceptions;
using Microsoft.OpenApi.CSharpComment.Reader.Extensions;
using Microsoft.OpenApi.CSharpComment.Reader.Models;
using Microsoft.OpenApi.CSharpComment.Reader.OperationConfigFilters;
using Microsoft.OpenApi.CSharpComment.Reader.OperationFilters;
using Microsoft.OpenApi.CSharpComment.Reader.PostProcessingDocumentFilters;
using Microsoft.OpenApi.CSharpComment.Reader.PreprocessingOperationFilters;
using Microsoft.OpenApi.CSharpComment.Reader.ReferenceRegistries;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpComment.Reader
{
    /// <summary>
    /// Provides functionality to parse xml documentation and contract assemblies into OpenAPI documents.
    /// </summary>
    internal class InternalOpenApiDocumentGenerator
    {
        private readonly CSharpCommentOpenApiGeneratorFilterConfig _cSharpCommentOpenApiGeneratorFilterConfig;

        /// <summary>
        /// Creates a new instance of <see cref="InternalOpenApiDocumentGenerator"/>.
        /// </summary>
        /// <param name="cSharpCommentOpenApiGeneratorFilterConfig">The configuration encapsulating all the filters
        /// that will be applied while generating/processing OpenAPI document from C# comments.</param>
        public InternalOpenApiDocumentGenerator(
            CSharpCommentOpenApiGeneratorFilterConfig cSharpCommentOpenApiGeneratorFilterConfig)
        {
            _cSharpCommentOpenApiGeneratorFilterConfig = cSharpCommentOpenApiGeneratorFilterConfig;
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

            foreach (var preprocessingOperationFilter in
                _cSharpCommentOpenApiGeneratorFilterConfig.PreProcessingOperationFilters)
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
                    foreach (var operationFilter in _cSharpCommentOpenApiGeneratorFilterConfig.OperationFilters)
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
                        foreach (var configFilter in _cSharpCommentOpenApiGeneratorFilterConfig.OperationConfigFilters)
                        {
                            try
                            {
                                configFilter.Apply(
                                    operation,
                                    operationConfigElement,
                                    new OperationConfigFilterSettings
                                    {
                                        OperationFilterSettings = operationFilterSettings,
                                        OperationFilters = _cSharpCommentOpenApiGeneratorFilterConfig.OperationFilters
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
        /// <param name="annotationXml">The serialized XDocument representing annotation.</param>
        /// <param name="contractAssemblyPaths">The contract assembly paths.</param>
        /// <param name="configurationXml">The serialized XDocument representing the generation configuration.</param>
        /// <param name="openApiSpecVersion">Specification version of the OpenAPI documents to generate.</param>
        /// <param name="openApiFormat">Format (YAML or JSON) of the OpenAPI document to generate.</param>
        /// <param name="generationDiagnostic">A string representing serialized version of
        /// <see cref="GenerationDiagnostic"/>>
        /// </param>
        /// <returns>
        /// A string representing serialized version of <see cref="IDictionary{DocumentVariantInfo, OpenApiDocument}"/>>
        /// </returns>
        /// <remarks>
        /// Given that this function is expected to be called from an isolated domain,
        /// the input and output must be serializable to string or value type.
        /// </remarks>
        public IDictionary<DocumentVariantInfo, string> GenerateOpenApiDocuments(
            string annotationXml,
            IList<string> contractAssemblyPaths,
            string configurationXml,
            OpenApiSpecVersion openApiSpecVersion,
            OpenApiFormat openApiFormat,
            out GenerationDiagnostic generationDiagnostic)
        {
            IDictionary<DocumentVariantInfo, string> openApiDocuments = new Dictionary<DocumentVariantInfo, string>();

            var annotationXmlDocument = XDocument.Parse(annotationXml);
            var operationElements = annotationXmlDocument.XPathSelectElements("//doc/members/member[url and verb]")
                .ToList();

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
                        },
                        GenerationStatus = GenerationStatus.Warning
                    }
                };

                return openApiDocuments;
            }

            try
            {
                generationDiagnostic = new GenerationDiagnostic();

                var typeFetcher = new TypeFetcher(contractAssemblyPaths);

                var operationGenerationDiagnostics = GenerateSpecificationDocuments(
                    typeFetcher,
                    operationElements,
                    operationConfigElement,
                    documentVariantElementNames,
                    out var documents);

                foreach (var operationGenerationDiagnostic in operationGenerationDiagnostics)
                {
                    generationDiagnostic.OperationGenerationDiagnostics.Add(
                        new OperationGenerationDiagnostic(operationGenerationDiagnostic));
                }

                var documentGenerationDiagnostic = new DocumentGenerationDiagnostic();

                foreach (var variantInfoDocumentValuePair in documents)
                {
                    var openApiDocument = variantInfoDocumentValuePair.Value;

                    foreach (var documentFilter in _cSharpCommentOpenApiGeneratorFilterConfig.DocumentFilters)
                    {
                        try
                        {
                            documentFilter.Apply(
                                openApiDocument,
                                annotationXmlDocument,
                                new DocumentFilterSettings
                                {
                                    TypeFetcher = typeFetcher
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
                }

                if (documentConfigElement != null)
                {
                    foreach (var documentConfigFilter in
                        _cSharpCommentOpenApiGeneratorFilterConfig.DocumentConfigFilters)
                    {
                        try
                        {
                            documentConfigFilter.Apply(
                                documents,
                                documentConfigElement,
                                annotationXmlDocument,
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

                if (documentGenerationDiagnostic.Errors.Any())
                {
                    documentGenerationDiagnostic.GenerationStatus = GenerationStatus.Warning;
                    generationDiagnostic.DocumentGenerationDiagnostic = documentGenerationDiagnostic;
                }
                else
                {
                    documentGenerationDiagnostic.GenerationStatus = GenerationStatus.Success;
                    generationDiagnostic.DocumentGenerationDiagnostic = documentGenerationDiagnostic;
                }

                return documents.ToSerializedOpenApiDocuments(openApiSpecVersion, openApiFormat);
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
                            },
                            GenerationStatus = GenerationStatus.Failure
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
            IList<string> documentVariantElementNames,
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
                            GenerationStatus = GenerationStatus.Failure,
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
                            GenerationStatus = GenerationStatus.Failure,
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

                    foreach (var documentVariantElementName in documentVariantElementNames)
                    {
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

                        operationGenerationResult.GenerationStatus = GenerationStatus.Failure;
                    }
                    else
                    {
                        operationGenerationResult.GenerationStatus = GenerationStatus.Success;
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
                            GenerationStatus = GenerationStatus.Failure,
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

                foreach (var filter in _cSharpCommentOpenApiGeneratorFilterConfig.PostProcessingDocumentFilters)
                {
                    filter.Apply(
                        specificationDocuments[documentVariantInfo],
                        new PostProcessingDocumentFilterSettings()
                        {
                            OperationGenerationDiagnostics = operationGenerationResults
                        });
                }
            }

            return operationGenerationResults;
        }
    }
}