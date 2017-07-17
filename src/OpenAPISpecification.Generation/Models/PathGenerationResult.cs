// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.OpenApiSpecification.Generation.Models
{
    /// <summary>
    /// Model representing the generation result for the path.
    /// </summary>
    public class PathGenerationResult
    {
        /// <summary>
        /// The message providing details on the generation.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// The path.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// The generation status for the path.
        /// </summary>
        public GenerationStatus Status { get; }
    }
}