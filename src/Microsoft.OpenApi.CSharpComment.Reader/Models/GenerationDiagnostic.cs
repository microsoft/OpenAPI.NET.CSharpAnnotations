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
        /// List of operation-level generation diagnostics.
        /// </summary>
        public IList<OperationGenerationDiagnostic> OperationGenerationDiagnostics { get; internal set; } =
            new List<OperationGenerationDiagnostic>();
    }
}