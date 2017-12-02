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
    /// The class to store the open api document generation result.
    /// </summary>
    public class DocumentGenerationResult
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DocumentGenerationResult"/>.
        /// </summary>
        public DocumentGenerationResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DocumentGenerationResult"/>.
        /// </summary>
        /// <param name="operationGenerationResults">The operation generation results.</param>
        public DocumentGenerationResult(IList<OperationGenerationResult> operationGenerationResults)
        {
            foreach (var pathGenerationResult in operationGenerationResults)
            {
                OperationGenerationResults.Add(new OperationGenerationResult(pathGenerationResult));
            }
        }

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
                if (OperationGenerationResults.Any(i => i.GenerationStatus == GenerationStatus.Failure))
                {
                    return GenerationStatus.Failure;
                }

                if (OperationGenerationResults.Any(i => i.GenerationStatus == GenerationStatus.Warning))
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
        /// List of operation generations results.
        /// </summary>
        [JsonProperty]
        public IList<OperationGenerationResult> OperationGenerationResults { get; internal set; } =
            new List<OperationGenerationResult>();

        /// <summary>
        /// Converts this object to <see cref="DocumentGenerationResultSerializedDocument"/>.
        /// </summary>
        public DocumentGenerationResultSerializedDocument ToDocumentGenerationResultSerializedDocument(
            OpenApiSpecVersion openApiSpecVersion)
        {
            var documentGenerationResult = new DocumentGenerationResultSerializedDocument();

            foreach (var pathGenerationResult in OperationGenerationResults)
            {
                documentGenerationResult.OperationGenerationResults.Add(
                    new OperationGenerationResult(pathGenerationResult));
            }

            foreach (var variantInfoDocumentKeyValuePair in Documents)
            {
                documentGenerationResult.Documents[new DocumentVariantInfo(variantInfoDocumentKeyValuePair.Key)]
                    = variantInfoDocumentKeyValuePair.Value.SerializeAsJson(openApiSpecVersion);
            }

            return documentGenerationResult;
        }
    }
}