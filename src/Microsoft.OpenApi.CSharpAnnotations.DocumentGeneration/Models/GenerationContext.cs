// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models
{
    /// <summary>
    /// Holds various required map b/w cref and schema to perform Open API spec generation.
    /// </summary>
    public class GenerationContext
    {
        /// <summary>
        /// Cref key to <see cref="InternalSchemaGenerationInfo"/> map.
        /// </summary>
        public Dictionary<string, SchemaGenerationInfo> CrefToSchemaMap { get; set; } =
            new Dictionary<string, SchemaGenerationInfo>();

        /// <summary>
        /// Cref key to <see cref="FieldValueInfo"/> map.
        /// </summary>
        public Dictionary<string, FieldValueInfo> CrefToFieldValueMap { get; set; }
            = new Dictionary<string, FieldValueInfo>();

        /// <summary>
        /// Document Variant to Schema reference map (Schema key --> OpenApiSchema).
        /// </summary>
        public Dictionary<DocumentVariantInfo, IDictionary<string, OpenApiSchema>> VariantSchemaReferenceMap { get; set; } =
            new Dictionary<DocumentVariantInfo, IDictionary<string, OpenApiSchema>>();
    }
}
