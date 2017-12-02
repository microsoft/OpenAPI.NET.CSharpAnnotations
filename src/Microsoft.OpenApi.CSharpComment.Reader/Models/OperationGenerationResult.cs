// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Microsoft.OpenApi.CSharpComment.Reader.Models
{
    /// <summary>
    /// Model representing the generation result for the path.
    /// </summary>
    public class OperationGenerationResult
    {
        /// <summary>
        /// Default constructor. Required for deserialization.
        /// </summary>
        public OperationGenerationResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="OperationGenerationResult"/> based on the other instance.
        /// </summary>
        public OperationGenerationResult(OperationGenerationResult other)
        {
            OperationMethod = other.OperationMethod;
            Path = other.Path;
            GenerationStatus = other.GenerationStatus;
            foreach (var error in other.Errors)
            {
                Errors.Add(new OperationGenerationError(error));
            }
        }


        /// <summary>
        /// List of generation errors for this operation.
        /// </summary>
        public IList<OperationGenerationError> Errors { get; } = new List<OperationGenerationError>();

       
        /// <summary>
        /// The generation status for the operation.
        /// </summary>
        [JsonProperty]
        public GenerationStatus GenerationStatus { get; set; }

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
    }
}