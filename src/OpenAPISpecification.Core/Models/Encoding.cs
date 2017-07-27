// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.OpenApiSpecification.Core.Models
{
    /// <summary>
    /// Represents encoding for the schema.
    /// </summary>
    public class Encoding
    {
        /// <summary>
        /// Gets or sets if the the parameter value SHOULD allow reserved character.
        /// </summary>
        [JsonProperty(PropertyName = "allowReserved", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool AllowReserved { get; set; }

        /// <summary>
        /// Gets or sets the Content-Type for encoding a specific property.
        /// </summary>
        [JsonProperty(PropertyName = "contentType", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets to explode or not.
        /// When this is true, property values of type array or object generate separate parameters for each
        /// value of the array, or key-value-pair of the map. For other types of properties this property has no effect.
        /// </summary>
        [JsonProperty(PropertyName = "explode", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool Explode { get; set; }

        /// <summary>
        /// Gets the map allowing additional information to be provided as headers,
        /// </summary>
        [JsonProperty(PropertyName = "headers", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public IDictionary<string, Header> Headers { get; } = new Dictionary<string, Header>();

        /// <summary>
        /// Gets or sets how a specific property value will be serialized depending on its type.
        /// </summary>
        [JsonProperty(PropertyName = "style", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Style { get; set; }
    }
}