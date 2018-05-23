// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Linq;
using System.Xml.Linq;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models.KnownStrings;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.PreprocessingOperationFilters
{
    /// <summary>
    /// Converts the alternative param tags (queryParam, pathParam, header) to standard param tags.
    /// </summary>
    public class ConvertAlternativeParamTagsFilter : IPreProcessingOperationFilter
    {
        /// <summary>
        /// Converts the alternative param tags (queryParam, pathParam, header) to standard param tags.
        /// </summary>
        /// <param name="paths">The paths to be updated.</param>
        /// <param name="element">The xml element representing an operation in the annotation xml.</param>
        /// <param name="settings">The operation filter settings.</param>
        public void Apply(OpenApiPaths paths, XElement element, PreProcessingOperationFilterSettings settings)
        {
            var pathParamElements = element.Elements()
                .Where(p => p.Name == KnownXmlStrings.PathParam)
                .ToList();

            var queryParamElements = element.Elements()
                .Where(p => p.Name == KnownXmlStrings.QueryParam)
                .ToList();

            var headerParamElements = element.Elements()
                .Where(p => p.Name == KnownXmlStrings.Header)
                .ToList();

            var requestTypeElements = element.Elements()
                .Where(p => p.Name == KnownXmlStrings.RequestType)
                .ToList();

            var paramElements = element.Elements().Where(i => i.Name == KnownXmlStrings.Param);
            var paramElementsWithInAttributeNotSpecified = paramElements.Where(i => i.Attribute("in") == null);

            if (pathParamElements.Any())
            {
                foreach (var pathParamElement in pathParamElements)
                {
                    var conflictingPathParam = paramElements.Where(
                        i => i.Attribute("name")?.Value == pathParamElement.Attribute("name")?.Value);

                    // Remove param tags that have same name as pathParam tags
                    // e.g. if service is documented like below, it will remove the param tag
                    //
                    // <param name="samplePathParam">Sample path param</param>
                    // <pathParam name="samplePathParam" in="path">Sample path param</pathParam>
                    conflictingPathParam?.Remove();

                    var nameAttribute = pathParamElement.Attribute(KnownXmlStrings.Name);
                    var name = nameAttribute?.Value.Trim();

                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        nameAttribute.Value = name;
                    }

                    pathParamElement.Name = KnownXmlStrings.Param;
                    pathParamElement.Add(new XAttribute(KnownXmlStrings.In, KnownXmlStrings.Path));
                }
            }

            if (queryParamElements.Any())
            {
                foreach (var queryParamElement in queryParamElements)
                {
                    var conflictingQueryParam = paramElements.Where(
                        i => i.Attribute("name")?.Value == queryParamElement.Attribute("name")?.Value);

                    // Remove param tags that have same name as queryParam tags
                    // e.g. if service is documented like below, it will remove the param tag
                    //
                    // <param name="sampleQueryParam">Sample query param</param>
                    // <queryParam name="sampleQueryParam" in="path">Sample query param</queryParam>
                    conflictingQueryParam?.Remove();

                    var nameAttribute = queryParamElement.Attribute(KnownXmlStrings.Name);
                    var name = nameAttribute?.Value.Trim();

                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        nameAttribute.Value = name;
                    }

                    queryParamElement.Name = KnownXmlStrings.Param;
                    queryParamElement.Add(new XAttribute(KnownXmlStrings.In, KnownXmlStrings.Query));
                }
            }

            if (requestTypeElements.Any())
            {
                var paramTagToRemove = element.Elements()
                    .Where(i => i.Name == KnownXmlStrings.Param
                        && string.IsNullOrWhiteSpace(i.Attribute("in")?.Value));

                // If there are still conflicting param tags remaining, then it's safe to assume that these are neither
                // path nor query params and could be documented request params which is not intended to be used with
                // C# document generator so remove the tags.
                paramTagToRemove?.Remove();

                foreach (var requestTypeElement in requestTypeElements)
                {
                    requestTypeElement.Name = KnownXmlStrings.Param;
                    requestTypeElement.Add(new XAttribute(KnownXmlStrings.In, KnownXmlStrings.Body));
                }
            }

            foreach (var headerParamElement in headerParamElements)
            {
                var nameAttribute = headerParamElement.Attribute(KnownXmlStrings.Name);
                var name = nameAttribute?.Value.Trim();

                if (!string.IsNullOrWhiteSpace(name))
                {
                    nameAttribute.Value = name;
                }

                headerParamElement.Name = KnownXmlStrings.Param;
                headerParamElement.Add(new XAttribute(KnownXmlStrings.In, KnownXmlStrings.Header));
            }

            // If any of the alternative tags are present then remove any param tag element that have "in" attribute
            // not specified b/c assumption is made that those params are not supposed to be processed by
            // CSharp Annotation Document Generator.
            if (pathParamElements.Any()
                || queryParamElements.Any()
                || requestTypeElements.Any()
                || headerParamElements.Any())
            {
                paramElementsWithInAttributeNotSpecified?.Remove();
            }
        }
    }
}