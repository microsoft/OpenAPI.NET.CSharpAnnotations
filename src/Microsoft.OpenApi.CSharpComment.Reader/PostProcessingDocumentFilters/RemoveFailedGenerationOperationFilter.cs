// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.CSharpComment.Reader.Models;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpComment.Reader.PostProcessingDocumentFilters
{
    /// <summary>
    /// 
    /// </summary>
    public class RemoveFailedGenerationOperationFilter : IPostProcessingDocumentFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="specificationDocuments"></param>
        /// <param name="settings"></param>
        public void Apply(
            IDictionary<DocumentVariantInfo, OpenApiDocument> specificationDocuments,
            PostProcessingDocumentFilterSettings settings)
        {
            foreach(var operationDiagnostic in
                settings.OperationGenerationDiagnostics.Where(
                    operationDiagnostic => operationDiagnostic.GenerationStatus == GenerationStatus.Failure))
            {
                foreach(var key in specificationDocuments.Keys)
                {
                    var document = specificationDocuments[key];

                    OperationType operationMethod;
                    Enum.TryParse(operationDiagnostic.OperationMethod, true, out operationMethod);

                    if (document.Paths.ContainsKey(operationDiagnostic.Path))
                    {
                        document.Paths[operationDiagnostic.Path].Operations.Remove(operationMethod);
                    }
                }
            }
        }
    }
}