// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Linq;
using Microsoft.OpenApi.CSharpComment.Reader.Models;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpComment.Reader.PostProcessingDocumentFilters
{
    /// <summary>
    /// Removes the operations from the OpenAPI document for which generation failed.
    /// </summary>
    public class RemoveFailedGenerationOperationFilter : IPostProcessingDocumentFilter
    {
        /// <summary>
        /// Removes the operations from the OpenAPI document for which generation failed.
        /// </summary>
        /// <param name="openApiDocument">The OpenAPI document to process.</param>
        /// <param name="settings">The filter settings.</param>
        public void Apply(
            OpenApiDocument openApiDocument,
            PostProcessingDocumentFilterSettings settings)
        {
            if(openApiDocument == null || settings == null)
            {
                return;
            }

            foreach (var operationDiagnostic in
                settings.OperationGenerationDiagnostics.Where(
                    operationDiagnostic => operationDiagnostic.GenerationStatus == GenerationStatus.Failure))
            {
                if (!Enum.TryParse(operationDiagnostic.OperationMethod, true, out OperationType operationMethod) ||
                    !openApiDocument.Paths.ContainsKey(operationDiagnostic.Path))
                {
                    continue;
                }

                var operations = openApiDocument.Paths[operationDiagnostic.Path].Operations;

                if (operations.Count == 1)
                {
                    // If there is only one operation under the path and it failed generation, remove complete path.
                    openApiDocument.Paths.Remove(operationDiagnostic.Path);
                }
                else
                {
                    openApiDocument.Paths[operationDiagnostic.Path].Operations.Remove(operationMethod);
                }
            }
        }
    }
}