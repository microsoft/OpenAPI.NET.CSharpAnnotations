// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.OpenApi.CSharpComment.Reader.Models;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpComment.Reader.DocumentConfigFilters
{
    /// <summary>
    /// The class representing the contract of a document config filter.
    /// </summary>
    public interface IDocumentConfigFilter
    {
        /// <summary>
        /// Contains the required logic to manipulate the documents and their document variant info
        /// based on information in the document config element.
        /// </summary>
        /// <param name="documents">The documents to be updated.</param>
        /// <param name="documentConfigElement">The xml element containing document-level config in the config xml.</param>
        /// <param name="xmlDocument">The entire XML documentation</param>
        /// <param name="settings">The document config filter settings.</param>
        void Apply(
            IDictionary<DocumentVariantInfo, OpenApiDocument> documents,
            XElement documentConfigElement,
            XDocument xmlDocument,
            DocumentConfigFilterSettings settings);
    }
}