// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Newtonsoft.Json;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models
{
    /// <summary>
    /// Holds various map b/w cref and schema.
    /// </summary>
    internal class InternalSchemaTypeInfo
    {
        OpenApiStringReader openApiStringReader = new OpenApiStringReader();

        /// <summary>
        /// Cref key to <see cref="InternalSchemaInfo"/> map.
        /// </summary>
        public Dictionary<string, InternalSchemaInfo> CrefToSchemaMap { get; set; } =
            new Dictionary<string, InternalSchemaInfo>();

        /// <summary>
        /// Cref key to field value map
        /// </summary>
        public Dictionary<string, string> CrefToFieldValueMap { get; set; } = new Dictionary<string, string>();


        /// <summary>
        /// Document Variant to Schema reference map (Schema key --> OpenApiSchema)
        /// </summary>
        public Dictionary<string, IDictionary<string, string>> VariantSchemaReferenceMap { get; set; } =
            new Dictionary<string, IDictionary<string, string>>();

        public SchemaTypeInfo ToSchemaTypeInfo()
        {
            var schemaTypeInfo = new SchemaTypeInfo();

            schemaTypeInfo.CrefToFieldValueMap = this.CrefToFieldValueMap;

            foreach(var key in this.CrefToSchemaMap.Keys)
            {
                InternalSchemaInfo schemaInfo = this.CrefToSchemaMap[key];
                var internalSchemaInfo = new SchemaInfo
                {
                    schema = openApiStringReader.ReadFragment<OpenApiSchema>(
                            schemaInfo.schema,
                            OpenApiSpecVersion.OpenApi3_0,
                            out OpenApiDiagnostic diagnostic),
                    error = schemaInfo.error
                };

                schemaTypeInfo.CrefToSchemaMap.Add(key, internalSchemaInfo);
            }

            foreach(var key in this.VariantSchemaReferenceMap.Keys)
            {
                var documentVariantInfo = JsonConvert.DeserializeObject<DocumentVariantInfo>(key);
                Dictionary<string, OpenApiSchema> schemaCrefMap = this.VariantSchemaReferenceMap[key].ToDictionary(
                    k => k.Key, k => openApiStringReader.ReadFragment<OpenApiSchema>(
                            k.Value,
                            OpenApiSpecVersion.OpenApi3_0,
                            out OpenApiDiagnostic diagnostic));

                schemaTypeInfo.VariantSchemaReferenceMap.Add(documentVariantInfo, schemaCrefMap);
            }

            return schemaTypeInfo;
        }
    }
}