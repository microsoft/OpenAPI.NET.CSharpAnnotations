// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.DocumentFilters;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.DocumentConfigFilters
{
    /// <summary>
    /// The class representing the contract of a filter to process the <see cref="Dictionary{TKey,TValue}"/>
    /// where TKey is <see cref="DocumentVariantInfo"/> and TValue is <see cref="OpenApiDocument"/>
    /// based on the information in the document config element of
    /// <see cref="OpenApiGeneratorConfig.AdvancedConfigurationXmlDocument"/>, after its processed by the
    /// <see cref="IDocumentFilter"/>.
    /// </summary>
    public interface IDocumentConfigFilter : IFilter
    {
        /// <summary>
        /// Contains the required logic to manipulate the documents and their document variant info
        /// based on information in the document config element.
        /// </summary>
        /// <param name="documents">The documents to be updated.</param>
        /// <param name="documentConfigElement">The xml element containing document-level config in the config xml,
        /// <see cref="OpenApiGeneratorConfig.AdvancedConfigurationXmlDocument"/>.</param>
        /// <param name="xmlDocuments">The list of XML documentations provided in
        /// <see cref="OpenApiGeneratorConfig.AnnotationXmlDocuments"/>.</param>
        /// <param name="settings"><see cref="DocumentConfigFilterSettings"/></param>
        /// <returns>The list of generation errors, if any produced when processing the filter."></returns>
        IList<GenerationError> Apply(
            IDictionary<DocumentVariantInfo, OpenApiDocument> documents,
            XElement documentConfigElement,
            IList<XDocument> xmlDocuments,
            DocumentConfigFilterSettings settings);
    }
}