// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApiSpecification.Core.Models;
using Newtonsoft.Json;

namespace Microsoft.OpenApiSpecification.Generation.Models
{
    /// <summary>
    /// The class to store the open api document generation result.
    /// </summary>
    [Serializable]
    public class OpenApiDocumentGenerationResult
    {
        /// <summary>
        /// Default constructor. Required for deserialization.
        /// </summary>
        public OpenApiDocumentGenerationResult()
        {
        }

        /// <summary>
        /// Initializes the instance of <see cref="OpenApiDocumentGenerationResult"/>.
        /// </summary>
        /// <param name="generationResult">The path generation result.</param>
        public OpenApiDocumentGenerationResult(PathGenerationResult generationResult)
        {
            OpenApiSpecificationV3Document = null;
            PathGenerationResults.Add(generationResult);
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
                PathGenerationResults.Add(
                    new PathGenerationResult(pathGenerationResult.Path, pathGenerationResult.Message,
                        pathGenerationResult.Status));
            }
        }

        /// <summary>
        /// The generated open api V3 specification document.
        /// </summary>
        [JsonProperty]
        public OpenApiV3SpecificationDocument OpenApiSpecificationV3Document { get; internal set; }

        /// <summary>
        /// List of path generations results.
        /// </summary>
        [JsonProperty]
        public IList<PathGenerationResult> PathGenerationResults { get; internal set; } =
            new List<PathGenerationResult>();

        /// <summary>
        /// Returns the generation status.
        /// </summary>
        /// <returns>The generation status.</returns>
        public GenerationStatus GetGenerationStatus()
        {
            if (PathGenerationResults.Any(i => i.Status == GenerationStatus.Failure))
            {
                return GenerationStatus.Failure;
            }

            if (PathGenerationResults.Any(i => i.Status == GenerationStatus.Warning))
            {
                return GenerationStatus.Warning;
            }

            return GenerationStatus.Success;
        }
    }
}