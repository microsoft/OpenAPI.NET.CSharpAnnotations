// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApiSpecification.Generation.OperationFilters;

namespace Microsoft.OpenApiSpecification.Generation.OperationConfigFilters
{
    /// <summary>
    /// Settings for <see cref="IOperationConfigFilter"/>.
    /// </summary>
    public class OperationConfigFilterSettings
    {
        /// <summary>
        /// List of all operation filters
        /// </summary>
        public IList<IOperationFilter> OperationFilters { get; set; }

        /// <summary>
        /// The settings for the operation filters.
        /// </summary>
        public OperationFilterSettings OperationFilterSettings { get; set; }
    }
}