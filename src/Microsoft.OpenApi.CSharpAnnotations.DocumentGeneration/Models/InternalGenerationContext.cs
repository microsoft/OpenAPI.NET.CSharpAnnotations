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
    /// Holds various required map b/w cref and schema to perform Open API spec generation.
    /// This class will be used to transfer data across app domain.
    /// </summary>
    internal class InternalGenerationContext
    {
        OpenApiStringReader openApiStringReader = new OpenApiStringReader();

        /// <summary>
        /// Cref key to <see cref="InternalSchemaGenerationInfo"/> map.
        /// </summary>
        public Dictionary<string, InternalSchemaGenerationInfo> CrefToSchemaMap { get; set; } =
            new Dictionary<string, InternalSchemaGenerationInfo>();

        /// <summary>
        /// Cref key to <see cref="FieldValueInfo"/> map.
        /// </summary>
        public Dictionary<string, FieldValueInfo> CrefToFieldValueMap { get; set; }
            = new Dictionary<string, FieldValueInfo>();


        /// <summary>
        /// Serialized <see cref="DocumentVariantInfo"/> to Schema reference map (Schema key --> Serialized OpenApiSchema).
        /// </summary>
        public Dictionary<string, IDictionary<string, string>> VariantSchemaReferenceMap { get; set; } =
            new Dictionary<string, IDictionary<string, string>>();

        /// <summary>
        /// Converts to <see cref="GenerationContext"/>.
        /// </summary>
        /// <returns><see cref="GenerationContext"/></returns>
        public GenerationContext ToGenerationContext()
        {
            var generationContext = new GenerationContext
            {
                CrefToFieldValueMap = this.CrefToFieldValueMap
            };

            foreach (var key in this.CrefToSchemaMap.Keys)
            {
                InternalSchemaGenerationInfo schemaInfo = this.CrefToSchemaMap[key];
                var internalSchemaInfo = new SchemaGenerationInfo
                {
                    Schema = schemaInfo.Schema == null 
                    ? null
                    : openApiStringReader.ReadFragment<OpenApiSchema>(
                            schemaInfo.Schema,
                            OpenApiSpecVersion.OpenApi3_0,
                            out OpenApiDiagnostic diagnostic),
                    Error = schemaInfo.Error
                };

                generationContext.CrefToSchemaMap.Add(key, internalSchemaInfo);
            }

            foreach(var key in this.VariantSchemaReferenceMap.Keys)
            {
                var documentVariantInfo = JsonConvert.DeserializeObject<DocumentVariantInfo>(key);
                Dictionary<string, OpenApiSchema> schemaCrefMap = this.VariantSchemaReferenceMap[key].ToDictionary(
                    k => k.Key, k => openApiStringReader.ReadFragment<OpenApiSchema>(
                        k.Value,
                        OpenApiSpecVersion.OpenApi3_0,
                        out OpenApiDiagnostic diagnostic));

                generationContext.VariantSchemaReferenceMap.Add(documentVariantInfo, schemaCrefMap);
            }

            return generationContext;
        }
    }
}