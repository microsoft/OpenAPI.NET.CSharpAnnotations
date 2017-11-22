// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.OpenApi.CSharpComment.Reader.Models.KnownStrings;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpComment.Reader.OperationConfigFilters
{
    /// <summary>
    /// Parses the value of the annotation nodes in operation-level config and applies them to the specified operations.
    /// </summary>
    public class CommonAnnotationFilter : IOperationConfigFilter
    {
        /// <summary>
        /// Fetches the annotations specified in the operation-level config and applies them to the specified operations.
        /// </summary>
        /// <param name="operation">The operation to be updated.</param>
        /// <param name="element">The xml element containing operation-level config in the config xml.</param>
        /// <param name="settings">The operation config filter settings.</param>
        public void Apply(OpenApiOperation operation, XElement element, OperationConfigFilterSettings settings)
        {
            var annotationElements = element.Descendants().Where(i => i.Name == KnownXmlStrings.Annotation);

            foreach (var annotationElement in annotationElements)
            {
                var targetedTag = annotationElement.Attribute(KnownXmlStrings.Tag);

                if (targetedTag == null ||
                    !string.Equals(targetedTag.Value.Trim(), "$default", StringComparison.InvariantCultureIgnoreCase) &&
                    !operation.Tags.Select(t => t.Name)
                        .Contains(targetedTag.Value.Trim(), StringComparer.InvariantCultureIgnoreCase))
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