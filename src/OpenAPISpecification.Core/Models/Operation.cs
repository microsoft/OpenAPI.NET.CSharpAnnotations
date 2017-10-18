// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.OpenApiSpecification.Core.Models
{
    /// <summary>
    /// Describes a single API operation on a path.
    /// </summary>
    public class Operation
    {
        /// <summary>
        /// Gets or sets the detailed explanation of operation.
        /// </summary>
        [JsonProperty(PropertyName = "description", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the external documentation.
        /// </summary>
        [JsonProperty(PropertyName = "externalDocs", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ExternalDocumentation ExternalDocumentation { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for operation.
        /// </summary>
        [JsonProperty(PropertyName = "operationId", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string OperationId { get; set; }

        /// <summary>
        /// Gets or sets the list of parameters that are applicable for this operation.
        /// </summary>
        [JsonProperty(PropertyName = "parameters", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IList<Parameter> Parameters { get; internal set; } = new List<Parameter>();

        /// <summary>
        /// Gets or sets the request body applicable for the operation.
        /// </summary>
        [JsonProperty(PropertyName = "requestBody", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public RequestBody RequestBody { get; set; }

        /// <summary>
        /// Gets the Dictionary of possible response code - response
        /// </summary>
        [JsonProperty(PropertyName = "responses", Required = Required.Always,
            DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, Response> Responses { get; internal set; } = new Dictionary<string, Response>();

        /// <summary>
        /// Gets the collection of security description.
        /// </summary>
        [JsonProperty(PropertyName = "security", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IList<SecurityRequirement> Security { get; internal set; } = new List<SecurityRequirement>();

        /// <summary>
        /// Gets an alternative server array to service this operation.
        /// If an alternative server object is specified at the Root level, it will be overridden by this value.
        /// </summary>
        [JsonProperty(PropertyName = "servers", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IList<Server> Servers { get; internal set; } = new List<Server>();

        /// <summary>
        /// Gets or sets the summary of the operation.
        /// </summary>
        [JsonProperty(PropertyName = "summary", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Summary { get; set; }

        /// <summary>
        /// Gets the tags for the API documentation control.
        /// </summary>
        [JsonProperty(PropertyName = "tags", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IList<string> Tags { get; internal set; } = new List<string>();
    }
}