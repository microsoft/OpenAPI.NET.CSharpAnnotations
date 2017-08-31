// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.OpenApiSpecification.Generation.Models;

namespace Microsoft.OpenApiSpecification.Generation
{
    /// <summary>
    /// The contract for open api document generator.
    /// </summary>
    public interface IOpenApiDocumentGenerator
    {
        /// <summary>
        /// Generates V3 documents using the provided xdocument and contract assemblies.
        /// </summary>
        /// <param name="annotationXmlDocument">The XDocument representing the annotation xml.</param>
        /// <param name="contractAssemblyPaths">The list of relative or absolute paths to the contract assemblies.</param>
        /// <returns>The open api document generation result.</returns>
        DocumentGenerationResult GenerateV3Documents(
            XDocument annotationXmlDocument,
            IList<string> contractAssemblyPaths);

        /// <summary>
        /// Generates V3 documents using the provided visual studio summary comment and contract assemblies.
        /// </summary>
        /// <param name="summaryComment">The visual studio summary comment.</param>
        /// <param name="contractAssemblyPaths">The list of relative or absolute paths to the contract assemblies.</param>
        /// <returns>The open api document generation result.</returns>
        DocumentGenerationResult GenerateV3Documents(
            string summaryComment,
            IList<string> contractAssemblyPaths);
    }
}