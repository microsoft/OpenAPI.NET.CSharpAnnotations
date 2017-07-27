// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.OpenApiSpecification.Core.Models
{
    /// <summary>
    /// Represents the expected response of an operation.
    /// </summary>
    public class Response
    {
        /// <summary>
        /// Gets the map containing descriptions of potential response payloads.
        /// The key is the media type and the value is used to describe it.
        /// </summary>
        [JsonProperty(PropertyName = "content", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public IDictionary<string, MediaType> Content { get; } = new Dictionary<string, MediaType>();

        /// <summary>
        /// Gets or sets the Response description.
        /// </summary>
        [JsonProperty(PropertyName = "description", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
            Required = Required.Always)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the headers.
        /// </summary>
        [JsonProperty(PropertyName = "headers", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public IDictionary<string, Header> Headers { get; } = new Dictionary<string, Header>();
    }
}