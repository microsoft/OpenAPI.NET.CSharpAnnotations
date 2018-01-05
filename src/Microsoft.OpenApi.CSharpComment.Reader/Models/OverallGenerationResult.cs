// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Microsoft.OpenApi.CSharpComment.Reader.Models
{
    /// <summary>
    /// The class to store the overall generation result.
    /// </summary>
    public class OverallGenerationResult : OverallGenerationResultBase<OpenApiDocument>
    {
        /// <summary>
        /// Dictionary mapping a document variant information to its associated specification document.
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(DictionaryJsonConverter<DocumentVariantInfo, OpenApiDocument>))]
        public override IDictionary<DocumentVariantInfo, OpenApiDocument>
            Documents { get; internal set; } = new Dictionary<DocumentVariantInfo, OpenApiDocument>();

        /// <summary>
        /// Converts this object to <see cref="OverallGenerationResultSerializedDocument"/>.
        /// </summary>
        public OverallGenerationResultSerializedDocument ToOverallGenerationResultSerializedDocument(
            OpenApiSpecVersion openApiSpecVersion,
            OpenApiFormat openApiFormat)
        {
            return ToOverallGenerationResult<OverallGenerationResultSerializedDocument, string>(
                document => document.Serialize(openApiSpecVersion, openApiFormat));
        }
    }
}