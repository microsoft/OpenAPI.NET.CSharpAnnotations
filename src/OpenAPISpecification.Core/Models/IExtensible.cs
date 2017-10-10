// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OpenApiSpecification.Core.Models
{
    /// <summary>
    /// Interface for models that are extensible.
    /// </summary>
    public interface IExtensible
    {
        /// <summary>
        /// Gets the extension properties.
        /// </summary>
        IDictionary<string, object> Extensions { get; set; }
    }
}