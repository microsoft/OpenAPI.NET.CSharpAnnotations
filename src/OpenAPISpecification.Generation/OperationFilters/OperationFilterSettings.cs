// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApiSpecification.Generation.ReferenceRegistries;

namespace Microsoft.OpenApiSpecification.Generation.OperationFilters
{
    /// <summary>
    /// Settings for <see cref="IOperationFilter"/>.
    /// </summary>
    public class OperationFilterSettings
    {
        /// <summary>
        /// Gets the reference registry manager.
        /// </summary>
        public ReferenceRegistryManager ReferenceRegistryManager { get; } = new ReferenceRegistryManager();

        /// <summary>
        /// Gets or sets the type fetcher.
        /// </summary>
        public TypeFetcher TypeFetcher { get; set; }
    }
}