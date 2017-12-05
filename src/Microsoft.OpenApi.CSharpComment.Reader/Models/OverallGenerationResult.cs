// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Microsoft.OpenApi.CSharpComment.Reader.Models
{
    /// <summary>
    /// The class to store the overall open api document generation result.
    /// </summary>
    public class OverallGenerationResult
    {
        /// <summary>
        /// Dictionary mapping a document variant information to its associated specification document.
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(DictionaryJsonConverter<DocumentVariantInfo, OpenApiDocument>))]
        public IDictionary<DocumentVariantInfo, OpenApiDocument>
            Documents { get; internal set; } = new Dictionary<DocumentVariantInfo, OpenApiDocument>();

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
        public OpenApiDocument MainDocument
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
        /// Converts this object to <see cref="OverallGenerationResultSerializedDocument"/>.
        /// </summary>
        public OverallGenerationResultSerializedDocument ToDocumentGenerationResultSerializedDocument(
            OpenApiSpecVersion openApiSpecVersion)
        {
            var generationResult = new OverallGenerationResultSerializedDocument();

            foreach (var operationGenerationResult in OperationGenerationResults)
            {
                generationResult.OperationGenerationResults.Add(
                    new OperationGenerationResult(operationGenerationResult));
            }

            foreach (var documentGenerationResult in DocumentGenerationResults)
            {
                generationResult.DocumentGenerationResults.Add(
                    new DocumentGenerationResult(documentGenerationResult));
            }

            foreach (var variantInfoDocumentKeyValuePair in Documents)
            {
                generationResult.Documents[new DocumentVariantInfo(variantInfoDocumentKeyValuePair.Key)]
                    = variantInfoDocumentKeyValuePair.Value.SerializeAsJson(openApiSpecVersion);
            }

            return generationResult;
        }
    }
}