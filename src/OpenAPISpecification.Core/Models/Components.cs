// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.OpenApiSpecification.Core.Models
{
    /// <summary>
    /// Class to hold a set of reusable objects for different aspects of the OAS.
    /// </summary>
    public class Components
    {
        /// <summary>
        /// Gets the reusable CallBacks.
        /// </summary>
        [JsonProperty(PropertyName = "callbacks", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public IDictionary<string, Callback> CallBacks { get; internal set; } = new Dictionary<string, Callback>();

        /// <summary>
        /// Gets the reusable Headers.
        /// </summary>
        [JsonProperty(PropertyName = "headers", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public IDictionary<string, Header> Headers { get; internal set; } = new Dictionary<string, Header>();

        /// <summary>
        /// Gets the reusable Parameters.
        /// </summary>
        [JsonProperty(PropertyName = "parameters", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public IDictionary<string, Parameter> Parameters { get; internal set; } = new Dictionary<string, Parameter>();

        /// <summary>
        /// Gets the reusable Request bodies.
        /// </summary>
        [JsonProperty(PropertyName = "requestBodies", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public IDictionary<string, RequestBody> RequestBodies { get; internal set; } = new Dictionary<string, RequestBody>();

        /// <summary>
        /// Gets the reusable Responses.
        /// </summary>
        [JsonProperty(PropertyName = "responses", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public IDictionary<string, Response> Responses { get; internal set; } = new Dictionary<string, Response>();

        /// <summary>
        /// Gets the reusable Schemas.
        /// </summary>
        [JsonProperty(PropertyName = "schemas", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public IDictionary<string, Schema> Schemas { get; internal set; } = new Dictionary<string, Schema>();
    }
}