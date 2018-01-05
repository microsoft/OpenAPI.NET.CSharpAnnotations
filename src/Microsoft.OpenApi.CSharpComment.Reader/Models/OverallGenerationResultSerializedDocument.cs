// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Newtonsoft.Json;

namespace Microsoft.OpenApi.CSharpComment.Reader.Models
{
    /// <summary>
    /// The class to store the overall generation result with the document explicitly stored as string.
    /// This is needed to allow JsonConvert to serialize the entire object correctly given that
    /// <see cref="OpenApiDocument"/> cannot be serialized directly with JsonConvert.
    /// </summary>
    public class OverallGenerationResultSerializedDocument : OverallGenerationResultBase<string>
    {
        /// <summary>
        /// Dictionary mapping a document variant information to its associated specification document.
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(DictionaryJsonConverter<DocumentVariantInfo, string>))]
        public override IDictionary<DocumentVariantInfo, string>
            Documents { get; internal set; } = new Dictionary<DocumentVariantInfo, string>();

        /// <summary>
        /// Converts this object to <see cref="OverallGenerationResult"/>.
        /// </summary>
        public OverallGenerationResult ToOverallGenerationResult()
        {
            return ToOverallGenerationResult<OverallGenerationResult, OpenApiDocument>(
                document => new OpenApiStringReader().Read(document, out var _));
        }
    }
}