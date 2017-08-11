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
using Microsoft.OpenApiSpecification.Generation.Exceptions;
using Microsoft.OpenApiSpecification.Generation.Extensions;
using Microsoft.OpenApiSpecification.Generation.Models;

namespace Microsoft.OpenApiSpecification.Generation
{
    /// <summary>
    /// Provides functionality to parse xml into OpenApiV3Specification
    /// </summary>
    internal class InternalOpenApiDocumentGenerator
    {
        private readonly OpenApiDocumentGeneratorSettings _generatorSettings;

        /// <summary>
        /// Creates a new instance of <see cref="InternalOpenApiDocumentGenerator"/>.
        /// </summary>
        /// <param name="generatorSettings">The xml tag processor.</param>
        public InternalOpenApiDocumentGenerator(OpenApiDocumentGeneratorSettings generatorSettings)
        {
            _generatorSettings = generatorSettings;
        }

        /// <summary>
        /// Takes in annotation xml document and returns the open api document generation result which contains the
        /// corresponding open api document.
        /// </summary>
        /// <param name="annotationXml">The annotation xml document.</param>
        /// <returns>See <see cref="OpenApiDocumentGenerationResult"/>></returns>
        public OpenApiDocumentGenerationResult GenerateOpenApiDocument(XDocument annotationXml)
        {
            var operationElements = annotationXml.XPathSelectElements("//doc/members/member[url and verb]");

            if (!operationElements.Any())
            {
                return new OpenApiDocumentGenerationResult(
                    new PathGenerationResult(SpecificationGenerationMessages.NoOperationElementFoundToParse,
                        GenerationStatus.Success));
            }

            try
            {
                var generatePathResult = GeneratePaths(operationElements);
                var openApiDocument = new OpenApiV3SpecificationDocument();

                generatePathResult.Item2.CopyInto(openApiDocument.Paths);

                foreach (var documentFilter in _generatorSettings.DocumentFilters)
                {
                    documentFilter.Apply(openApiDocument, annotationXml);
                }

                return new OpenApiDocumentGenerationResult(
                    openApiDocument,
                    generatePathResult.Item1);
            }
            catch (Exception e)
            {
                return new OpenApiDocumentGenerationResult(new PathGenerationResult(
                    string.Format(SpecificationGenerationMessages.UnexpectedError, e),
                    GenerationStatus.Failure));
            }
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
            IEnumerable<XElement> operationElements)
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
                        operationFilter.Apply(operation, operationElement);
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