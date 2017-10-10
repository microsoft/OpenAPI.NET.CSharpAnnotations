// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.OpenApiSpecification.Core.Models
{
    /// <summary>
    /// Represents a Server Variable for server URL template substitution.
    /// </summary>
    public class ServerVariable
    {
        /// <summary>
        /// Gets or sets the default value to use for substitution, and to send, if an alternate value is not supplied.
        /// </summary>
        [JsonProperty(PropertyName = "default", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets an optional description for the server variable.
        /// </summary>
        [JsonProperty(PropertyName = "description", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Description { get; set; }

        /// <summary>
        /// Gets an enumeration of string values to be used if the substitution options are from a limited set.
        /// </summary>
        [JsonProperty(PropertyName = "enum", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
            Required = Required.Always)]
        public IList<string> Enum { get; internal set; } = new List<string>();
    }
}