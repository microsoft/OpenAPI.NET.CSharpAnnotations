// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.DocumentFilters
{
    /// <summary>
    /// Settings for <see cref="IDocumentFilter"/>.
    /// </summary>
    public class DocumentFilterSettings
    {
        /// <summary>
        /// Gets or sets the type fetcher.
        /// </summary>
        public TypeFetcher TypeFetcher { get; set; }

        /// <summary>
        /// Gets or sets the OpenAPI document version.
        /// </summary>
        public string OpenApiDocumentVersion { get; set; }
    }
}