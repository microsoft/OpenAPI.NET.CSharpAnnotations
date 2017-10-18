// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.OpenApiSpecification.Core.Models;

namespace Microsoft.OpenApiSpecification.Generation.DocumentFilters
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
        /// <param name="xmlDocument">The document representing annotation xml.</param>
        /// <param name="settings">Settings for document filters.</param>
        public void Apply(OpenApiV3SpecificationDocument specificationDocument, XDocument xmlDocument, DocumentFilterSettings settings)
        {
            specificationDocument.Info = new Info
            {
                Title = xmlDocument.XPathSelectElement("//doc/assembly/name")?.Value
            };
        }
    }
}