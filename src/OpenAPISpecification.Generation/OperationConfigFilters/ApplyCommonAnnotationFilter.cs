// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.OpenApiSpecification.Core.Models;
using Microsoft.OpenApiSpecification.Generation.Models.KnownStrings;

namespace Microsoft.OpenApiSpecification.Generation.ConfigFilters
{
    /// <summary>
    /// Parses the value of the annotation nodes in operation-level config and applies them to the specified operations.
    /// </summary>
    public class ApplyCommonAnnotationFilter : IOperationConfigFilter
    {
        /// <summary>
        /// Fetches the annotations specified in the operation-level config and applies them to the specified operations.
        /// </summary>
        /// <param name="operation">The operation to be updated.</param>
        /// <param name="element">The xml element containing operation-level config in the config xml.</param>
        /// <param name="settings">The operation config filter settings.</param>
        public void Apply(Operation operation, XElement element, OperationConfigFilterSettings settings)
        {
            var annotationElements = element.Descendants().Where(i => i.Name == KnownXmlStrings.Annotation);

            foreach (var annotationElement in annotationElements)
            {
                var targetedTag = annotationElement.Attribute(KnownXmlStrings.Tag);

                if (targetedTag == null ||
                    !string.Equals(targetedTag.Value.Trim(), "$default", StringComparison.InvariantCultureIgnoreCase) &&
                    !operation.Tags.Contains(targetedTag.Value.Trim(), StringComparer.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                foreach (var operationFilter in settings.OperationFilters)
                {
                    operationFilter.Apply(
                        operation,
                        annotationElement,
                        settings.OperationFilterSettings);
                }
            }
        }
    }
}