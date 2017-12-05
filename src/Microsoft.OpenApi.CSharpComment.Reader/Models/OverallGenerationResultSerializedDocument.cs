// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Newtonsoft.Json;

namespace Microsoft.OpenApi.CSharpComment.Reader.Models
{
    /// <summary>
    /// The class to store the overall open api document generation result with the document explicitly stored as string.
    /// This is needed to allow JsonConvert to serialize the entire object correctly given that
    /// <see cref="OpenApiDocument"/> cannot directly be serialized with JsonConvert.
    /// </summary>
    public class OverallGenerationResultSerializedDocument
    {
        /// <summary>
        /// Dictionary mapping a document variant information to its associated specification document.
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(DictionaryJsonConverter<DocumentVariantInfo, string>))]
        public IDictionary<DocumentVariantInfo, string>
            Documents { get; internal set; } = new Dictionary<DocumentVariantInfo, string>();

        /// <summary>
        /// The generation status.
        /// </summary>
        [JsonIgnore]
        public GenerationStatus GenerationStatus
        {
            get
            {
                if (OperationGenerationResults.Any(i => i.GenerationStatus == GenerationStatus.Failure) ||
                    DocumentGenerationResults.Any(i => i.GenerationStatus == GenerationStatus.Failure))
                {
                    return GenerationStatus.Failure;
                }

                if (OperationGenerationResults.Any(i => i.GenerationStatus == GenerationStatus.Warning) ||
                    DocumentGenerationResults.Any(i => i.GenerationStatus == GenerationStatus.Warning))
                {
                    return GenerationStatus.Warning;
                }

                return GenerationStatus.Success;
            }
        }

        /// <summary>
        /// Gets the document generated from the entire documentation regardless of document variant info.
        /// </summary>
        [JsonIgnore]
        public string MainDocument
        {
            get
            {
                if (Documents.ContainsKey(DocumentVariantInfo.Default))
                {
                    return Documents[DocumentVariantInfo.Default];
                }

                return null;
            }
        }

        /// <summary>
        /// List of operation-level generations results.
        /// </summary>
        [JsonProperty]
        public IList<OperationGenerationResult> OperationGenerationResults { get; internal set; } =
            new List<OperationGenerationResult>();

        /// <summary>
        /// List of document-level generations results.
        /// </summary>
        [JsonProperty]
        public IList<DocumentGenerationResult> DocumentGenerationResults { get; internal set; } =
            new List<DocumentGenerationResult>();

        /// <summary>
        /// Converts this object to <see cref="OverallGenerationResult"/>.
        /// </summary>
        public OverallGenerationResult ToDocumentGenerationResult()
        {
            var generationResult = new OverallGenerationResult();

            foreach (var pathGenerationResult in OperationGenerationResults)
            {
                generationResult.OperationGenerationResults.Add(
                    new OperationGenerationResult(pathGenerationResult));
            }

            foreach (var documentGenerationResult in DocumentGenerationResults)
            {
                generationResult.DocumentGenerationResults.Add(
                    new DocumentGenerationResult(documentGenerationResult));
            }

            foreach (var variantInfoDocumentKeyValuePair in Documents)
            {
                generationResult.Documents[new DocumentVariantInfo(variantInfoDocumentKeyValuePair.Key)]
                    = new OpenApiStringReader().Read(variantInfoDocumentKeyValuePair.Value, out var _);
            }

            return generationResult;
        }
    }
}