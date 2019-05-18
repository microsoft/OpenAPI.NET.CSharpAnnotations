// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Exceptions;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.PostProcessingDocumentFilters
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
        /// <returns>The list of generation errors, if any produced when processing the filter.</returns>
        public IList<GenerationError> Apply(
            OpenApiDocument openApiDocument,
            PostProcessingDocumentFilterSettings settings)
        {
            var generationErrors = new List<GenerationError>();

            try
            {
                if (openApiDocument == null || settings == null)
                {
                    return generationErrors;
                }

                // Remove all operations with generation errors except for with DuplicateOperationException as only the
                // first occurrence of the duplicate paths added to the document and rest all are never added to the document.
                foreach (var operationDiagnostic in
                    settings.OperationGenerationDiagnostics.Where(
                        operationDiagnostic => operationDiagnostic.Errors.Any()
                        && operationDiagnostic.Errors.Any(i => i.ExceptionType
                        != typeof(DuplicateOperationException).Name)))
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
            catch(Exception ex)
            {
                generationErrors.Add(
                   new GenerationError
                   {
                       Message = ex.Message,
                       ExceptionType = ex.GetType().Name
                   });
            }

            return generationErrors;
        }
    }
}