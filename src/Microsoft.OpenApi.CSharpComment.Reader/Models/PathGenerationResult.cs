// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Newtonsoft.Json;

namespace Microsoft.OpenApi.CSharpComment.Reader.Models
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
        /// Initializes a new instance of <see cref="PathGenerationResult"/> based on the other instance.
        /// </summary>
        public PathGenerationResult(PathGenerationResult other)
        {
            OperationMethod = other.OperationMethod;
            Path = other.Path;
            Message = other.Message;
            ExceptionType = other.ExceptionType;
            GenerationStatus = other.GenerationStatus;
        }

        /// <summary>
        /// The type name of the exception.
        /// </summary>
        public Type ExceptionType { get; set; }

        /// <summary>
        /// The generation status for the path.
        /// </summary>
        [JsonProperty]
        public GenerationStatus GenerationStatus { get; set; }

        /// <summary>
        /// The message providing details on the generation.
        /// </summary>
        [JsonProperty]
        public string Message { get; set; }

        /// <summary>
        /// The path.
        /// </summary>
        [JsonProperty]
        public string OperationMethod { get; set; }

        /// <summary>
        /// The path.
        /// </summary>
        [JsonProperty]
        public string Path { get; set; }

        /// <summary>
        /// Determines whether this equals to the other object.
        /// </summary>
        public override bool Equals(object other)
        {
            var pathGenerationResult = other as PathGenerationResult;

            return pathGenerationResult != null && Equals(pathGenerationResult);
        }

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

        /// <summary>
        /// Gets the hash code of this path generation result.
        /// </summary>
        public override int GetHashCode() =>
            new {OperationMethod, GenerationStatus, Message, Path}.GetHashCode();
    }
}