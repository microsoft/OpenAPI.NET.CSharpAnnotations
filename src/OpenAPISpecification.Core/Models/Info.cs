// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Newtonsoft.Json;

namespace Microsoft.OpenApiSpecification.Core.Models
{
    /// <summary>
    /// Represents the metadata about the API.
    /// </summary>
    public class Info
    {
        /// <summary>
        /// Gets or sets the contact information.
        /// </summary>
        [JsonProperty(PropertyName = "contact", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public Contact Contact { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [JsonProperty(PropertyName = "description", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the license information.
        /// </summary>
        [JsonProperty(PropertyName = "license", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public License License { get; set; }

        /// <summary>
        /// Gets or sets the terms of service.
        /// </summary>
        [JsonProperty(PropertyName = "termsOfService", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public Uri TermsOfServiceUrl { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [JsonProperty(PropertyName = "title", Required = Required.Always,
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Title { get; set; } = "Swagger specification";

        /// <summary>
        /// Gets or sets the API version.
        /// </summary>
        [JsonProperty(PropertyName = "version", Required = Required.Always,
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Version { get; set; } = "1.0.0";
    }
}