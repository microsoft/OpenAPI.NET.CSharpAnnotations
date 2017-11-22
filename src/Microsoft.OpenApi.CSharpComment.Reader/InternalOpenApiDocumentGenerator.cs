// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

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
using Microsoft.OpenApi.CSharpComment.Reader.PreprocessingOperationFilters;
using Microsoft.OpenApi.CSharpComment.Reader.ReferenceRegistries;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Microsoft.OpenApi.CSharpComment.Reader
{
    /// <summary>
    /// Provides functionality to parse xml into OpenApiV3Specification
    /// </summary>
    internal class InternalOpenApiDocumentGenerator : MarshalByRefObject
    {
        public DocumentGenerationResultWithDocumentAsString Result
        {
            get;
            private set;
        }

        private static readonly IList<IDocumentConfigFilter> _defaultDocumentConfigFilters =
            new List<IDocumentConfigFilter>
            {
                new DocumentVariantAttributesFilter()
            };

        private static readonly IList<IDocumentFilter> _defaultDocumentFilters = new List<IDocumentFilter>
        {
            new AddSpecVersionFilter(),
            new AssemblyNameToInfoFilter(),
            new UrlToServerFilter(),
            new MemberSummaryToSchemaDescriptionFilter()
        };

        private static readonly IList<IOperationConfigFilter> _defaultOperationConfigFilters =
            new List<IOperationConfigFilter>
            {
                new CommonAnnotationFilter()
            };

        private static readonly IList<IOperationFilter> _defaultOperationFilters = new List<IOperationFilter>
        {
            new GroupToTagFilter(),
            new ParamToParameterFilter(),
            new ParamToRequestBodyFilter(),
            new ResponseToResponseFilter(),
            new RemarksToDescriptionFilter(),
            new SummaryToSummaryFilter()
        };

        private static readonly IList<IPreprocessingOperationFilter> _defaultPreprocessingOperationFilters =
            new List<IPreprocessingOperationFilter>
            {
                new ConvertAlternativeParamTagsFilter(),
                new PopulateInAttributeFilter(),
                new BranchOptionalPathParametersFilter()
            };

        private readonly OpenApiDocumentGeneratorConfig _generatorConfig = new OpenApiDocumentGeneratorConfig
        {
            DocumentConfigFilters = _defaultDocumentConfigFilters,
            DocumentFilters = _defaultDocumentFilters,
            OperationConfigFilters = _defaultOperationConfigFilters,
            OperationFilters = _defaultOperationFilters,
            PreprocessingOperationFilters = _defaultPreprocessingOperationFilters
        };

        /// <summary>
        /// Add operation and update the operation filter settings based on the given document variant info.
        /// </summary>
        private void AddOperation(
            IDictionary<DocumentVariantInfo, OpenApiDocument> specificationDocuments,
            IDictionary<DocumentVariantInfo, ReferenceRegistryManager> referenceRegistryManagerMap,
            DocumentVariantInfo documentVariantInfo,
            XElement operationElement,
            XElement operationConfigElement,
            TypeFetcher typeFetcher)
        {
            var paths = new OpenApiPaths();

            foreach (var preprocessingOperationFilter in _generatorConfig.PreprocessingOperationFilters)
            {
                preprocessingOperationFilter.Apply(
                    paths,
                    operationElement,
                    new PreprocessingOperationFilterSettings());
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
                        OperationMethod = operationMethod.ToString(),
                    };

                    // Apply all the operation-related filters to extract information related to the operation.
                    // It is important that these are applied before the config filters below
                    // since the config filters may rely on information generated from operation filters.
                    foreach (var operationFilter in _generatorConfig.OperationFilters)
                    {
                        operationFilter.Apply(
                            operation,
                            operationElement,
                            operationFilterSettings);
                    }

                    if (operationConfigElement != null)
                    {
                        // Apply the config-related filters to extract information from the config xml
                        // that can be applied to the operations.
                        foreach (var configFilter in _generatorConfig.OperationConfigFilters)
                        {
                            configFilter.Apply(
                                operation,
                                operationConfigElement,
                                new OperationConfigFilterSettings
                                {
                                    OperationFilterSettings = operationFilterSettings,
                                    OperationFilters = _generatorConfig.OperationFilters
                                });
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
        /// <returns>A string representing serialized version of 
        /// <see cref="DocumentGenerationResultWithDocumentAsString"/>></returns>
        /// <remarks>
        /// Given that this function is expected to be called from an isolated domain,
        /// the input and output must be serialized to string.
        /// </remarks>
        public string GenerateOpenApiDocuments(
            string annotationXml,
            IList<string> contractAssemblyPaths,
            string configurationXml)
        {
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

            DocumentGenerationResultWithDocumentAsString result;

            if (!operationElements.Any())
            {
                result = new DocumentGenerationResultWithDocumentAsString(
                    new List<PathGenerationResult>
                    {
                        new PathGenerationResult
                        {
                            Message = SpecificationGenerationMessages.NoOperationElementFoundToParse,
                            GenerationStatus = GenerationStatus.Success
                        }
                    });

                return JsonConvert.SerializeObject(result);
            }

            try
            {
                result = new DocumentGenerationResultWithDocumentAsString();

                var typeFetcher = new TypeFetcher(contractAssemblyPaths);
                
                var pathGenerationResults = GenerateSpecificationDocuments(
                    typeFetcher,
                    operationElements,
                    operationConfigElement,
                    documentVariantElementNames, 
                    out var documents);

                foreach (var pathGenerationResult in pathGenerationResults)
                {
                    result.PathGenerationResults.Add(new PathGenerationResult(pathGenerationResult));
                }

                try
                {
                    foreach (var variantInfoDocumentValuePair in documents)
                    {
                        var openApiDocument = variantInfoDocumentValuePair.Value;

                        foreach (var documentFilter in _generatorConfig.DocumentFilters)
                        {
                            documentFilter.Apply(
                                openApiDocument,
                                annotationXmlDocument,
                                new DocumentFilterSettings
                                {
                                    TypeFetcher = typeFetcher
                                });
                        }
                    }

                    if (documentConfigElement != null)
                    {
                        foreach (var documentConfigFilter in _generatorConfig.DocumentConfigFilters)
                        {
                            documentConfigFilter.Apply(
                                documents,
                                documentConfigElement,
                                annotationXmlDocument,
                                new DocumentConfigFilterSettings());
                        }
                    }
                }
                catch (Exception e)
                {
                    // Document and document config filters yield an exception.
                    // This exception may not be tied to a particular operation and the resulting
                    // documents may be in bad state. We simply return empty document with
                    // an exception message in the path generation result.
                    result = new DocumentGenerationResultWithDocumentAsString(
                        new List<PathGenerationResult>
                        {
                            new PathGenerationResult
                            {
                                ExceptionType = e.GetType(),
                                Message = e.Message,
                                GenerationStatus = GenerationStatus.Failure
                            }
                        });
                }
                
                foreach (var variantInfoDocumentPair in documents)
                {
                    result.Documents[variantInfoDocumentPair.Key] = 
                        variantInfoDocumentPair.Value.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0_0);
                }

                return JsonConvert.SerializeObject(result);
            }
            catch (Exception e)
            {
                result = new DocumentGenerationResultWithDocumentAsString(
                    new List<PathGenerationResult>
                    {
                        new PathGenerationResult
                        {
                            ExceptionType = e.GetType(),
                            Message = string.Format(SpecificationGenerationMessages.UnexpectedError, e),
                            GenerationStatus = GenerationStatus.Failure
                        }
                    });

                return JsonConvert.SerializeObject(result);
            }
        }

        /// <summary>
        /// Populate the specification documents for all document variant infos.
        /// </summary>
        /// <returns>The path generation results from populating the specification documents.</returns>
        private IList<PathGenerationResult> GenerateSpecificationDocuments(
            TypeFetcher typeFetcher,
            IList<XElement> operationElements,
            XElement operationConfigElement,
            IList<string> documentVariantElementNames,
            out IDictionary<DocumentVariantInfo, OpenApiDocument> specificationDocuments)
        {
            specificationDocuments = new Dictionary<DocumentVariantInfo, OpenApiDocument>();

            var pathGenerationResults = new List<PathGenerationResult>();

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
                    pathGenerationResults.Add(
                        new PathGenerationResult
                        {
                            OperationMethod = SpecificationGenerationMessages.OperationMethodNotParsedGivenUrlIsInvalid,
                            Path = e.Url,
                            ExceptionType = e.GetType(),
                            Message = e.Message,
                            GenerationStatus = GenerationStatus.Failure
                        });

                    continue;
                }

                try
                {
                    operationMethod = OperationHandler.GetOperationMethod(url, operationElement);
                }
                catch (InvalidVerbException e)
                {
                    pathGenerationResults.Add(
                        new PathGenerationResult
                        {
                            OperationMethod = e.Verb,
                            Path = url,
                            ExceptionType = e.GetType(),
                            Message = e.Message,
                            GenerationStatus = GenerationStatus.Failure
                        });

                    continue;
                }

                try
                {
                    AddOperation(
                        specificationDocuments,
                        referenceRegistryManagerMap,
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
                                documentVariantInfo,
                                operationElement,
                                operationConfigElement,
                                typeFetcher);
                        }
                    }

                    pathGenerationResults.Add(
                        new PathGenerationResult
                        {
                            OperationMethod = operationMethod.ToString(),
                            Path = url,
                            Message = SpecificationGenerationMessages.SuccessfulPathGeneration,
                            GenerationStatus = GenerationStatus.Success
                        });
                }
                catch (Exception e)
                {
                    pathGenerationResults.Add(
                        new PathGenerationResult
                        {
                            OperationMethod = operationMethod.ToString(),
                            Path = url,
                            ExceptionType = e.GetType(),
                            Message = e.Message,
                            GenerationStatus = GenerationStatus.Failure
                        });
                }
            }

            foreach (var documentVariantInfo in specificationDocuments.Keys)
            {
                referenceRegistryManagerMap[documentVariantInfo]
                    .SchemaReferenceRegistry.References.CopyInto(
                        specificationDocuments[documentVariantInfo].Components.Schemas);
            }

            return pathGenerationResults;
        }
    }
}