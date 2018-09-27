// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.IO;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration
{
    /// <summary>
    /// The class that holds functionality to generate OpenAPI document from csharp documentation.
    /// </summary>
    public class OpenApiGenerator : IOpenApiGenerator
    {
        /// <summary>
        /// Generates an OpenAPI document based on the provided configuration, but ignores any variant configuration
        /// that may be present.
        /// </summary>
        /// <param name="openApiGeneratorConfig">The configuration that will be used to generate
        /// the document.</param>
        /// <param name="generationDiagnostic">The generation diagnostics.</param>
        /// <param name="openApiDocumentGenerationSettings">The optional Open API document generation settings.</param>
        /// <returns>The generated OpenAPI document.</returns>
        public OpenApiDocument GenerateDocument(
            OpenApiGeneratorConfig openApiGeneratorConfig,
            out GenerationDiagnostic generationDiagnostic,
            OpenApiDocumentGenerationSettings openApiDocumentGenerationSettings = null)
        {
            var documents = GenerateDocuments(
                openApiGeneratorConfig,
                out generationDiagnostic,
                openApiDocumentGenerationSettings);

            return documents?.Count == 0 ? null : documents[DocumentVariantInfo.Default];
        }

        /// <summary>
        /// Generates an OpenAPI document per variant specified in configuration.
        /// In addition, a "default" variant document is generated, which contains no alterations based on
        /// variant metadata.
        /// </summary>
        /// <param name="openApiGeneratorConfig">The configuration that will be used to generate
        /// the document.</param>
        /// <param name="generationDiagnostic">The generation diagnostics.</param>
        /// <param name="openApiDocumentGenerationSettings">The optional Open API document generation settings.</param>
        /// <returns>Dictionary mapping document variant metadata to their respective OpenAPI document.</returns>
        public IDictionary<DocumentVariantInfo, OpenApiDocument> GenerateDocuments(
            OpenApiGeneratorConfig openApiGeneratorConfig,
            out GenerationDiagnostic generationDiagnostic,
            OpenApiDocumentGenerationSettings openApiDocumentGenerationSettings = null)
        {
            foreach (var assemblyPath in openApiGeneratorConfig.AssemblyPaths)
            {
                if (!File.Exists(assemblyPath))
                {
                    throw new FileNotFoundException(assemblyPath);
                }
            }

            if (openApiDocumentGenerationSettings == null)
            {
                openApiDocumentGenerationSettings = new OpenApiDocumentGenerationSettings(
                    new SchemaGenerationSettings(new DefaultPropertyNameResolver()));
            }

            var internalOpenApiGenerator = new InternalOpenApiGenerator(
                openApiGeneratorConfig.OpenApiGeneratorFilterConfig,
                openApiDocumentGenerationSettings);

            return internalOpenApiGenerator.GenerateOpenApiDocuments(
                openApiGeneratorConfig.AnnotationXmlDocuments,
                openApiGeneratorConfig.AssemblyPaths,
                openApiGeneratorConfig.AdvancedConfigurationXmlDocument?.ToString(),
                openApiGeneratorConfig.OpenApiDocumentVersion,
                openApiGeneratorConfig.OpenApiInfoDescription,
                out generationDiagnostic);
        }
    }
}