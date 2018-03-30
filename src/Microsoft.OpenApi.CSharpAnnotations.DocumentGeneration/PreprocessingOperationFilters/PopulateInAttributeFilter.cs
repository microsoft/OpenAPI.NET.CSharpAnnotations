// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Exceptions;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Extensions;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models.KnownStrings;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.PreprocessingOperationFilters
{
    /// <summary>
    /// Parses the URL to populate the in attribute of the param tags if not explicitly documented.
    /// </summary>
    public class PopulateInAttributeFilter : IPreProcessingOperationFilter
    {
        /// <summary>
        /// Parses the parameters in the URL to populate the in attribute of the param tags as path or query
        /// if not explicitly documented.
        /// </summary>
        /// <param name="paths">The paths to be updated.</param>
        /// <param name="element">The xml element representing an operation in the annotation xml.</param>
        /// <param name="settings">The operation filter settings.</param>
        public void Apply(OpenApiPaths paths, XElement element, PreProcessingOperationFilterSettings settings)
        {
            var paramElements = element.Elements()
                .Where(p => p.Name == KnownXmlStrings.Param)
                .ToList();

            var paramElementsWithoutIn = paramElements.Where(
                    p => !KnownXmlStrings.AllowedInValues.Contains(p.Attribute(KnownXmlStrings.In)?.Value))
                .ToList();
            
            var url = element.Elements()
                .FirstOrDefault(p => p.Name == KnownXmlStrings.Url)
                ?.Value;

            if (!string.IsNullOrWhiteSpace(url))
            {
                foreach (var paramElement in paramElementsWithoutIn)
                {
                    var paramName = paramElement.Attribute(KnownXmlStrings.Name)?.Value;

                    if (url.Contains(
                            $"/{{{paramName}}}",
                            StringComparison.InvariantCultureIgnoreCase) &&
                        url.Contains(
                            $"={{{paramName}}}",
                            StringComparison.InvariantCultureIgnoreCase))
                    {
                        // The parameter is in both path and query. We cannot determine what to put for "in" attribute.
                        throw new ConflictingPathAndQueryParametersException(paramName, url);
                    }

                    if (url.Contains(
                        $"/{{{paramName}}}",
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        paramElement.Add(new XAttribute(KnownXmlStrings.In, KnownXmlStrings.Path));
                    }
                    else if (url.Contains(
                        $"={{{paramName}}}",
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        paramElement.Add(new XAttribute(KnownXmlStrings.In, KnownXmlStrings.Query));
                    }
                }

                var pathParamElements = paramElements
                    .Where(p => p.Attribute(KnownXmlStrings.In)?.Value == KnownXmlStrings.Path)
                    .ToList();

                var matches = new Regex(@"\{(.*?)\}").Matches(url.Split('?')[0]);

                foreach (Match match in matches)
                {
                    var pathParamNameFromUrl = match.Groups[1].Value;

                    // All path params in the URL must be documented.
                    if (!pathParamElements.Any(p => p.Attribute(KnownXmlStrings.Name)?.Value == pathParamNameFromUrl))
                    {
                        throw new UndocumentedPathParameterException(pathParamNameFromUrl, url);
                    }
                }
            }

            paramElementsWithoutIn = paramElements.Where(
                    p => !KnownXmlStrings.AllowedInValues.Contains(p.Attribute(KnownXmlStrings.In)?.Value))
                .ToList();
            
            if (paramElementsWithoutIn.Any())
            {
                throw new MissingInAttributeException(
                    paramElementsWithoutIn.Select(
                            p => p.Attribute(KnownXmlStrings.Name)?.Value)
                        .ToList());
            }
        }
    }
}