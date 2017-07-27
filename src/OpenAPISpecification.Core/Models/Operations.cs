// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.OpenApiSpecification.Core.Models
{
    /// <summary>
    /// Holds all the operations of a path.
    /// </summary>
    public class Operations : Dictionary<OperationMethod, Operation>
    {
        /// <summary>
        /// Gets or sets an optional, string description. Intended to apply to all operations in this path.
        /// </summary>
        [JsonProperty(PropertyName = "description", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets an optional, string summary. Intended to apply to all operations in this path.
        /// </summary>
        [JsonProperty(PropertyName = "summary", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Summary { get; set; }

        /// <summary>
        /// Gets an alternative server array to service all operations in this path.
        /// </summary>
        [JsonProperty(PropertyName = "servers", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public IList<Server> Servers { get; } = new List<Server>();

        /// <summary>
        /// Gets the list of parameters that are applicable for all the operations described under this path.
        /// </summary>
        [JsonProperty(PropertyName = "parameters", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public IDictionary<string, Parameter> Parameters { get; } = new Dictionary<string, Parameter>();
    }
}