// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
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
        /// <param name="generationStatus">The generation status.</param>
        public PathGenerationResult(string message, GenerationStatus generationStatus) : this(
            null,
            message,
            generationStatus)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="PathGenerationResult"/>.
        /// </summary>
        /// <param name="pathGenerationResult">The path generation result to copy from.</param>
        public PathGenerationResult(PathGenerationResult pathGenerationResult) :
            this(pathGenerationResult.Path, pathGenerationResult.Message, pathGenerationResult.GenerationStatus)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="PathGenerationResult"/>.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="message">The generation message.</param>
        /// <param name="generationStatus">The generation status.</param>
        public PathGenerationResult(string path, string message, GenerationStatus generationStatus)
        {
            Path = path;
            Message = message;
            GenerationStatus = generationStatus;
        }

        /// <summary>
        /// The generation status for the path.
        /// </summary>
        [JsonProperty]
        public GenerationStatus GenerationStatus { get; internal set; }

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
    }
}