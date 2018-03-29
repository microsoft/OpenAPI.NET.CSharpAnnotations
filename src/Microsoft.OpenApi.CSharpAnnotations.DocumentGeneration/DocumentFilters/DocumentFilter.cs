// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.OperationFilters;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.DocumentFilters
{
    /// <summary>
    /// The class representing the contract of a filter to process the <see cref="OpenApiDocument"/> based on the
    /// information provided in annotation xml(s), after its processed by the <see cref="OperationFilter"/>.
    /// </summary>
    public abstract class DocumentFilter : IFilter
    {
        /// <summary>
        /// Contains the required logic to populate certain parts of OpenAPI document.
        /// </summary>
        /// <param name="openApiDocument">The OpenAPI document to be updated.</param>
        /// <param name="xmlDocuments">The list of documents representing the annotation xmls,
        /// <see cref="OpenApiGeneratorConfig.AnnotationXmlDocuments"/>.</param>
        /// <param name="settings"><see cref="DocumentFilterSettings"/></param>
        public abstract void Apply(
            OpenApiDocument openApiDocument,
            IList<XDocument> xmlDocuments,
            DocumentFilterSettings settings);

        /// <summary>
        /// The type of filter.
        /// </summary>
        public Type FilterType { get; } = typeof(DocumentFilter);
    }
}