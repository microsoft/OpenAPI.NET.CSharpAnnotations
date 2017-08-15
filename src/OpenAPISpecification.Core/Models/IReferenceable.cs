// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Newtonsoft.Json;

namespace Microsoft.OpenApiSpecification.Core.Models
{
    /// <summary>
    /// Interface for models that are referenceable.
    /// </summary>
    public interface IReferenceable
    {
        /// <summary>
        /// Gets or sets the reference string to the defined model in <see cref="Components"/> section.
        /// </summary>
        [JsonProperty(PropertyName = "$ref", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        string Reference { get; set; }
    }
}