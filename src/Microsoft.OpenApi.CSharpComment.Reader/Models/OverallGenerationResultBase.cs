// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Microsoft.OpenApi.CSharpComment.Reader.Models
{
    /// <summary>
    /// The class to store the overall generation result.
    /// </summary>
    public abstract class OverallGenerationResultBase<TDocument>
    {
        /// <summary>
        /// Dictionary mapping a document variant information to its associated specification document.
        /// </summary>
        [JsonProperty]
        public abstract IDictionary<DocumentVariantInfo, TDocument>
            Documents { get; internal set; }

        /// <summary>
        /// The generation status.
        /// </summary>
        [JsonIgnore]
        public GenerationStatus GenerationStatus
        {
            get
            {
                if (OperationGenerationResults.Any(i => i.GenerationStatus == GenerationStatus.Failure) ||
                    DocumentGenerationResult.GenerationStatus == GenerationStatus.Failure)
                {
                    return GenerationStatus.Failure;
                }

                if (OperationGenerationResults.Any(i => i.GenerationStatus == GenerationStatus.Warning) ||
                    DocumentGenerationResult.GenerationStatus == GenerationStatus.Warning)
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
        public TDocument MainDocument
        {
            get
            {
                if (Documents.ContainsKey(DocumentVariantInfo.Default))
                {
                    return Documents[DocumentVariantInfo.Default];
                }

                return default(TDocument);
            }
        }

        /// <summary>
        /// List of operation-level generation results.
        /// </summary>
        [JsonProperty]
        public IList<OperationGenerationResult> OperationGenerationResults { get; internal set; } =
            new List<OperationGenerationResult>();

        /// <summary>
        /// The document-level generation result (e.g. from applying document-level filters)
        /// </summary>
        [JsonProperty]
        public DocumentGenerationResult DocumentGenerationResult { get; set; }

        /// <summary>
        /// Converts this object to some other type of <see cref="OverallGenerationResultBase{TOutputDocument}"/>
        /// </summary>
        protected TOverallGenerationResult ToOverallGenerationResult<TOverallGenerationResult, TOutputDocument>(
            Func<TDocument, TOutputDocument> convertFunc)
            where TOverallGenerationResult : OverallGenerationResultBase<TOutputDocument>, new()
        {
            var generationResult = new TOverallGenerationResult();

            foreach (var operationGenerationResult in OperationGenerationResults)
            {
                generationResult.OperationGenerationResults.Add(
                    new OperationGenerationResult(operationGenerationResult));
            }

            generationResult.DocumentGenerationResult = DocumentGenerationResult;

            foreach (var variantInfoDocumentKeyValuePair in Documents)
            {
                generationResult.Documents[new DocumentVariantInfo(variantInfoDocumentKeyValuePair.Key)]
                    = convertFunc(variantInfoDocumentKeyValuePair.Value);
            }

            return generationResult;
        }
    }
}