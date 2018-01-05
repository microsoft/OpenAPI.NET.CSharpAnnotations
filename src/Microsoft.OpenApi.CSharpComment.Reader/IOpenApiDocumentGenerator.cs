// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.OpenApi.CSharpComment.Reader.Models;

namespace Microsoft.OpenApi.CSharpComment.Reader
{
    /// <summary>
    /// The contract for Open API document generator.
    /// </summary>
    public interface IOpenApiDocumentGenerator
    {
        /// <summary>
        /// Generates OpenAPI documents using the provided XDocument and contract assemblies.
        /// </summary>
        /// <param name="annotationXmlDocument">The XDocument representing the annotation xml.</param>
        /// <param name="contractAssemblyPaths">The list of relative or absolute paths to the contract assemblies.</param>
        /// <param name="openApiSpecVersion">Specification version of the OpenAPI document to generate.</param>
        /// <returns>The overall generation result.</returns>
        OverallGenerationResult GenerateOpenApiDocuments(
            XDocument annotationXmlDocument,
            IList<string> contractAssemblyPaths,
            OpenApiSpecVersion openApiSpecVersion);

        /// <summary>
        /// Generates OpenAPI documents using the provided XDocument and contract assemblies.
        /// </summary>
        /// <param name="annotationXmlDocument">The XDocument representing the annotation xml.</param>
        /// <param name="contractAssemblyPaths">The list of relative or absolute paths to the contract assemblies.</param>
        /// <param name="openApiSpecVersion">Specification version of the OpenAPI document to generate.</param>
        /// <param name="configurationXmlDocument">The XDocument representing the generation configuration.</param>
        /// <returns>The overall generation result.</returns>
        OverallGenerationResult GenerateOpenApiDocuments(
            XDocument annotationXmlDocument,
            IList<string> contractAssemblyPaths,
            OpenApiSpecVersion openApiSpecVersion,
            XDocument configurationXmlDocument);

        /// <summary>
        /// Generates OpenAPI documents using the provided visual studio summary comment and contract assemblies.
        /// </summary>
        /// <param name="summaryComment">The visual studio summary comment.</param>
        /// <param name="contractAssemblyPaths">The list of relative or absolute paths to the contract assemblies.</param>
        /// <param name="openApiSpecVersion">Specification version of the OpenAPI document to generate.</param>
        /// <returns>The overall generation result.</returns>
        OverallGenerationResult GenerateOpenApiDocuments(
            string summaryComment,
            IList<string> contractAssemblyPaths,
            OpenApiSpecVersion openApiSpecVersion);

        /// <summary>
        /// Generates OpenAPI documents using the provided XDocument and contract assemblies.
        /// </summary>
        /// <param name="annotationXmlDocument">The XDocument representing the annotation xml.</param>
        /// <param name="contractAssemblyPaths">The list of relative or absolute paths to the contract assemblies.</param>
        /// <param name="openApiSpecVersion">Specification version of the OpenAPI document to generate.</param>
        /// <param name="openApiFormat">Format (YAML or JSON) of the OpenAPI document to generate.</param>
        /// <returns>The overall generation result with serialized documents.</returns>
        SerializedOverallGenerationResult GenerateSerializedOpenApiDocuments(
            XDocument annotationXmlDocument,
            IList<string> contractAssemblyPaths,
            OpenApiSpecVersion openApiSpecVersion,
            OpenApiFormat openApiFormat);

        /// <summary>
        /// Generates OpenAPI documents using the provided XDocument and contract assemblies.
        /// </summary>
        /// <param name="annotationXmlDocument">The XDocument representing the annotation xml.</param>
        /// <param name="contractAssemblyPaths">The list of relative or absolute paths to the contract assemblies.</param>
        /// <param name="openApiSpecVersion">Specification version of the OpenAPI document to generate.</param>
        /// <param name="openApiFormat">Format (YAML or JSON) of the OpenAPI document to generate.</param>
        /// <param name="configurationXmlDocument">The XDocument representing the generation configuration.</param>
        /// <returns>The overall generation result with serialized documents.</returns>
        SerializedOverallGenerationResult GenerateSerializedOpenApiDocuments(
            XDocument annotationXmlDocument,
            IList<string> contractAssemblyPaths,
            OpenApiSpecVersion openApiSpecVersion,
            OpenApiFormat openApiFormat,
            XDocument configurationXmlDocument);

        /// <summary>
        /// Generates OpenAPI documents using the provided visual studio summary comment and contract assemblies.
        /// </summary>
        /// <param name="summaryComment">The visual studio summary comment.</param>
        /// <param name="contractAssemblyPaths">The list of relative or absolute paths to the contract assemblies.</param>
        /// <param name="openApiSpecVersion">Specification version of the OpenAPI document to generate.</param>
        /// <param name="openApiFormat">Format (YAML or JSON) of the OpenAPI document to generate.</param>
        /// <returns>The overall generation result with serialized documents.</returns>
        SerializedOverallGenerationResult GenerateSerializedOpenApiDocuments(
            string summaryComment,
            IList<string> contractAssemblyPaths,
            OpenApiSpecVersion openApiSpecVersion,
            OpenApiFormat openApiFormat);
    }
}