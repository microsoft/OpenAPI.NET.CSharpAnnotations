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
        private readonly OpenApiDocumentGeneratorSettings _generatorSettings = new OpenApiDocumentGeneratorSettings(
            DefaultOperationFilters, DefaultDocumentFilters);

        /// <summary>
        /// Takes in annotation xml document and returns the open api document generation result which contains the
        /// corresponding open api document.
        /// </summary>
        /// <param name="annotationXml">The annotation xml document.</param>
        /// <param name="contractAssemblyPaths">The contract assembly paths.</param>
        /// <returns>See <see cref="OpenApiDocumentGenerationResult"/>></returns>
        public string GenerateOpenApiDocument(
            string annotationXml,
            IEnumerable<string> contractAssemblyPaths)
        {
            var annotationXmlDocument = XDocument.Parse(annotationXml);

            var operationElements = annotationXmlDocument.XPathSelectElements("//doc/members/member[url and verb]");

            var operationFilterSettings =
                new OperationFilterSettings
                {
                    TypeFetcher = new TypeFetcher(contractAssemblyPaths)
                };

            OpenApiDocumentGenerationResult result;

            if (!operationElements.Any())
            {
                result = new OpenApiDocumentGenerationResult(
                    new PathGenerationResult(SpecificationGenerationMessages.NoOperationElementFoundToParse,
                        GenerationStatus.Success));

                return JsonConvert.SerializeObject(result);
            }

            try
            {
                var generatePathResult = GeneratePaths(operationElements, operationFilterSettings);
                var openApiDocument = new OpenApiV3SpecificationDocument();

                generatePathResult.Item2.CopyInto(openApiDocument.Paths);

                foreach (var documentFilter in _generatorSettings.DocumentFilters)
                {
                    documentFilter.Apply(openApiDocument, annotationXmlDocument);
                }

                operationFilterSettings.ReferenceRegistryManager
                    .SchemaReferenceRegistry.References.CopyInto(openApiDocument.Components.Schemas);

                result = new OpenApiDocumentGenerationResult(
                    openApiDocument,
                    generatePathResult.Item1);
            }
            catch (Exception e)
            {
                result = new OpenApiDocumentGenerationResult(new PathGenerationResult(
                    string.Format(SpecificationGenerationMessages.UnexpectedError, e),
                    GenerationStatus.Failure));
            }

            return JsonConvert.SerializeObject(result);
        }

        private string GenerateOperationId(string absolutePath, OperationMethod operationMethod)
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
                            current += "By" + ToTitleCase(match.Groups[1].Value);
                        }
                    }
                }
                else
                {
                    current = ToTitleCase(segment);
                }

                // Open api spec recommends to follow common programming naming conventions for operation Id
                // So only allow alphabets or alphanumerics.
                operationId.Append(Regex.Replace(current, "[^a-zA-Z0-9]", string.Empty));
            }

            return operationId.ToString();
        }

        private Tuple<List<PathGenerationResult>, IDictionary<string, Operations>> GeneratePaths(
            IEnumerable<XElement> operationElements,
            OperationFilterSettings operationFilterSettings)
        {
            var pathGenerationResults = new List<PathGenerationResult>();
            var paths = new Dictionary<string, Operations>();

            foreach (var operationElement in operationElements)
            {
                var urls = operationElement.Descendants().Where(i => i.Name == "url").Select(i => i.Value);

                // Can't process further if no url is documented, so skip the operation.
                var pathId = urls.FirstOrDefault();

                if (pathId == null)
                {
                    continue;
                }

                try
                {
                    pathId = HttpUtility.UrlDecode(new Uri(pathId).AbsolutePath);

                    var operationMethod = GetOperationMethod(operationElement);

                    var operation = new Operation
                    {
                        OperationId = GenerateOperationId(pathId, operationMethod)
                    };

                    foreach (var operationFilter in _generatorSettings.OperationFilters)
                    {
                        operationFilter.Apply(operation, operationElement, operationFilterSettings);
                    }

                    AnnotationXmlDocumentValidator.ValidateAllPathParametersAreDocumented(
                        operation.Parameters.Where(i => i.In == ParameterKind.Path), pathId);

                    if (!paths.ContainsKey(pathId))
                    {
                        var operations = new Operations();
                        operations.Add(operationMethod, operation);

                        paths.Add(pathId, operations);
                    }
                    else
                    {
                        paths[pathId].Add(operationMethod, operation);
                    }

                    pathGenerationResults.Add(new PathGenerationResult(pathId,
                        SpecificationGenerationMessages.SuccessfulPathGeneration, GenerationStatus.Success));
                }
                catch (UriFormatException)
                {
                    pathGenerationResults.Add(new PathGenerationResult(
                        pathId,
                        string.Format(SpecificationGenerationMessages.InvalidUrl, pathId),
                        GenerationStatus.Failure));
                }
                catch (Exception e)
                {
                    pathGenerationResults.Add(
                        new PathGenerationResult(pathId, e.Message, GenerationStatus.Failure));
                }
            }

            return new Tuple<List<PathGenerationResult>, IDictionary<string, Operations>>(
                pathGenerationResults, paths);
        }

        private static OperationMethod GetOperationMethod(XElement element)
        {
            var verbElement = element.Descendants().FirstOrDefault(i => i.Name == "verb");

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

            throw new DocumentationException(string.Format(
                SpecificationGenerationMessages.InvalidHttpMethod, verb));
        }

        private static string ToTitleCase(string target)
        {
            if (string.IsNullOrWhiteSpace(target))
            {
                return target;
            }

            return target.Substring(0, 1).ToUpperInvariant() + target.Substring(1);
        }
    }
}