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
    /// The class to store the open api document generation result with the document explicitly stored as string.
    /// This is needed to allow JsonConvert to serialize the entire object correctly given that
    /// <see cref="OpenApiDocument"/> cannot directly be serialized with JsonConvert.
    /// </summary>
    public class DocumentGenerationResultSerializedDocument
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DocumentGenerationResultSerializedDocument"/>.
        /// </summary>
        public DocumentGenerationResultSerializedDocument()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DocumentGenerationResultSerializedDocument"/>.
        /// </summary>
        /// <param name="pathGenerationResults">The path generation results.</param>
        public DocumentGenerationResultSerializedDocument(IList<OperationGenerationResult> pathGenerationResults)
        {
            foreach (var pathGenerationResult in pathGenerationResults)
            {
                PathGenerationResults.Add(new OperationGenerationResult(pathGenerationResult));
            }
        }

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
                if (PathGenerationResults.Any(i => i.GenerationStatus == GenerationStatus.Failure))
                {
                    return GenerationStatus.Failure;
                }

                if (PathGenerationResults.Any(i => i.GenerationStatus == GenerationStatus.Warning))
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
        /// List of path generations results.
        /// </summary>
        [JsonProperty]
        public IList<OperationGenerationResult> PathGenerationResults { get; internal set; } =
            new List<OperationGenerationResult>();

        /// <summary>
        /// Converts this object to <see cref="DocumentGenerationResult"/>.
        /// </summary>
        public DocumentGenerationResult ToDocumentGenerationResult()
        {
            var documentGenerationResult = new DocumentGenerationResult();

            foreach (var pathGenerationResult in PathGenerationResults)
            {
                documentGenerationResult.PathGenerationResults.Add(
                    new OperationGenerationResult(pathGenerationResult));
            }

            foreach (var variantInfoDocumentKeyValuePair in Documents)
            {
                documentGenerationResult.Documents[new DocumentVariantInfo(variantInfoDocumentKeyValuePair.Key)]
                    = new OpenApiStringReader().Read(variantInfoDocumentKeyValuePair.Value, out var _);
            }

            return documentGenerationResult;
        }
    }
}