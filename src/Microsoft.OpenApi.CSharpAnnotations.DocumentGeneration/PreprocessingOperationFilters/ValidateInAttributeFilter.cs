// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Linq;
using System.Xml.Linq;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Exceptions;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models.KnownStrings;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.PreprocessingOperationFilters
{
    /// <summary>
    /// Validates the in attribute in param tag.
    /// </summary>
    public class ValidateInAttributeFilter : IPreProcessingOperationFilter
    {
        /// <summary>
        /// Validates the "in" attribute in param tagsif all parameter tags.
        /// </summary>
        /// <param name="paths">The paths to be validated.</param>
        /// <param name="element">The xml element representing an operation in the annotation xml.</param>
        /// <param name="settings">The operation filter settings.</param>
        public void Apply(OpenApiPaths paths, XElement element, PreProcessingOperationFilterSettings settings)
        {
            var paramElements = element.Elements()
                .Where(p => p.Name == KnownXmlStrings.Param)
                .ToList();

            var paramElementsWithoutIn = paramElements.Where(p => p.Attribute(KnownXmlStrings.In)?.Value == null)
                .ToList();

            var paramElementsWithoutAllowedValues = paramElements.Where(
                p => !KnownXmlStrings.AllowedInValues.Contains(p.Attribute(KnownXmlStrings.In)?.Value)).ToList();

            if (paramElementsWithoutIn.Any())
            {
                throw new MissingInAttributeException(
                    paramElementsWithoutIn.Select(
                        p => p.Attribute(KnownXmlStrings.Name)?.Value));
            }

            if (paramElementsWithoutAllowedValues.Any())
            {
                throw new NotSupportedInAttributeValueException(
                    paramElementsWithoutAllowedValues.Select(
                        p => p.Attribute(KnownXmlStrings.Name)?.Value),
                    paramElementsWithoutAllowedValues.Select(
                        p => p.Attribute(KnownXmlStrings.In)?.Value));
            }
        }
    }
}