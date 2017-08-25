// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.OpenApiSpecification.Core.Models
{
    /// <summary>
    /// Represents a single operation parameter.
    /// </summary>
    public class Parameter : IReferenceable
    {
        /// <summary>
        /// Gets or sets a value indicating whether passing empty-valued parameters is allowed (default: false).
        /// </summary>
        [JsonProperty(PropertyName = "allowEmptyValue", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool AllowEmptyValue { get; set; }

        /// <summary>
        /// Gets a map containing the representations for the parameter.
        /// The key is the media type and the value describes it. The map MUST only contain one entry.
        /// </summary>
        [JsonProperty(PropertyName = "content", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public IDictionary<string, MediaType> Content { get; } = new Dictionary<string, MediaType>();

        /// <summary>
        /// Gets or sets the parameter description.
        /// </summary>
        [JsonProperty(PropertyName = "description", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the explode. When this is true, parameter values of type array or object
        /// generate separate parameters for each value of the array or key-value pair of the map.
        /// For other types of parameters this property has no effect.
        /// When style is form, the default value is true. For all other styles, the default value is false.
        /// </summary>
        [JsonProperty(PropertyName = "explode", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool Explode { get; set; }

        /// <summary>
        /// Gets or sets the location of the parameter (query, header, path)
        /// </summary>
        [JsonProperty(PropertyName = "in", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ParameterKind In { get; set; }

        /// <summary>
        /// Gets or sets whether or not the parameter is required.
        /// </summary>
        [JsonProperty(PropertyName = "required", DefaultValueHandling = DefaultValueHandling.Include)]
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets the name of the Parameter.
        /// </summary>
        [JsonProperty(PropertyName = "name", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Response / Request schema.
        /// </summary>
        [JsonProperty(PropertyName = "schema", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public Schema Schema { get; set; }

        /// <summary>
        /// Gets or sets how the parameter value will be serialized depending on the type of the parameter value.
        /// </summary>
        [JsonProperty(PropertyName = "style", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Style { get; set; }

        /// <summary>
        /// Gets or sets the reference string.
        /// </summary>
        /// <remarks>If this is present, the rest of the object will be ignored.</remarks>
        public string Reference { get; set; }
    }
}