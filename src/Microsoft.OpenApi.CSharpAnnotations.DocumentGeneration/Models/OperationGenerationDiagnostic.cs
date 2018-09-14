// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models
{
    /// <summary>
    /// Object containing all diagnostic information related to operation-level of generation process.
    /// </summary>
    [Serializable]
    public class OperationGenerationDiagnostic
    {
        /// <summary>
        /// Default constructor. Required for deserialization.
        /// </summary>
        public OperationGenerationDiagnostic()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="OperationGenerationDiagnostic"/> based on the other instance.
        /// If the other instance given is null, the OperationGenerationDiagnostic with all properties being their 
        /// default values is initialized.
        /// </summary>
        /// <param name="other">Other instance.</param>
        public OperationGenerationDiagnostic(OperationGenerationDiagnostic other)
        {
            if (other == null)
            {
                return;
            }

            OperationMethod = other.OperationMethod;
            Path = other.Path;

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
        /// The operation method.
        /// </summary>
        public string OperationMethod { get; set; }

        /// <summary>
        /// The path.
        /// </summary>
        public string Path { get; set; }
    }
}