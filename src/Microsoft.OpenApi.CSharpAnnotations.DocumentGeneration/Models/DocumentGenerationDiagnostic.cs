// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models
{
    /// <summary>
    /// Object containing all diagnostic information related to document-level of generation process.
    /// </summary>
    [Serializable]
    public class DocumentGenerationDiagnostic
    {
        /// <summary>
        /// Default constructor. Required for deserialization.
        /// </summary>
        public DocumentGenerationDiagnostic()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DocumentGenerationDiagnostic"/> based on the other instance.
        /// If the other instance given is null, the DocumentGenerationDiagnostic with all properties being their 
        /// default values is initialized.
        /// </summary>
        /// <param name="other">Other instance.</param>
        public DocumentGenerationDiagnostic(DocumentGenerationDiagnostic other)
        {
            if (other == null)
            {
                return;
            }

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
    }
}