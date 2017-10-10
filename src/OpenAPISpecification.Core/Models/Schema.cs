// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApiSpecification.Core.Serialization;
using Newtonsoft.Json;

namespace Microsoft.OpenApiSpecification.Core.Models
{
    /// <summary>
    /// The schema defining the type.
    /// </summary>
    [JsonConverter(typeof(ExtensibleJsonConverter))]
    public class Schema : IReferenceable, IExtensible
    {
        /// <summary>
        /// Gets or sets the schema for the additional properties.
        /// </summary>
        [JsonProperty(
            PropertyName = "additionalProperties",
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public Schema AdditionalProperties { get; set; }

        /// <summary>
        /// Gets the collection of schemas where each schema must be valid.
        /// </summary>
        [JsonProperty(PropertyName = "allOf", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public IList<Schema> AllOf { get; internal set; } = new List<Schema>();

        /// <summary>
        /// Gets the collection of schemas where at least one must be valid.
        /// </summary>
        [JsonProperty(PropertyName = "anyOf", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public IList<Schema> AnyOf { get; internal set; } = new List<Schema>();

        /// <summary>
        /// Gets or sets the default value.
        /// </summary>
        [JsonProperty(PropertyName = "default", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public object Default { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [JsonProperty(PropertyName = "description", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the schema property name that is used to differentiate between other schema that inherit this schema.
        /// </summary>
        [JsonProperty(PropertyName = "discriminator", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public Discriminator Discriminator { get; set; }

        /// <summary>
        /// Gets the possible enumerated values.
        /// </summary>
        [JsonProperty(PropertyName = "enum", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public IList<string> Enum { get; internal set; } = new List<string>();

        /// <summary>
        /// Gets or sets the free-form property to include a an example of an instance for this schema.
        /// </summary>
        [JsonProperty(PropertyName = "example", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public object Example { get; set; }

        /// <summary>
        /// Gets the extension properties
        /// </summary>
        public IDictionary<string, object> Extensions { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets or sets the additional external documentation for this schema.
        /// </summary>
        [JsonProperty(PropertyName = "externalDocs", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public ExternalDocumentation ExternalDocs { get; set; }

        /// <summary>
        /// Gets or sets the format string.
        /// </summary>
        [JsonProperty(PropertyName = "format", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Format { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the maximum value is excluded.
        /// </summary>
        [JsonProperty(PropertyName = "exclusiveMaximum", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool IsExclusiveMaximum { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the minimum value is excluded.
        /// </summary>
        [JsonProperty(PropertyName = "exclusiveMinimum", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool IsExclusiveMinimum { get; set; }

        /// <summary>
        /// Gets or sets the schemas of the array's tuple values.
        /// </summary>
        [JsonProperty(PropertyName = "items", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public Schema Items { get; set; }

        /// <summary>
        /// Gets or sets the maximum allowed value.
        /// </summary>
        [JsonProperty(PropertyName = "maximum", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int? Maximum { get; set; }

        /// <summary>
        /// Gets or sets the maximum length of the array.
        /// </summary>
        [JsonProperty(PropertyName = "maxItems", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int MaxItems { get; set; }

        /// <summary>
        /// Gets or sets the maximum length of the value string.
        /// </summary>
        [JsonProperty(PropertyName = "maxLength", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int? MaxLength { get; set; }

        /// <summary>
        /// Gets or sets the maximal number of allowed properties in an object.
        /// </summary>
        [JsonProperty(PropertyName = "maxProperties", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int MaxProperties { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the minimum value is excluded.
        /// </summary>
        [JsonProperty(PropertyName = "minimum", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int? Minimum { get; set; }

        /// <summary>
        /// Gets or sets the minimum length of the array.
        /// </summary>
        [JsonProperty(PropertyName = "minItems", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int MinItems { get; set; }

        /// <summary>
        /// Gets or sets the minimum length of the value string.
        /// </summary>
        [JsonProperty(PropertyName = "minLength", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int? MinLength { get; set; }

        /// <summary>
        /// Gets or sets the minimal number of allowed properties in an object.
        /// </summary>
        [JsonProperty(PropertyName = "minProperties", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int MinProperties { get; set; }

        /// <summary>
        /// Gets or sets the required multiple of for the number value.
        /// </summary>
        [JsonProperty(PropertyName = "multipleOf", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int? MultipleOf { get; set; }

        /// <summary>
        /// Gets or sets the schema which must not be valid.
        /// </summary>
        [JsonProperty(PropertyName = "not", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public Schema Not { get; set; }

        /// <summary>
        /// Gets the collection of schemas where exactly one must be valid.
        /// </summary>
        [JsonProperty(PropertyName = "oneOf", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public IList<Schema> OneOf { get; internal set; } = new List<Schema>();

        /// <summary>
        /// Gets or sets the validation pattern as regular expression.
        /// </summary>
        [JsonProperty(PropertyName = "pattern", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Pattern { get; set; }

        /// <summary>
        /// Gets the properties of the type.
        /// </summary>
        [JsonProperty(PropertyName = "properties", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public IDictionary<string, Schema> Properties { get; internal set; } = new Dictionary<string, Schema>();

        /// <summary>
        /// Gets or sets if a schema is read only.
        /// </summary>
        [JsonProperty(PropertyName = "readOnly", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool? ReadOnly { get; set; }

        /// <summary>
        /// Gets or sets the reference string.
        /// </summary>
        /// <remarks>If this is present, the rest of the object will be ignored.</remarks>
        [JsonProperty(PropertyName = "$ref", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Reference { get; set; }

        /// <summary>
        /// Gets or sets the list of properties that are required in the schema.
        /// </summary>
        [JsonProperty(PropertyName = "required", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public IList<string> RequiredProperties { get; internal set; } = new List<string>();

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [JsonProperty(PropertyName = "title", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Title { get; set; }

        /// <summary>
        /// Gets the object type.
        /// </summary>
        [JsonProperty(PropertyName = "type", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the items in the array must be unique.
        /// </summary>
        [JsonProperty(PropertyName = "uniqueItems", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool UniqueItems { get; set; }
    }
}