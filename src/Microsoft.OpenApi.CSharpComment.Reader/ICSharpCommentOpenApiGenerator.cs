// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.CSharpComment.Reader.Models;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpComment.Reader
{
    /// <summary>
    /// The contract for OpenAPI document generator from csharp documentation.
    /// </summary>
    public interface ICSharpCommentOpenApiGenerator
    {
        /// <summary>
        /// Generates an OpenAPI document based on the provided configuration, but ignores any variant configuration
        /// that may be present.
        /// </summary>
        /// <param name="cSharpCommentOpenApiGeneratorConfig">The configuration that will be used to generate
        /// the document.</param>
        /// <param name="generationDiagnostic">The generation diagnostics.</param>
        /// <returns>The generated OpenAPI document.</returns>
        OpenApiDocument GenerateDocument(
            CSharpCommentOpenApiGeneratorConfig cSharpCommentOpenApiGeneratorConfig,
            out GenerationDiagnostic generationDiagnostic);

        /// <summary>
        /// Generates an OpenAPI document per variant specified in configuration.
        /// In addition, a "default" variant document is generated, which contains no alterations based on
        /// variant metadata.
        /// </summary>
        /// <param name="cSharpCommentOpenApiGeneratorConfig">The configuration that will be used to generate
        /// the document.</param>
        /// <param name="generationDiagnostic">The generation diagnostics.</param>
        /// <returns>Dictionary mapping document variant metadata to their respective OpenAPI document.</returns>
        IDictionary<DocumentVariantInfo, OpenApiDocument> GenerateDocuments(
            CSharpCommentOpenApiGeneratorConfig cSharpCommentOpenApiGeneratorConfig,
            out GenerationDiagnostic generationDiagnostic);
    }
}