// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Xml.Linq;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpComment.Reader.DocumentFilters
{
    /// <summary>
    /// Adds the specification version to the document.
    /// </summary>
    public class AddSpecVersionFilter : IDocumentFilter
    {
        /// <summary>
        /// Adds the specification version to the document.
        /// </summary>
        /// <param name="specificationDocument">The Open Api V3 specification document to be updated.</param>
        /// <param name="xmlDocument">The document representing annotation xml.</param>
        /// <param name="settings">Settings for document filters.</param>
        public void Apply(OpenApiDocument specificationDocument, XDocument xmlDocument, DocumentFilterSettings settings)
        {
            specificationDocument.SpecVersion = new Version(3, 0, 0);
        }
    }
}