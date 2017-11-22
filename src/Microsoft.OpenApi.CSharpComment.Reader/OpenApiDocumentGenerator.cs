// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Microsoft.OpenApi.CSharpComment.Reader.Models;
using Newtonsoft.Json;

namespace Microsoft.OpenApi.CSharpComment.Reader
{
    /// <summary>
    /// The class that holds functionality to generate open api document.
    /// </summary>
    public class OpenApiDocumentGenerator : IOpenApiDocumentGenerator
    {
        /// <summary>
        /// Generates V3 documents using the provided xdocument and contract assemblies.
        /// </summary>
        /// <param name="annotationXmlDocument">The XDocument representing the annotation xml.</param>
        /// <param name="contractAssemblyPaths">The list of relative or absolute paths to the contract assemblies.</param>
        /// <param name="configurationXmlDocument">The XDocument representing the generation configuration.</param>
        /// <returns>The open api document generation result.</returns>
        public DocumentGenerationResult GenerateV3Documents(
            XDocument annotationXmlDocument,
            IList<string> contractAssemblyPaths,
            XDocument configurationXmlDocument)
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
                    configurationXmlDocument?.ToString());

                var documentGenerationResultWithDocumentAsString =
                    JsonConvert.DeserializeObject<DocumentGenerationResultWithDocumentAsString>(result);

                return documentGenerationResultWithDocumentAsString.ToDocumentGenerationResult();
            }
        }

        /// <summary>
        /// Generates V3 documents using the provided xdocument and contract assemblies.
        /// </summary>
        /// <param name="annotationXmlDocument">The XDocument representing the annotation xml.</param>
        /// <param name="contractAssemblyPaths">The list of relative or absolute paths to the contract assemblies.</param>
        /// <returns>The open api document generation result.</returns>
        public DocumentGenerationResult GenerateV3Documents(
            XDocument annotationXmlDocument,
            IList<string> contractAssemblyPaths)
        {
            return GenerateV3Documents(
                annotationXmlDocument,
                contractAssemblyPaths,
                configurationXmlDocument: null);
        }

        /// <summary>
        /// Generates V3 documents using the provided visual studio summary comment and contract assemblies.
        /// </summary>
        /// <param name="summaryComment">The visual studio summary comment.</param>
        /// <param name="contractAssemblyPaths">The list of relative or absolute paths to the contract assemblies.</param>
        /// <returns>The open api document generation result.</returns>
        public DocumentGenerationResult GenerateV3Documents(
            string summaryComment,
            IList<string> contractAssemblyPaths)
        {
            return null;
        }
    }
}