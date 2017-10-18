// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.OpenApiSpecification.Core.Models
{
    /// <summary>
    /// Represents header.
    /// </summary>
    public class Header : IReferenceable
    {
        /// <summary>
        /// Gets or sets the reference string.
        /// </summary>
        /// <remarks>If this is present, the rest of the object will be ignored.</remarks>
        public string Reference { get; set; }
    }
}