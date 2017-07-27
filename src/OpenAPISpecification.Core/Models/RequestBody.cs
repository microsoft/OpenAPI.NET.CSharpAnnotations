// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.OpenApiSpecification.Core.Models
{
    /// <summary>
    /// Represents the request body for the operation.
    /// </summary>
    public class RequestBody
    {
        /// <summary>
        /// Gets the content of the request body.The key is the media type and the value describes it.
        /// </summary>
        [JsonProperty(PropertyName = "content", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
            Required = Required.Always)]
        public IDictionary<string, MediaType> Content { get; } = new Dictionary<string, MediaType>();

        /// <summary>
        /// Gets or sets the brief description of the request body.
        /// </summary>
        [JsonProperty(PropertyName = "description", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets whether or not the request body is required.
        /// </summary>
        [JsonProperty(PropertyName = "required", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool IsRequired { get; set; }
    }
}