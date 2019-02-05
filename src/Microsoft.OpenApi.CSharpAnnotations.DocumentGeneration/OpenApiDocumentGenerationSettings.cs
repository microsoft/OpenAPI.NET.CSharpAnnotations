// ------------------------------------------------------------
//  Copyright (c)Microsoft Corporation.  All rights reserved.
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
            : this(schemaGenerationSettings, false)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="OpenApiDocumentGenerationSettings"/>.
        /// </summary>
        /// <param name="removeRoslynDuplicateStringFromParamName">Indicates whether to remove duplicate string from
        /// parameter name to work around roslyn issue. https://github.com/dotnet/roslyn/issues/26292.
        /// </param>
        public OpenApiDocumentGenerationSettings(bool removeRoslynDuplicateStringFromParamName)
            : this(new SchemaGenerationSettings(new DefaultPropertyNameResolver()),
                removeRoslynDuplicateStringFromParamName)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="OpenApiDocumentGenerationSettings"/>.
        /// </summary>
        /// <param name="schemaGenerationSettings">The schema generation settings.</param>
        /// <param name="removeRoslynDuplicateStringFromParamName">Indicates whether to remove duplicate string from
        /// parameter name to work around rosyln issue.</param>
        public OpenApiDocumentGenerationSettings(
            SchemaGenerationSettings schemaGenerationSettings,
            bool removeRoslynDuplicateStringFromParamName)
        {
            this.SchemaGenerationSettings = schemaGenerationSettings ??
                throw new ArgumentNullException(nameof(schemaGenerationSettings));
            this.RemoveRoslynDuplicateStringFromParamName = removeRoslynDuplicateStringFromParamName;
        }

        /// <summary>
        /// Gets the bool to indicate whether to remove duplicate string from parameter name to work around roslyn
        /// issue. https://github.com/dotnet/roslyn/issues/26292.
        /// </summary>
        public bool RemoveRoslynDuplicateStringFromParamName { get; }

        /// <summary>
        /// Gets the schema generation settings.
        /// </summary>
        public SchemaGenerationSettings SchemaGenerationSettings { get; }
    }
}