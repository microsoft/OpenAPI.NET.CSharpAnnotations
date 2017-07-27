// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.OpenApiSpecification.Core.Models
{
    /// <summary>
    /// Represents the object that is used to differentiate between schemas.
    /// When request bodies or response payloads may be one of a number of different schemas,
    /// a discriminator object can be used to aid in serialization, deserialization, and validation.
    /// </summary>
    public class Discriminator
    {
        /// <summary>
        /// Gets or sets the property in the payload that will hold the discriminator value.
        /// </summary>
        [JsonProperty(PropertyName = "propertyName", Required = Required.Always,
            DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets the mapping between payload values and schema names or references.
        /// </summary>
        [JsonProperty(PropertyName = "mapping", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, string> Mapping { get; } = new Dictionary<string, string>();
    }
}