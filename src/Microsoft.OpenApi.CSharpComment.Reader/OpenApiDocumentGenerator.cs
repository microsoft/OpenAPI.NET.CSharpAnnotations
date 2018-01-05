// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Microsoft.OpenApi.CSharpComment.Reader.Models;
using Newtonsoft.Json;

namespace Microsoft.OpenApi.CSharpComment.Reader
{
    /// <summary>
    /// The class that holds functionality to generate OpenAPI document.
    /// </summary>
    public class OpenApiDocumentGenerator : IOpenApiDocumentGenerator
    {
        /// <summary>
        /// Generates OpenAPI documents using the provided xdocument and contract assemblies.
        /// </summary>
        /// <param name="annotationXmlDocument">The XDocument representing the annotation xml.</param>
        /// <param name="contractAssemblyPaths">The list of relative or absolute paths to the contract assemblies.</param>
        /// <param name="configurationXmlDocument">The XDocument representing the generation configuration.</param>
        /// <param name="openApiSpecVersion">Specification version of the OpenAPI document to generate.</param>
        /// <returns>The overall generation result.</returns>
        public OverallGenerationResult GenerateOpenApiDocuments(
            XDocument annotationXmlDocument,
            IList<string> contractAssemblyPaths,
            XDocument configurationXmlDocument,
            OpenApiSpecVersion openApiSpecVersion)
        {
            return GenerateSerializedOpenApiDocuments(
                    annotationXmlDocument,
                    contractAssemblyPaths,
                    configurationXmlDocument,
                    openApiSpecVersion,
                    // The format choice here is arbitrary. The document returned from this method
                    // would be serialized and deserialized back to a .NET object anyway.
                    OpenApiFormat.Json)
                .ToOverallGenerationResult();
        }

        /// <summary>
        /// Generates OpenAPI documents using the provided xdocument and contract assemblies.
        /// </summary>
        /// <param name="annotationXmlDocument">The XDocument representing the annotation xml.</param>
        /// <param name="contractAssemblyPaths">The list of relative or absolute paths to the contract assemblies.</param>
        /// <param name="openApiSpecVersion">Specification version of the OpenAPI document to generate.</param>
        /// <returns>The overall generation result.</returns>
        public OverallGenerationResult GenerateOpenApiDocuments(
            XDocument annotationXmlDocument,
            IList<string> contractAssemblyPaths,
            OpenApiSpecVersion openApiSpecVersion)
        {
            return GenerateOpenApiDocuments(
                annotationXmlDocument,
                contractAssemblyPaths,
                configurationXmlDocument: null,
                openApiSpecVersion: openApiSpecVersion);
        }

        /// <summary>
        /// Generates OpenAPI documents using the provided visual studio summary comment and contract assemblies.
        /// </summary>
        /// <param name="summaryComment">The visual studio summary comment.</param>
        /// <param name="contractAssemblyPaths">The list of relative or absolute paths to the contract assemblies.</param>
        /// <param name="openApiSpecVersion">Specification version of the OpenAPI document to generate.</param>
        /// <returns>The overall generation result.</returns>
        public OverallGenerationResult GenerateOpenApiDocuments(
            string summaryComment,
            IList<string> contractAssemblyPaths,
            OpenApiSpecVersion openApiSpecVersion)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates OpenAPI documents using the provided xdocument and contract assemblies.
        /// </summary>
        /// <param name="annotationXmlDocument">The XDocument representing the annotation xml.</param>
        /// <param name="contractAssemblyPaths">The list of relative or absolute paths to the contract assemblies.</param>
        /// <param name="configurationXmlDocument">The XDocument representing the generation configuration.</param>
        /// <param name="openApiSpecVersion">Specification version of the OpenAPI document to generate.</param>
        /// <param name="openApiFormat">Format (YAML or JSON) of the OpenAPI document to generate.</param>
        /// <returns>The overall generation result with serialized documents.</returns>
        public SerializedOverallGenerationResult GenerateSerializedOpenApiDocuments(
            XDocument annotationXmlDocument,
            IList<string> contractAssemblyPaths,
            XDocument configurationXmlDocument,
            OpenApiSpecVersion openApiSpecVersion,
            OpenApiFormat openApiFormat)
        {
            foreach (var contractAssemblyPath in contractAssemblyPaths)
            {
                if (!File.Exists(contractAssemblyPath))
                {
                    throw new FileNotFoundException(contractAssemblyPath);
                }
            }

            using (var isolatedDomain = new AppDomainCreator<InternalOpenApiDocumentGenerator>())
            {
                var result = isolatedDomain.Object.GenerateOpenApiDocuments(
                    annotationXmlDocument.ToString(),
                    contractAssemblyPaths,
                    configurationXmlDocument?.ToString(),
                    openApiSpecVersion,
                    openApiFormat);

                return JsonConvert.DeserializeObject<SerializedOverallGenerationResult>(result);
            }
        }

        /// <summary>
        /// Generates OpenAPI documents using the provided xdocument and contract assemblies.
        /// </summary>
        /// <param name="annotationXmlDocument">The XDocument representing the annotation xml.</param>
        /// <param name="contractAssemblyPaths">The list of relative or absolute paths to the contract assemblies.</param>
        /// <param name="openApiSpecVersion">Specification version of the OpenAPI document to generate.</param>
        /// <param name="openApiFormat">Format (YAML or JSON) of the OpenAPI document to generate.</param>
        /// <returns>The overall generation result with serialized documents.</returns>
        public SerializedOverallGenerationResult GenerateSerializedOpenApiDocuments(
            XDocument annotationXmlDocument,
            IList<string> contractAssemblyPaths,
            OpenApiSpecVersion openApiSpecVersion,
            OpenApiFormat openApiFormat)
        {
            return GenerateSerializedOpenApiDocuments(
                annotationXmlDocument,
                contractAssemblyPaths,
                configurationXmlDocument: null,
                openApiSpecVersion: openApiSpecVersion,
                openApiFormat: openApiFormat);
        }

        /// <summary>
        /// Generates OpenAPI documents using the provided visual studio summary comment and contract assemblies.
        /// </summary>
        /// <param name="summaryComment">The visual studio summary comment.</param>
        /// <param name="contractAssemblyPaths">The list of relative or absolute paths to the contract assemblies.</param>
        /// <param name="openApiSpecVersion">Specification version of the OpenAPI document to generate.</param>
        /// <param name="openApiFormat">Format (YAML or JSON) of the OpenAPI document to generate.</param>
        /// <returns>The overall generation result with serialized documents.</returns>
        public SerializedOverallGenerationResult GenerateSerializedOpenApiDocuments(
            string summaryComment,
            IList<string> contractAssemblyPaths,
            OpenApiSpecVersion openApiSpecVersion,
            OpenApiFormat openApiFormat)
        {
            throw new NotImplementedException();
        }
    }
}