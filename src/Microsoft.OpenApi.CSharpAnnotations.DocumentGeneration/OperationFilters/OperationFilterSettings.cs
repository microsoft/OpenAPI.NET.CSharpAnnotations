// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.ReferenceRegistries;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.OperationFilters
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
        /// Gets or sets the reference registry manager.
        /// </summary>
        public ReferenceRegistryManager ReferenceRegistryManager { get; set; }

        /// <summary>
        /// Gets or sets the bool to indicate whether to remove duplicate string from parameter name to work around
        /// roslyn issue. https://github.com/dotnet/roslyn/issues/26292.
        /// </summary>
        public bool RemoveRoslynDuplicateStringFromParamName { get; set; }

        /// <summary>
        /// Gets or sets the type fetcher.
        /// </summary>
        public TypeFetcher TypeFetcher { get; set; }
    }
}