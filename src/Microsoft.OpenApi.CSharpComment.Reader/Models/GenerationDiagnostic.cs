// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Readers.Interface;

namespace Microsoft.OpenApi.CSharpComment.Reader.Models
{
    /// <summary>
    /// Object coining all diagnostic information related to OpenApi document generation.
    /// </summary>
    public class GenerationDiagnostic : IDiagnostic
    {
        /// <summary>
        /// The document-level generation diagnostics (e.g. from applying document-level filters).
        /// </summary>
        public DocumentGenerationDiagnostic DocumentGenerationDiagnostic { get; set; }

        /// <summary>
        /// The generation status.
        /// </summary>
        public GenerationStatus GenerationStatus
        {
            get
            {
                if (OperationGenerationDiagnostics.Any(i => i.GenerationStatus == GenerationStatus.Failure) ||
                    DocumentGenerationDiagnostic.GenerationStatus == GenerationStatus.Failure)
                {
                    return GenerationStatus.Failure;
                }

                if (OperationGenerationDiagnostics.Any(i => i.GenerationStatus == GenerationStatus.Warning) ||
                    DocumentGenerationDiagnostic.GenerationStatus == GenerationStatus.Warning)
                {
                    return GenerationStatus.Warning;
                }

                return GenerationStatus.Success;
            }
        }

        /// <summary>
        /// List of operation-level generation diagnostics.
        /// </summary>
        public IList<OperationGenerationDiagnostic> OperationGenerationDiagnostics { get; internal set; } =
            new List<OperationGenerationDiagnostic>();
    }
}