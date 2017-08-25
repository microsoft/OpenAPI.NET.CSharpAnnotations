// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Newtonsoft.Json;

namespace Microsoft.OpenApiSpecification.Generation.Models
{
    /// <summary>
    /// Model representing the generation result for the path.
    /// </summary>
    public class PathGenerationResult
    {
        /// <summary>
        /// Default constructor. Required for deserialization.
        /// </summary>
        public PathGenerationResult()
        {
        }

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
        [JsonProperty]
        public string Message { get; internal set; }

        /// <summary>
        /// The path.
        /// </summary>
        [JsonProperty]
        public string Path { get; internal set; }

        /// <summary>
        /// The generation status for the path.
        /// </summary>
        [JsonProperty]
        public GenerationStatus Status { get; internal set; }
    }
}