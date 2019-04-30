// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models
{
    /// <summary>
    /// Holds various map b/w cref and schema.
    /// </summary>
    public class SchemaTypeInfo
    {
        /// <summary>
        /// Cref to serialized <see cref="SchemaInfo"/> map.
        /// </summary>
        public Dictionary<string, string> CrefToSchemaMap { get; set; } =
            new Dictionary<string, string>();

        /// <summary>
        /// Cref to field value map
        /// </summary>
        public Dictionary<string, string> CrefToFieldValueMap { get; set; } = new Dictionary<string, string>();


        /// <summary>
        /// Document Variant to Schema reference map
        /// </summary>
        public Dictionary<string, IDictionary<string, string>> VariantSchemaReferenceMap { get; set; } =
            new Dictionary<string, IDictionary<string, string>>();
    }
}