// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpComment.Reader.DocumentFilters
{
    /// <summary>
    /// The class representing the contract of a filter to process the <see cref="OpenApiDocument"/> based on the
    /// information provided in annotation xml(s).
    /// </summary>
    public interface IDocumentFilter
    {
        /// <summary>
        /// Contains the required logic to populate certain parts of Open Api V3 specification document.
        /// </summary>
        /// <param name="specificationDocument">The Open Api V3 specification document to be updated.</param>
        /// <param name="xmlDocuments">The list of documents representing the annotation xmls.</param>
        /// <param name="settings">Settings for document filters.</param>
        void Apply(
            OpenApiDocument specificationDocument,
            IList<XDocument> xmlDocuments,
            DocumentFilterSettings settings);
    }
}