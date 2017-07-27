// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Newtonsoft.Json;

namespace Microsoft.OpenApiSpecification.Core.Models
{
    /// <summary>
    /// Represents the contact information for the exposed Api.
    /// </summary>
    public class Contact
    {
        /// <summary>
        /// Gets or sets the email address of the contact person/organization.
        /// </summary>
        [JsonProperty(PropertyName = "email", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the identifying name of the contact person/organization.
        /// </summary>
        [JsonProperty(PropertyName = "name", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the URL pointing to the contact information.
        /// </summary>
        [JsonProperty(PropertyName = "url", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public Uri Url { get; set; }
    }
}