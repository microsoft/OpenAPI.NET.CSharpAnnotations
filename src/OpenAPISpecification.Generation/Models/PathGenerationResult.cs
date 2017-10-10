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
    public class PathGenerationResult : IEquatable<PathGenerationResult>
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
            null,
            message,
            generationStatus)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="PathGenerationResult"/>.
        /// </summary>
        /// <param name="pathGenerationResult">The path generation result to copy from.</param>
        public PathGenerationResult(PathGenerationResult pathGenerationResult) : this(
            pathGenerationResult.OperationMethod,
            pathGenerationResult.Path,
            pathGenerationResult.Message,
            pathGenerationResult.GenerationStatus)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="PathGenerationResult"/>.
        /// </summary>
        /// <param name="operationMethod">The operation method</param>
        /// <param name="path">The path.</param>
        /// <param name="message">The generation message.</param>
        /// <param name="generationStatus">The generation status.</param>
        public PathGenerationResult(string operationMethod, string path, string message, GenerationStatus generationStatus)
        {
            OperationMethod = operationMethod;
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
        public string OperationMethod { get; internal set; }

        /// <summary>
        /// The path.
        /// </summary>
        [JsonProperty]
        public string Path { get; internal set; }

        /// <summary>
        /// Determines whether this equals to the other object.
        /// </summary>
        public override bool Equals(object other)
        {
            var pathGenerationResult = other as PathGenerationResult;

            return pathGenerationResult != null && Equals(pathGenerationResult);
        }

        /// <summary>
        /// Gets the hash code of this path generation result.
        /// </summary>
        public override int GetHashCode() => 
            new {OperationMethod, GenerationStatus, Message, Path}.GetHashCode();

        /// <summary>
        /// Determines whether this equals to the other path generation result.
        /// </summary>
        public bool Equals(PathGenerationResult other)
        {
            return other != null &&
                OperationMethod == other.OperationMethod &&
                GenerationStatus == other.GenerationStatus &&
                Message == other.Message &&
                Path == other.Path;
        }
    }
}