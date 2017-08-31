// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.OpenApiSpecification.Core.Models;
using Microsoft.OpenApiSpecification.Generation.Exceptions;

namespace Microsoft.OpenApiSpecification.Generation
{
    /// <summary>
    /// Holds functionality to validate if annotation xml is documented correctly.
    /// </summary>
    public static class AnnotationXmlDocumentValidator
    {
        /// <summary>
        /// Validates if all the path parameters are documented in the annotation xml.
        /// </summary>
        /// <param name="pathParameters">The list of path parameters documented in annotation xml.</param>
        /// <param name="absolutePath">The absolute path to compare against.</param>
        public static void ValidateAllPathParametersAreDocumented(
            IEnumerable<Parameter> pathParameters,
            string absolutePath)
        {
            // Regex remove brackets from {pathParamNames} in segment
            //    Examples: {pathSegment} -> [pathSegment]
            //              productId:{productId}-skuId:{skuId} -> [productId, skuId]
            var matches = new Regex(@"\{(.*?)\}").Matches(absolutePath);

            foreach (Match match in matches)
            {
                var pathParamNameFromAbsolutePath = match.Groups[1].Value;

                // Throw an exception if it is not found as this is a mistake in the documentation.
                if (!pathParameters.Any(p => p.Name == pathParamNameFromAbsolutePath))
                {
                    throw new UndocumentedPathParameterException(pathParamNameFromAbsolutePath, absolutePath);
                }
            }
        }
    }
}