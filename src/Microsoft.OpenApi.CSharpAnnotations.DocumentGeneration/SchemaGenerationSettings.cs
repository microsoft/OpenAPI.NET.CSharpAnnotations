// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration
{
    /// <summary>
    /// Specifies the settings for schema generation.
    /// </summary>
    public class SchemaGenerationSettings
    {
        /// <summary>
        /// Creates an instance of <see cref="SchemaGenerationSettings"/> class.
        /// </summary>
        /// <param name="propertyNameResolver">The property name resolver.</param>
        public SchemaGenerationSettings(IPropertyNameResolver propertyNameResolver)
        {
            PropertyNameResolver = propertyNameResolver
                ?? throw new ArgumentNullException(nameof(propertyNameResolver));
        }

        /// <summary>
        /// Gets the property name resolver.
        /// </summary>
        public IPropertyNameResolver PropertyNameResolver { get; }
    }
}