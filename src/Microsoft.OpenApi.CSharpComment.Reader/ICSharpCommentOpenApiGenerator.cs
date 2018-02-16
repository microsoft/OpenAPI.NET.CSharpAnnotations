// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.CSharpComment.Reader.Models;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Interface;

namespace Microsoft.OpenApi.CSharpComment.Reader
{
    /// <summary>
    /// The contract for Open API document generator from csharp documentation.
    /// </summary>
    public interface ICSharpCommentOpenApiGenerator : IOpenApiReader<CSharpCommentOpenApiGeneratorInput,
        OverallGenerationResult>
    {
        /// <summary>
        /// Generates Open API document(s) using the provided input.
        /// </summary>
        /// <param name="openApiDocumentGeneratorInput">The required input to generated the document.</param>
        /// <param name="overallGenerationResult">The overall generation result.</param>
        /// <returns>The generated open api document.</returns>
        IDictionary<DocumentVariantInfo, OpenApiDocument> Generate(
            CSharpCommentOpenApiGeneratorInput openApiDocumentGeneratorInput,
            out OverallGenerationResult overallGenerationResult);

        /// <summary>
        /// Generates OpenAPI documents using the provided input.
        /// </summary>
        /// <param name="openApiDocumentGeneratorInput">The required input to generate the document.</param>
        /// <param name="overallGenerationResult">The overall generation result.</param>
        /// <returns>The serialized open api documents.</returns>
        IDictionary<DocumentVariantInfo, string> GenerateSerialized(
            CSharpCommentOpenApiGeneratorInput openApiDocumentGeneratorInput,
            out OverallGenerationResult overallGenerationResult);
    }
}