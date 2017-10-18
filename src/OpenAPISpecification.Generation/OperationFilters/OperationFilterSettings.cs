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
        /// Gets or sets the operation method for the operation.
        /// </summary>
        public string OperationMethod { get; set; }

        /// <summary>
        /// Gets or sets the path for the operation.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets the reference registry manager.
        /// </summary>
        public ReferenceRegistryManager ReferenceRegistryManager { get; set; }

        /// <summary>
        /// Gets or sets the type fetcher.
        /// </summary>
        public TypeFetcher TypeFetcher { get; set; }
    }
}