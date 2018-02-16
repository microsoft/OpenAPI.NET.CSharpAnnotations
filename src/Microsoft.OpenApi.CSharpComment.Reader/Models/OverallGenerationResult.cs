// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Readers.Interface;

namespace Microsoft.OpenApi.CSharpComment.Reader.Models
{
    /// <summary>
    /// The class to store the overall generation result.
    /// </summary>
    public class OverallGenerationResult : IDiagnostic
    {
        /// <summary>
        /// The document-level generation result (e.g. from applying document-level filters)
        /// </summary>
        public DocumentGenerationResult DocumentGenerationResult { get; set; }

        /// <summary>
        /// The generation status.
        /// </summary>
        public GenerationStatus GenerationStatus
        {
            get
            {
                if (OperationGenerationResults.Any(i => i.GenerationStatus == GenerationStatus.Failure) ||
                    DocumentGenerationResult.GenerationStatus == GenerationStatus.Failure)
                {
                    return GenerationStatus.Failure;
                }

                if (OperationGenerationResults.Any(i => i.GenerationStatus == GenerationStatus.Warning) ||
                    DocumentGenerationResult.GenerationStatus == GenerationStatus.Warning)
                {
                    return GenerationStatus.Warning;
                }

                return GenerationStatus.Success;
            }
        }

        /// <summary>
        /// List of operation-level generation results.
        /// </summary>
        public IList<OperationGenerationResult> OperationGenerationResults { get; internal set; } =
            new List<OperationGenerationResult>();
    }
}