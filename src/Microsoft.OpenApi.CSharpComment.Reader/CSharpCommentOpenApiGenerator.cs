// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.IO;
using Microsoft.OpenApi.CSharpComment.Reader.Extensions;
using Microsoft.OpenApi.CSharpComment.Reader.Models;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpComment.Reader
{
    /// <summary>
    /// The class that holds functionality to generate OpenAPI document from csharp documentation.
    /// </summary>
    public class CSharpCommentOpenApiGenerator : ICSharpCommentOpenApiGenerator
    {
        /// <summary>
        /// Generates an OpenAPI document based on the provided configuration, but ignores any variant configuration
        /// that may be present.
        /// </summary>
        /// <param name="cSharpCommentOpenApiGeneratorConfig">The configuration that will be used to generate
        /// the document.</param>
        /// <param name="generationDiagnostic">The generation diagnostics.</param>
        /// <returns>The generated OpenAPI document.</returns>
        public OpenApiDocument GenerateDocument(
            CSharpCommentOpenApiGeneratorConfig cSharpCommentOpenApiGeneratorConfig,
            out GenerationDiagnostic generationDiagnostic)
        {
            var documents = GenerateDocuments(cSharpCommentOpenApiGeneratorConfig, out generationDiagnostic);

            return documents?.Count == 0 ? null : documents[DocumentVariantInfo.Default];
        }

        /// <summary>
        /// Generates an OpenAPI document per variant specified in configuration.
        /// In addition, a "default" variant document is generated, which contains no alterations based on
        /// variant metadata.
        /// </summary>
        /// <param name="cSharpCommentOpenApiGeneratorConfig">The configuration that will be used to generate
        /// the document.</param>
        /// <param name="generationDiagnostic">The generation diagnostics.</param>
        /// <returns>Dictionary mapping document variant metadata to their respective OpenAPI document.</returns>
        public IDictionary<DocumentVariantInfo, OpenApiDocument> GenerateDocuments(
            CSharpCommentOpenApiGeneratorConfig cSharpCommentOpenApiGeneratorConfig,
            out GenerationDiagnostic generationDiagnostic)
        {
            var result = GenerateSerializedDocuments(
                cSharpCommentOpenApiGeneratorConfig,
                out generationDiagnostic);

            return result.ToOpenApiDocuments();
        }

        /// <summary>
        /// Generates a serialized OpenAPI document based on the provided configuration, but ignores any variant
        /// configuration that may be present.
        /// </summary>
        /// <param name="cSharpCommentOpenApiGeneratorConfig">The configuration that will be used to generate
        /// the document.</param>
        /// <param name="generationDiagnostic">The generation diagnostics.</param>
        /// <returns>The generated serialized OpenAPI document.</returns>
        public string GenerateSerializedDocument(
            CSharpCommentOpenApiGeneratorConfig cSharpCommentOpenApiGeneratorConfig,
            out GenerationDiagnostic generationDiagnostic)
        {
            var document = GenerateDocument(cSharpCommentOpenApiGeneratorConfig, out generationDiagnostic);

            return document?.Serialize(
                cSharpCommentOpenApiGeneratorConfig.OpenApiSpecificationVersion,
                cSharpCommentOpenApiGeneratorConfig.OpenApiFormat);
        }

        /// <summary>
        /// Generates a serialized OpenAPI document per variant specified in configuration.
        /// In addition, a serialized "default" variant document is generated, which contains no alterations based on
        /// variant metadata.
        /// </summary>
        /// <param name="cSharpCommentOpenApiGeneratorConfig">The configuration that will be used to generate
        /// the document.</param>
        /// <param name="generationDiagnostic">The generation diagnostics.</param>
        /// <returns>Dictionary mapping document variant metadata to their respective serialized OpenAPI document.
        /// </returns>
        public IDictionary<DocumentVariantInfo, string> GenerateSerializedDocuments(
            CSharpCommentOpenApiGeneratorConfig cSharpCommentOpenApiGeneratorConfig,
            out GenerationDiagnostic generationDiagnostic)
        {
            foreach (var assemblyPath in cSharpCommentOpenApiGeneratorConfig.AssemblyPaths)
            {
                if (!File.Exists(assemblyPath))
                {
                    throw new FileNotFoundException(assemblyPath);
                }
            }

            var internalOpenApiDocumentGenerator = new InternalOpenApiDocumentGenerator(
                cSharpCommentOpenApiGeneratorConfig.DocumentConfigFilters,
                cSharpCommentOpenApiGeneratorConfig.DocumentFilters,
                cSharpCommentOpenApiGeneratorConfig.OperationConfigFilters,
                cSharpCommentOpenApiGeneratorConfig.OperationFilters,
                cSharpCommentOpenApiGeneratorConfig.PreProcessingOperationFilters,
                cSharpCommentOpenApiGeneratorConfig.PostProcessingDocumentFilters);

            return internalOpenApiDocumentGenerator.GenerateOpenApiDocuments(
                cSharpCommentOpenApiGeneratorConfig.AnnotationXmlDocument.ToString(),
                cSharpCommentOpenApiGeneratorConfig.AssemblyPaths,
                cSharpCommentOpenApiGeneratorConfig.AdvancedConfigurationXmlDocument?.ToString(),
                cSharpCommentOpenApiGeneratorConfig.OpenApiSpecificationVersion,
                cSharpCommentOpenApiGeneratorConfig.OpenApiFormat,
                out generationDiagnostic);
        }
    }
}