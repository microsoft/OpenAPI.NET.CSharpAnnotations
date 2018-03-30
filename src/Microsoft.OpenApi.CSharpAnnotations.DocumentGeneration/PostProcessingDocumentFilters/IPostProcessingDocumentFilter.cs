// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.DocumentFilters;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.OperationFilters;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.PostProcessingDocumentFilters
{
    /// <summary>
    /// The class representing the contract of a filter to post process the <see cref="OpenApiDocument"/>
    /// after its processed by <see cref="IOperationFilter"/> and <see cref="IDocumentFilter"/>.
    /// </summary>
    public interface IPostProcessingDocumentFilter : IFilter
    {
        /// <summary>
        /// Applies the filter to post process the <see cref="OpenApiDocument"/>.
        /// </summary>
        /// <param name="openApiDocument">The OpenAPI document to process.</param>
        /// <param name="settings"><see cref="PostProcessingDocumentFilterSettings"/></param>
        void Apply(OpenApiDocument openApiDocument, PostProcessingDocumentFilterSettings settings);
    }
}