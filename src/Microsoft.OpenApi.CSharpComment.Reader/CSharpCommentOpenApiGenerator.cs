// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.IO;
using Microsoft.OpenApi.CSharpComment.Reader.Extensions;
using Microsoft.OpenApi.CSharpComment.Reader.Models;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Microsoft.OpenApi.CSharpComment.Reader
{
    /// <summary>
    /// The class that holds functionality to generate OpenAPI document from csharp documentation.
    /// </summary>
    public class CSharpCommentOpenApiGenerator : ICSharpCommentOpenApiGenerator
    {
        /// <summary>
        /// Generates OpenAPI document using the provided input.
        /// </summary>
        /// <param name="openApiDocumentGeneratorInput">Required input to generate the document.</param>
        /// <param name="overallGenerationResult">The overall generation result.</param>
        /// <returns>The generated open api document</returns>
        public OpenApiDocument Read(
            CSharpCommentOpenApiGeneratorInput openApiDocumentGeneratorInput,
            out OverallGenerationResult overallGenerationResult)
        {
            var documents = Generate(openApiDocumentGeneratorInput, out overallGenerationResult);

            return documents?.Count == 0 ? null : documents[DocumentVariantInfo.Default];
        }

        /// <summary>
        /// Generates Open API document(s) using the provided input.
        /// </summary>
        /// <param name="openApiDocumentGeneratorInput">Required input to generate the document.</param>
        /// <param name="overallGenerationResult">The overall generation result.</param>
        /// <returns>The generated open api documents.</returns>
        public IDictionary<DocumentVariantInfo, OpenApiDocument> Generate(
            CSharpCommentOpenApiGeneratorInput openApiDocumentGeneratorInput,
            out OverallGenerationResult overallGenerationResult)
        {
            var result = GenerateSerialized(
                openApiDocumentGeneratorInput,
                out overallGenerationResult);

            return result.ToOpenApiDocuments();
        }

        /// <summary>
        /// Generates OpenAPI documents using the provided input.
        /// </summary>
        /// <param name="openApiDocumentGeneratorInput">The XDocument representing the annotation xml.</param>
        /// <param name="overallGenerationResult">The overall generation result.</param>
        /// <returns>The serialized open api documents.</returns>
        public IDictionary<DocumentVariantInfo, string> GenerateSerialized(
            CSharpCommentOpenApiGeneratorInput openApiDocumentGeneratorInput,
            out OverallGenerationResult overallGenerationResult)
        {
            foreach (var contractAssemblyPath in openApiDocumentGeneratorInput.ContractAssemblyPaths)
            {
                if (!File.Exists(contractAssemblyPath))
                {
                    throw new FileNotFoundException(contractAssemblyPath);
                }
            }

            using (var isolatedDomain = new AppDomainCreator<InternalOpenApiDocumentGenerator>())
            {
                string serializedOverallGenerationResult;

                var documents = isolatedDomain.Object.GenerateOpenApiDocuments(
                    openApiDocumentGeneratorInput.AnnotationXmlDocument.ToString(),
                    openApiDocumentGeneratorInput.ContractAssemblyPaths,
                    openApiDocumentGeneratorInput.ConfigurationXmlDocument?.ToString(),
                    openApiDocumentGeneratorInput.OpenApiSpecVersion,
                    openApiDocumentGeneratorInput.OpenApiFormat,
                    out serializedOverallGenerationResult);

                overallGenerationResult =
                    JsonConvert.DeserializeObject<OverallGenerationResult>(serializedOverallGenerationResult);

                return JsonConvert.DeserializeObject<Dictionary<DocumentVariantInfo, string>>(
                        documents,
                        new DictionaryJsonConverter<DocumentVariantInfo, string>());
            }
        }
    }
}