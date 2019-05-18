// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models
{
    /// <summary>
    /// Gets or sets the generated schema info.
    /// </summary>
    public class SchemaGenerationInfo
    {
        /// <summary>
        /// Gets or sets the schema.
        /// </summary>
        public OpenApiSchema Schema { get; set; }

        /// <summary>
        /// Gets or sets the error encountered while generating schema.
        /// </summary>
        public GenerationError Error { get; set; }
    }
}
