// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApiSpecification.Core.Models;
using Newtonsoft.Json;

namespace Microsoft.OpenApiSpecification.Generation.Models
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
        /// <param name="pathGenerationResults">The path generation results.</param>
        public DocumentGenerationResult(IList<PathGenerationResult> pathGenerationResults)
        {
            foreach (var pathGenerationResult in pathGenerationResults)
            {
                PathGenerationResults.Add(new PathGenerationResult(pathGenerationResult));
            }
        }

        /// <summary>
        /// Dictionary mapping a document variant information to its associated specification document.
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(DictionaryJsonConverter<DocumentVariantInfo, OpenApiV3SpecificationDocument>))]
        public IDictionary<DocumentVariantInfo, OpenApiV3SpecificationDocument>
            Documents { get; internal set; }
            = new Dictionary<DocumentVariantInfo, OpenApiV3SpecificationDocument>();

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
        public OpenApiV3SpecificationDocument MainDocument
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
        public IList<PathGenerationResult> PathGenerationResults { get; internal set; } =
            new List<PathGenerationResult>();
    }
}