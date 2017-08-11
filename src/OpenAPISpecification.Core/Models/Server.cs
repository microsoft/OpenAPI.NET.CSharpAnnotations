// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.OpenApiSpecification.Core.Models
{
    /// <summary>
    /// A class representing a Server.
    /// </summary>
    public class Server
    {
        /// <summary>
        /// Gets or sets an optional string describing the host designated by the URL.
        /// </summary>
        [JsonProperty(PropertyName = "description", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Description { get; set; }

        /// <summary>
        /// Gets a map between a variable name and its value.
        /// The value is used for substitution in the server's URL template.
        /// </summary>
        [JsonProperty(PropertyName = "variables", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public IDictionary<string, ServerVariable> ServerVariables { get; } = new Dictionary<string, ServerVariable>();

        /// <summary>
        /// Gets or sets the URL to the target host.
        /// </summary>
        [JsonProperty(PropertyName = "url", Required = Required.Always,
            DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Url { get; set; }
    }
}