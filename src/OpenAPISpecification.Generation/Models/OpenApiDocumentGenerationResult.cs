// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApiSpecification.Core.Models;

namespace Microsoft.OpenApiSpecification.Generation.Models
{
    /// <summary>
    /// The class to store the open api document generation result.
    /// </summary>
    public class OpenApiDocumentGenerationResult
    {
        /// <summary>
        /// The generation status.
        /// </summary>
        public GenerationStatus GenerationStatus { get; }

        /// <summary>
        /// The generated open api V3 specification document.
        /// </summary>
        public OpenApiV3SpecificationDocument OpenApiSpecificationV3Document { get; }

        /// <summary>
        /// List of path generations results.
        /// </summary>
        public IReadOnlyCollection<PathGenerationResult> PathGenerationResults { get; }
    }
}