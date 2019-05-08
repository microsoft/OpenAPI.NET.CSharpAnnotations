// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.DocumentFilters
{
    /// <summary>
    /// Parses the value of assembly tag in xml documentation and apply that as info in Open Api V3 specification document.
    /// </summary>
    public class AssemblyNameToInfoFilter : IDocumentFilter
    {
        /// <summary>
        /// Fetches the value of "assembly" tag from xml documentation and use it to populate
        /// Info object of Open Api V3 specification document.
        /// </summary>
        /// <param name="specificationDocument">The Open Api V3 specification document to be updated.</param>
        /// <param name="xmlDocuments">The list of documents representing the annotation xmls.</param>
        /// <param name="settings">Settings for document filters.</param>
        /// <param name="openApiDocumentGenerationSettings"><see cref="OpenApiDocumentGenerationSettings"/></param>
        /// <returns>The list of generation errors, if any produced when processing the filter."></returns>
        public IList<GenerationError> Apply(
            OpenApiDocument specificationDocument,
            IList<XDocument> xmlDocuments,
            DocumentFilterSettings settings,
            OpenApiDocumentGenerationSettings openApiDocumentGenerationSettings)
        {
            var generationErrors = new List<GenerationError>();

            try
            {
                // Find the xml document that contains member tag with url and verb,
                // as that should be the service api documenation xml.
                var xmlDocument = xmlDocuments
                    .FirstOrDefault(i => i.XPathSelectElement("//doc/members/member[url and verb]") != null);

                if (xmlDocument == null)
                {
                    return generationErrors;
                }

                specificationDocument.Info = new OpenApiInfo
                {
                    Title = xmlDocument.XPathSelectElement("//doc/assembly/name")?.Value,
                    Version = settings.OpenApiDocumentVersion,
                    Description = settings.OpenApiInfoDescription
                };
            }
            catch(Exception ex)
            {
                generationErrors.Add(
                   new GenerationError
                   {
                       Message = ex.Message,
                       ExceptionType = ex.GetType().Name
                   });
            }

            return generationErrors;
        }
    }
}