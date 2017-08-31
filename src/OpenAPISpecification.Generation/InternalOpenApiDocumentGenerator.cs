// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.OpenApiSpecification.Core.Models;
using Microsoft.OpenApiSpecification.Generation.DocumentFilters;
using Microsoft.OpenApiSpecification.Generation.Exceptions;
using Microsoft.OpenApiSpecification.Generation.Extensions;
using Microsoft.OpenApiSpecification.Generation.Models;
using Microsoft.OpenApiSpecification.Generation.OperationFilters;
using Newtonsoft.Json;

namespace Microsoft.OpenApiSpecification.Generation
{
    /// <summary>
    /// Provides functionality to parse xml into OpenApiV3Specification
    /// </summary>
    internal class InternalOpenApiDocumentGenerator : MarshalByRefObject
    {
        private static readonly IList<IDocumentFilter> DefaultDocumentFilters = new List<IDocumentFilter>
        {
            new ApplyAssemblyNameAsInfoFilter(),
            new ApplyUrlAsServerFilter()
        };

        private static readonly IList<IOperationFilter> DefaultOperationFilters = new List<IOperationFilter>
        {
            new ApplyGroupsAsTagFilter(),
            new ApplyParamAsParameterFilter(),
            new ApplyParamAsRequestBodyFilter(),
            new ApplyParamAsResponseFilter(),
            new ApplyRemarksAsDescriptionFilter(),
            new ApplySummaryFilter()
        };

        // TO DO: Figure out a way to serialize this and pass as parameter from OpenApiDocumentGenerator.
        private readonly OpenApiDocumentGeneratorConfig _generatorConfig = new OpenApiDocumentGeneratorConfig(
            DefaultOperationFilters,
            DefaultDocumentFilters);

        /// <summary>
        /// Add operation and update the operation filter settings based on the given document variant info.
        /// </summary>
        private void AddOperation(
            IDictionary<DocumentVariantInfo, OpenApiV3SpecificationDocument> specificationDocuments,
            IDictionary<DocumentVariantInfo, OperationFilterSettings> operationFilterSettingsMap,
            DocumentVariantInfo documentVariantInfo,
            XElement operationElement,
            string url,
            TypeFetcher typeFetcher)
        {
            // Create the operation and apply all the filters specified.
            var operationMethod = GetOperationMethod(operationElement);

            var operation = new Operation
            {
                OperationId = GetOperationId(url, operationMethod)
            };

            if (!operationFilterSettingsMap.ContainsKey(documentVariantInfo))
            {
                operationFilterSettingsMap[documentVariantInfo] =
                    new OperationFilterSettings
                    {
                        TypeFetcher = typeFetcher
                    };
            }

            foreach (var operationFilter in _generatorConfig.OperationFilters)
            {
                operationFilter.Apply(
                    operation,
                    operationElement,
                    operationFilterSettingsMap[documentVariantInfo]);
            }

            AnnotationXmlDocumentValidator.ValidateAllPathParametersAreDocumented(
                operation.Parameters.Where(i => i.In == ParameterKind.Path),
                url);

            // Add the processed operation to the specification document.
            if (!specificationDocuments.ContainsKey(documentVariantInfo))
            {
                specificationDocuments.Add(
                    documentVariantInfo,
                    new OpenApiV3SpecificationDocument());
            }

            var paths = specificationDocuments[documentVariantInfo].Paths;

            if (!paths.ContainsKey(url))
            {
                var pathItem = new PathItem
                {
                    [operationMethod] = operation
                };

                paths.Add(url, pathItem);
            }
            else
            {
                paths[url].Add(operationMethod, operation);
            }
        }

        /// <summary>
        /// Takes in annotation xml document and returns the OpenAPI document generation result
        /// which contains OpenAPI specification document(s).
        /// </summary>
        /// <param name="annotationXml">The annotation xml document.</param>
        /// <param name="contractAssemblyPaths">The contract assembly paths.</param>
        /// <returns>A string representing serialized version of <see cref="DocumentGenerationResult"/>></returns>
        public string GenerateOpenApiDocuments(
            string annotationXml,
            IList<string> contractAssemblyPaths)
        {
            var annotationXmlDocument = XDocument.Parse(annotationXml);

            var operationElements = annotationXmlDocument.XPathSelectElements("//doc/members/member[url and verb]")
                .ToList();

            DocumentGenerationResult result;

            if (!operationElements.Any())
            {
                result = new DocumentGenerationResult(
                    new List<PathGenerationResult>
                    {
                        new PathGenerationResult(
                            SpecificationGenerationMessages.NoOperationElementFoundToParse,
                            GenerationStatus.Success)
                    });

                return JsonConvert.SerializeObject(result);
            }

            try
            {
                result = new DocumentGenerationResult();

                // TODO: Allow custom element names to be retrieved from a config file (aka default annotations).
                // Task 13256107 contains this work.
                // This will likely be given to this method in the argument in some way.
                var documentVariantElementNames = new List<string> {"swagger"};

                var typeFetcher = new TypeFetcher(contractAssemblyPaths);

                var pathGenerationResults = GenerateSpecificationDocuments(
                    typeFetcher,
                    operationElements,
                    documentVariantElementNames,
                    result.Documents);

                foreach (var documentVariantInfo in result.Documents.Keys)
                {
                    var openApiDocument = result.Documents[documentVariantInfo];

                    foreach (var documentFilter in _generatorConfig.DocumentFilters)
                    {
                        documentFilter.Apply(openApiDocument, annotationXmlDocument);
                    }
                }

                foreach (var pathGenerationResult in pathGenerationResults)
                {
                    result.PathGenerationResults.Add(new PathGenerationResult(pathGenerationResult));
                }

                return JsonConvert.SerializeObject(result);
            }
            catch (Exception e)
            {
                result = new DocumentGenerationResult(
                    new List<PathGenerationResult>
                    {
                        new PathGenerationResult(
                            string.Format(SpecificationGenerationMessages.UnexpectedError, e),
                            GenerationStatus.Failure)
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
            IList<string> documentVariantElementNames,
            IDictionary<DocumentVariantInfo, OpenApiV3SpecificationDocument> specificationDocuments)
        {
            var pathGenerationResults = new List<PathGenerationResult>();

            var operationFilterSettingsMap = new Dictionary<DocumentVariantInfo, OperationFilterSettings>();

            foreach (var operationElement in operationElements)
            {
                string url;
                if (!TryGetUrl(pathGenerationResults, operationElement, out url))
                {
                    continue;
                }

                try
                {
                    AddOperation(
                        specificationDocuments,
                        operationFilterSettingsMap,
                        DocumentVariantInfo.Default,
                        operationElement,
                        url,
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
                                operationFilterSettingsMap,
                                documentVariantInfo,
                                operationElement,
                                url,
                                typeFetcher);
                        }
                    }

                    pathGenerationResults.Add(
                        new PathGenerationResult(
                            url,
                            SpecificationGenerationMessages.SuccessfulPathGeneration,
                            GenerationStatus.Success));
                }
                catch (Exception e)
                {
                    pathGenerationResults.Add(
                        new PathGenerationResult(url, e.Message, GenerationStatus.Failure));
                }
            }

            foreach (var documentVariantInfo in specificationDocuments.Keys)
            {
                operationFilterSettingsMap[documentVariantInfo]
                    .ReferenceRegistryManager
                    .SchemaReferenceRegistry.References.CopyInto(
                        specificationDocuments[documentVariantInfo].Components.Schemas);
            }

            return pathGenerationResults;
        }

        private string GetOperationId(string absolutePath, OperationMethod operationMethod)
        {
            if (string.IsNullOrEmpty(absolutePath))
            {
                throw new ArgumentException(nameof(absolutePath) + " must be specified");
            }

            var operationId = new StringBuilder(operationMethod.ToString().ToLowerInvariant());

            foreach (var segment in absolutePath.Split('/'))
            {
                if (string.IsNullOrEmpty(segment))
                {
                    continue;
                }

                var current = string.Empty;

                // In order to build an operation id, extract the path parameters
                // and prepend By to these before adding them to the operationId.
                // e.g. for GET /v6/products/{productId} -> getV6ProductsByProductId 
                if (segment.Contains("{"))
                {
                    var matches = new Regex(@"\{(.*?)\}").Matches(segment);

                    if (matches.Count > 0)
                    {
                        foreach (Match match in matches)
                        {
                            current += "By" + match.Groups[1].Value.ToTitleCase();
                        }
                    }
                }
                else
                {
                    current = segment.ToTitleCase();
                }

                // Open api spec recommends to follow common programming naming conventions for operation Id
                // So only allow alphabets or alphanumerics.
                operationId.Append(Regex.Replace(current, "[^a-zA-Z0-9]", string.Empty));
            }

            return operationId.ToString();
        }

        private static OperationMethod GetOperationMethod(XElement operationElement)
        {
            var verbElement = operationElement.Descendants().FirstOrDefault(i => i.Name == "verb");

            if (verbElement == null)
            {
                return OperationMethod.Undefined;
            }

            var verb = verbElement.Value.Trim();
            OperationMethod method;

            if (Enum.TryParse(verb, true, out method))
            {
                return method;
            }

            throw new DocumentationException(
                string.Format(
                    SpecificationGenerationMessages.InvalidHttpMethod,
                    verb));
        }

        private static bool TryGetUrl(
            IList<PathGenerationResult> pathGenerationResults,
            XElement operationElement,
            out string url)
        {
            var urls = operationElement.Descendants().Where(i => i.Name == "url").Select(i => i.Value);

            // Can't process further if no url is documented, so skip the operation.
            url = urls.FirstOrDefault();

            if (url == null)
            {
                return false;
            }

            try
            {
                url = HttpUtility.UrlDecode(new Uri(url).AbsolutePath);
            }
            catch (UriFormatException)
            {
                pathGenerationResults.Add(
                    new PathGenerationResult(
                        url,
                        string.Format(SpecificationGenerationMessages.InvalidUrl, url),
                        GenerationStatus.Failure));

                return false;
            }
            catch (Exception e)
            {
                pathGenerationResults.Add(
                    new PathGenerationResult(
                        url,
                        e.Message,
                        GenerationStatus.Failure));

                return false;
            }

            return true;
        }
    }
}