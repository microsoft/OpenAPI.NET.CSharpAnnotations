// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.OpenApiSpecification.Core.Models
{
    /// <summary>
    /// The class representing OpenApi V3 specification document.
    /// The specification is documented at https://github.com/OAI/OpenAPI-Specification/blob/3.0.0-rc2/versions/3.0.md
    /// </summary>
    public class OpenApiV3SpecificationDocument
    {
        /// <summary>
        /// Gets or sets the element that holds a set of reusable objects for different aspects of the OAS.
        /// </summary>
        [JsonProperty(PropertyName = "components", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Components Components { get; set; } = new Components();

        /// <summary>
        /// Gets or sets the external documentation.
        /// </summary>
        [JsonProperty(PropertyName = "externalDocs", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ExternalDocumentation ExternalDocumentation { get; set; }

        /// <summary>
        /// Gets or sets the metadata about the API.
        /// </summary>
        [JsonProperty(
            PropertyName = "info",
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
            Required = Required.Always)]
        public Info Info { get; set; } = new Info();

        /// <summary>
        /// Gets or sets the Open API Specification version.
        /// </summary>
        [JsonProperty(
            PropertyName = "openapi",
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
            Required = Required.Always)]
        public string OpenApi { get; set; } = "3.0.0";

        /// <summary>
        /// Gets or sets the available paths for the API
        /// </summary>
        [JsonProperty(
            PropertyName = "paths",
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
            Required = Required.Always)]
        public IDictionary<string, Operations> Paths { get; } = new Dictionary<string, Operations>();

        /// <summary>
        /// Gets the collection of security requirement.
        /// </summary>
        [JsonProperty(PropertyName = "security", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IList<SecurityRequirement> Security { get; } = new List<SecurityRequirement>();

        /// <summary>
        /// Gets the collection of Server Objects, which provide connectivity information to a target server.
        /// </summary>
        [JsonProperty(PropertyName = "servers", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public IList<Server> Servers { get; } = new List<Server>();

        /// <summary>
        /// Gets the collection of tag.
        /// </summary>
        [JsonProperty(PropertyName = "tags", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IList<Tag> Tags { get; } = new List<Tag>();
    }
}