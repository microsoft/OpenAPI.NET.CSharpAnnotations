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
        /// Generates Open API documents using the provided xdocument and contract assemblies.
        /// </summary>
        /// <param name="annotationXmlDocument">The XDocument representing the annotation xml.</param>
        /// <param name="contractAssemblyPaths">The list of relative or absolute paths to the contract assemblies.</param>
        /// <param name="configurationXmlDocument">The XDocument representing the generation configuration.</param>
        /// <param name="openApiSpecVersion">Specification version of the Open API document to generate.</param>
        /// <returns>The open api document generation result.</returns>
        DocumentGenerationResult GenerateOpenApiDocuments(
            XDocument annotationXmlDocument,
            IList<string> contractAssemblyPaths,
            XDocument configurationXmlDocument,
            OpenApiSpecVersion openApiSpecVersion);

        /// <summary>
        /// Generates Open API documents using the provided xdocument and contract assemblies.
        /// </summary>
        /// <param name="annotationXmlDocument">The XDocument representing the annotation xml.</param>
        /// <param name="contractAssemblyPaths">The list of relative or absolute paths to the contract assemblies.</param>
        /// <param name="openApiSpecVersion">Specification version of the Open API document to generate.</param>
        /// <returns>The open api document generation result.</returns>
        DocumentGenerationResult GenerateOpenApiDocuments(
            XDocument annotationXmlDocument,
            IList<string> contractAssemblyPaths,
            OpenApiSpecVersion openApiSpecVersion);

        /// <summary>
        /// Generates Open API documents using the provided visual studio summary comment and contract assemblies.
        /// </summary>
        /// <param name="summaryComment">The visual studio summary comment.</param>
        /// <param name="contractAssemblyPaths">The list of relative or absolute paths to the contract assemblies.</param>
        /// <param name="openApiSpecVersion">Specification version of the Open API document to generate.</param>
        /// <returns>The open api document generation result.</returns>
        DocumentGenerationResult GenerateOpenApiDocuments(
            string summaryComment,
            IList<string> contractAssemblyPaths,
            OpenApiSpecVersion openApiSpecVersion);
    }
}