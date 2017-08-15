// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Newtonsoft.Json;

namespace Microsoft.OpenApiSpecification.Core.Models
{
    /// <summary>
    /// Represents an example illustrating the possible values for each field of an object.
    /// </summary>
    public class Example : IReferenceable
    {
        /// <summary>
        /// Gets or sets the long description of the example.
        /// </summary>
        [JsonProperty(PropertyName = "description", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the URL that points to the literal example.
        /// </summary>
        /// <remarks>
        /// This can be used for examples that cannot be easily included in JSON documents.
        /// Only one of this field and <see cref="Value"/> field should be populated.
        /// </remarks>
        [JsonProperty(PropertyName = "externalValue", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string ExternalValue { get; set; }

        /// <summary>
        /// Gets or sets the short description of the example.
        /// </summary>
        [JsonProperty(PropertyName = "summary", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets the literal example.
        /// </summary>
        /// <remarks>Only one of this field and <see cref="ExternalValue"/> field should be populated.</remarks>
        [JsonProperty(PropertyName = "value", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets the reference string.
        /// </summary>
        /// <remarks>If this is present, the rest of the object will be ignored.</remarks>
        public string Reference { get; set; }
    }
}