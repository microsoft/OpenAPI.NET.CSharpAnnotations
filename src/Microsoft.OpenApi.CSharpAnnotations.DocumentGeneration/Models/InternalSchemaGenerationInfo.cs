// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models
{
    /// <summary>
    ///The internal class to hold schema generation info and used to transport data across app domain.
    /// </summary>
    internal class InternalSchemaGenerationInfo
    {
        /// <summary>
        /// Gets or sets the error encountered while generating schema.
        /// </summary>
        public GenerationError Error { get; set; }

        /// <summary>
        /// Gets or sets the serialized <see cref="OpenApiSchema"/>
        /// </summary>
        public string Schema { get; set; }
    }
}
