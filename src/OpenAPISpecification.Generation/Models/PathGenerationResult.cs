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
        /// Initializes a new instance of <see cref="PathGenerationResult"/>
        /// </summary>
        /// <param name="message">The generation message.</param>
        /// <param name="status">The generation status.</param>
        public PathGenerationResult(string message, GenerationStatus status) : this(null, message, status)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="PathGenerationResult"/>
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="message">The generation message.</param>
        /// <param name="status">The generation status.</param>
        public PathGenerationResult(string path, string message, GenerationStatus status)
        {
            Path = path;
            Message = message;
            Status = status;
        }

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