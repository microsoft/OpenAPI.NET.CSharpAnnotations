// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.OpenApiSpecification.Core.Models
{
    /// <summary>
    /// Represents the schema and examples for the media type.
    /// </summary>
    public class MediaType
    {
        /// <summary>
        /// Gets the map between a property name and its encoding information.
        /// </summary>
        [JsonProperty(PropertyName = "encoding", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public IDictionary<string, Encoding> Encoding { get; } = new Dictionary<string, Encoding>();

        /// <summary>
        /// Gets or sets the schema defining the type used.
        /// </summary>
        [JsonProperty(PropertyName = "schema", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public Schema Schema { get; set; }
    }
}