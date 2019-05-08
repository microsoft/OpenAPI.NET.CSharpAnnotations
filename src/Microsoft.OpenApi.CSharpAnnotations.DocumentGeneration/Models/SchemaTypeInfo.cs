// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models
{
    public class SchemaTypeInfo
    {
        /// <summary>
        /// Cref key to <see cref="InternalSchemaInfo"/> map.
        /// </summary>
        public Dictionary<string, SchemaInfo> CrefToSchemaMap { get; set; } =
            new Dictionary<string, SchemaInfo>();

        /// <summary>
        /// Cref key to field value map
        /// </summary>
        public Dictionary<string, string> CrefToFieldValueMap { get; set; } = new Dictionary<string, string>();


        /// <summary>
        /// Document Variant to Schema reference map (Schema key --> OpenApiSchema)
        /// </summary>
        public Dictionary<DocumentVariantInfo, IDictionary<string, OpenApiSchema>> VariantSchemaReferenceMap { get; set; } =
            new Dictionary<DocumentVariantInfo, IDictionary<string, OpenApiSchema>>();
    }
}
