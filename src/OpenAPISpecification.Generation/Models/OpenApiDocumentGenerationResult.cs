// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApiSpecification.Core.Models;

namespace Microsoft.OpenApiSpecification.Generation.Models
{
    /// <summary>
    /// The class to store the open api document generation result.
    /// </summary>
    public class OpenApiDocumentGenerationResult
    {
        private List<PathGenerationResult> _pathGenerationResults = new List<PathGenerationResult>();

        /// <summary>
        /// Initializes the instance of <see cref="OpenApiDocumentGenerationResult"/>.
        /// </summary>
        /// <param name="generationResult">The path generation result.</param>
        public OpenApiDocumentGenerationResult(PathGenerationResult generationResult)
        {
            OpenApiSpecificationV3Document = null;
            _pathGenerationResults.Add(generationResult);
        }

        /// <summary>
        /// Initializes the instance of <see cref="OpenApiDocumentGenerationResult"/>.
        /// </summary>
        /// <param name="document">The generated open api specification document.</param>
        /// <param name="generationResults">The path generation results.</param>
        public OpenApiDocumentGenerationResult(OpenApiV3SpecificationDocument document,
            IEnumerable<PathGenerationResult> generationResults)
        {
            OpenApiSpecificationV3Document = document;

            foreach (var pathGenerationResult in generationResults)
            {
                _pathGenerationResults.Add(
                    new PathGenerationResult(pathGenerationResult.Path, pathGenerationResult.Message,
                        pathGenerationResult.Status));
            }
        }

        /// <summary>
        /// The generation status.
        /// </summary>
        public GenerationStatus GenerationStatus
        {
            get
            {
                if (_pathGenerationResults.Any(i => i.Status == GenerationStatus.Failure))
                {
                    return GenerationStatus.Failure;
                }

                if (_pathGenerationResults.Any(i => i.Status == GenerationStatus.Warning))
                {
                    return GenerationStatus.Warning;
                }

                return GenerationStatus.Success;
            }
        }

        /// <summary>
        /// The generated open api V3 specification document.
        /// </summary>
        public OpenApiV3SpecificationDocument OpenApiSpecificationV3Document { get; }

        /// <summary>
        /// List of path generations results.
        /// </summary>
        public IReadOnlyCollection<PathGenerationResult> PathGenerationResults => _pathGenerationResults;
    }
}