// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration
{
    /// <summary>
    /// Specifies the settings on a <see cref="OpenApiGenerator"/> object.
    /// </summary>
    public class OpenApiDocumentGenerationSettings
    {
        /// <summary>
        /// Creates an instance of <see cref="OpenApiDocumentGenerationSettings"/>.
        /// </summary>
        /// <param name="schemaGenerationSettings">The schema generation settings.</param>
        public OpenApiDocumentGenerationSettings(SchemaGenerationSettings schemaGenerationSettings)
        {
            this.SchemaGenerationSettings = schemaGenerationSettings ??
                throw new ArgumentNullException(nameof(schemaGenerationSettings));
        }

        /// <summary>
        /// Gets the schema generation settings.
        /// </summary>
        public SchemaGenerationSettings SchemaGenerationSettings { get; }
    }
}