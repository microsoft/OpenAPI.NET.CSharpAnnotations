// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Linq;
using System.Xml.Linq;
using Microsoft.OpenApi.CSharpComment.Reader.Models.KnownStrings;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpComment.Reader.PreprocessingOperationFilters
{
    /// <summary>
    /// Converts the alternative param tags (queryParam, pathParam, header) to standard param tags.
    /// </summary>
    public class ConvertAlternativeParamTagsFilter : IPreprocessingOperationFilter
    {
        /// <summary>
        /// Converts the alternative param tags (queryParam, pathParam, header) to standard param tags.
        /// </summary>
        /// <param name="paths">The paths to be updated.</param>
        /// <param name="element">The xml element representing an operation in the annotation xml.</param>
        /// <param name="settings">The operation filter settings.</param>
        public void Apply(OpenApiPaths paths, XElement element, PreprocessingOperationFilterSettings settings)
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

            foreach (var pathParamElement in pathParamElements)
            {
                pathParamElement.Name = KnownXmlStrings.Param;
                pathParamElement.Add(new XAttribute(KnownXmlStrings.In, KnownXmlStrings.Path));
            }

            foreach (var queryParamElement in queryParamElements)
            {
                queryParamElement.Name = KnownXmlStrings.Param;
                queryParamElement.Add(new XAttribute(KnownXmlStrings.In, KnownXmlStrings.Query));
            }

            foreach (var headerParamElement in headerParamElements)
            {
                headerParamElement.Name = KnownXmlStrings.Param;
                headerParamElement.Add(new XAttribute(KnownXmlStrings.In, KnownXmlStrings.Header));
            }
        }
    }
}