// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.OpenApi.CSharpComment.Reader.Models
{
    /// <summary>
    /// Model representing the result of the document-level portion of the generation process.
    /// </summary>
    public class DocumentGenerationResult
    {
        /// <summary>
        /// Default constructor. Required for deserialization.
        /// </summary>
        public DocumentGenerationResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DocumentGenerationResult"/> based on the other instance.
        /// If the other instance given is null, the DocumentGenerationResult with all properties being their 
        /// default values is initialized.
        /// </summary>
        /// <param name="other">Other instance.</param>
        public DocumentGenerationResult(DocumentGenerationResult other)
        {
            if (other == null)
            {
                return;
            }

            GenerationStatus = other.GenerationStatus;

            if (other.Errors != null)
            {
                foreach (var error in other.Errors)
                {
                    Errors.Add(new GenerationError(error));
                }
            }
        }

        /// <summary>
        /// List of generation errors for this operation.
        /// </summary>
        public IList<GenerationError> Errors { get; } = new List<GenerationError>();

        /// <summary>
        /// The generation status for the operation.
        /// </summary>
        [JsonProperty]
        public GenerationStatus GenerationStatus { get; set; }
    }
}