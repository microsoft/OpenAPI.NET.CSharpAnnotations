// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Extensions
{
    /// <summary>
    /// Class containing extensions for <see cref="OpenApiSchema"/>.
    /// </summary>
    public static class OpenApiSchemaExtensions
    {
        /// <summary>
        /// Copies source <see cref="OpenApiSchema"/> to target <see cref="OpenApiSchema"/>.
        /// </summary>
        /// <param name="sourceOpenApiSchema">The source schema to copy.</param>
        /// <param name="targetOpenApiSchema">The target schema to copy into.</param>
        public static void CopyInto(this OpenApiSchema sourceOpenApiSchema, OpenApiSchema targetOpenApiSchema)
        {
            if(targetOpenApiSchema == null)
            {
                throw new ArgumentNullException(nameof(targetOpenApiSchema));
            }

            if (sourceOpenApiSchema == null)
            {
                throw new ArgumentNullException(nameof(sourceOpenApiSchema));
            }

            targetOpenApiSchema.Format = sourceOpenApiSchema.Format;
            targetOpenApiSchema.Type = sourceOpenApiSchema.Type;
            targetOpenApiSchema.Maximum = sourceOpenApiSchema.Maximum;
            targetOpenApiSchema.Minimum = sourceOpenApiSchema.Minimum;
            targetOpenApiSchema.MaxItems = sourceOpenApiSchema.MaxItems;
            targetOpenApiSchema.MaxLength = sourceOpenApiSchema.MaxLength;
            targetOpenApiSchema.Properties = sourceOpenApiSchema.Properties;
            targetOpenApiSchema.MaxProperties = sourceOpenApiSchema.MaxProperties;
            targetOpenApiSchema.Enum = sourceOpenApiSchema.Enum;
            targetOpenApiSchema.Discriminator = sourceOpenApiSchema.Discriminator;
            targetOpenApiSchema.AdditionalProperties = sourceOpenApiSchema.AdditionalProperties;
            targetOpenApiSchema.AdditionalPropertiesAllowed = sourceOpenApiSchema.AdditionalPropertiesAllowed;
            targetOpenApiSchema.Nullable = sourceOpenApiSchema.Nullable;
            targetOpenApiSchema.Items = sourceOpenApiSchema.Items;
            targetOpenApiSchema.Example = sourceOpenApiSchema.Example;
            targetOpenApiSchema.AnyOf = sourceOpenApiSchema.AnyOf;
            targetOpenApiSchema.Reference = sourceOpenApiSchema.Reference;
            targetOpenApiSchema.OneOf = sourceOpenApiSchema.OneOf;
            targetOpenApiSchema.AllOf = sourceOpenApiSchema.AllOf;
            targetOpenApiSchema.Description = sourceOpenApiSchema.Description;
            targetOpenApiSchema.Deprecated = sourceOpenApiSchema.Deprecated;
            targetOpenApiSchema.Default = sourceOpenApiSchema.Default;
            targetOpenApiSchema.ExclusiveMaximum = sourceOpenApiSchema.ExclusiveMaximum;
            targetOpenApiSchema.ExclusiveMinimum = sourceOpenApiSchema.ExclusiveMinimum;
            targetOpenApiSchema.ExternalDocs = sourceOpenApiSchema.ExternalDocs;
            targetOpenApiSchema.Reference = sourceOpenApiSchema.Reference;
            targetOpenApiSchema.Required = sourceOpenApiSchema.Required;
            targetOpenApiSchema.ReadOnly = sourceOpenApiSchema.ReadOnly;
        }
    }
}
