// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.OpenApi.CSharpComment.Reader.OperationFilters;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpComment.Reader.DocumentFilters
{
    /// <summary>
    /// The class representing the contract of a filter to process the <see cref="OpenApiDocument"/> based on the
    /// information provided in annotation xml(s), after its processed by the <see cref="IOperationFilter"/>.
    /// </summary>
    public interface IDocumentFilter
    {
        /// <summary>
        /// Contains the required logic to populate certain parts of OpenAPI document.
        /// </summary>
        /// <param name="openApiDocument">The OpenAPI document to be updated.</param>
        /// <param name="xmlDocuments">The list of documents representing the annotation xmls,
        /// <see cref="CSharpCommentOpenApiGeneratorConfig.AnnotationXmlDocuments"/>.</param>
        /// <param name="settings"><see cref="DocumentFilterSettings"/></param>
        void Apply(
            OpenApiDocument openApiDocument,
            IList<XDocument> xmlDocuments,
            DocumentFilterSettings settings);
    }
}