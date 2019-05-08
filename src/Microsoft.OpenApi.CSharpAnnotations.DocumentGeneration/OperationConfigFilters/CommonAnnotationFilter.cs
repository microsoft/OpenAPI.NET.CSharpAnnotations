// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models.KnownStrings;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.OperationConfigFilters
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
        /// <returns>The list of generation errors, if any produced when processing the filter."></returns>
        public IList<GenerationError> Apply(
            OpenApiOperation operation,
            XElement element,
            OperationConfigFilterSettings settings)
        {
            var generationErrors = new List<GenerationError>();

            try
            {
                var annotationElements = element.Descendants().Where(i => i.Name == KnownXmlStrings.Annotation);

                foreach (var annotationElement in annotationElements)
                {
                    var targetedTag = annotationElement.Attribute(KnownXmlStrings.Tag);

                    // Only proceed if the target tag is null (indicating that the annotation applies to all operations)
                    // or if the target tag matches with tags in this operation.
                    if (targetedTag != null &&
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
            catch(Exception ex)
            {
                generationErrors.Add(
                   new GenerationError
                   {
                       Message = ex.Message,
                       ExceptionType = ex.GetType().Name
                   });
            }

            return generationErrors;
        }
    }
}