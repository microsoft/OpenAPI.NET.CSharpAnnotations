// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.OpenApiSpecification.Core.Models;
using Microsoft.OpenApiSpecification.Generation.Exceptions;
using Microsoft.OpenApiSpecification.Generation.Extensions;
using Microsoft.OpenApiSpecification.Generation.Models;
using Microsoft.OpenApiSpecification.Generation.Models.KnownStrings;
using Newtonsoft.Json;

namespace Microsoft.OpenApiSpecification.Generation.DocumentConfigFilters
{
    /// <summary>
    /// Populate the <see cref="DocumentVariantInfo.Attributes"/>.
    /// </summary>
    public class ApplyDocumentVariantAttributesFilter : IDocumentConfigFilter
    {
        /// <summary>
        /// Fetches the attributes in document variant option as well as the entire documentation itself
        /// and populate the document variant info objects.
        /// </summary>
        /// <param name="documents">The documents to be updated.</param>
        /// <param name="element">The xml element containing document-level config in the config xml.</param>
        /// <param name="xmlDocument">The entire XML documentation</param>
        /// <param name="settings">The document config filter settings.</param>
        /// <exception cref="ConflictingDocumentVariantAttributesException">
        /// Thrown when there is a conflict
        /// between the attributes in the existing document variant info and the new one.
        /// </exception>
        public void Apply(
            IDictionary<DocumentVariantInfo, OpenApiV3SpecificationDocument> documents,
            XElement element,
            XDocument xmlDocument,
            DocumentConfigFilterSettings settings)
        {
            var variantElements = element.Elements(KnownXmlStrings.Variant);
            
            foreach (var variantElement in variantElements)
            {
                // First, populate the document variant info attributes using the document variant option.
                var variantOptions = variantElement.Element(KnownXmlStrings.Options)?.Elements(KnownXmlStrings.Option);

                if (variantOptions != null)
                {
                    foreach (var variantOption in variantOptions)
                    {
                        var documentVariantInfo = new DocumentVariantInfo
                        {
                            Title = variantOption.Value,
                            Categorizer = variantElement.Element(KnownXmlStrings.Name)?.Value
                        };

                        foreach (var attribute in variantOption.Attributes())
                        {
                            if (!documentVariantInfo.Attributes.ContainsKey(attribute.Name.ToString()))
                            {
                                documentVariantInfo.Attributes[attribute.Name.ToString()] = attribute.Value;
                            }
                        }

                        PopulateAttributesInExistingDocumentVariantInfo(documents, documentVariantInfo);
                    }
                }

                // Second, populate the document variant info attributes using the information from the entire documentation.
                var allCategorizerNodes = xmlDocument.Descendants(variantElement.Element(KnownXmlStrings.Name)?.Value);
                
                foreach (var categorizerNode in allCategorizerNodes)
                {
                    var documentVariantInfo = new DocumentVariantInfo
                    {
                        Title = categorizerNode.Value,
                        Categorizer = categorizerNode.Name.ToString()
                    };

                    foreach (var attribute in categorizerNode.Attributes())
                    {
                        if (!documentVariantInfo.Attributes.ContainsKey(attribute.Name.ToString()))
                        {
                            documentVariantInfo.Attributes[attribute.Name.ToString()] = attribute.Value;
                        }
                    }

                    PopulateAttributesInExistingDocumentVariantInfo(documents, documentVariantInfo);
                }
            }

        }

        private void PopulateAttributesInExistingDocumentVariantInfo(
            IDictionary<DocumentVariantInfo, OpenApiV3SpecificationDocument> documents,
            DocumentVariantInfo documentVariantInfo)
        {
            foreach (var existingDocumentVariantInfo in documents.Keys)
            {
                if (!documentVariantInfo.Attributes.Any())
                {
                    // If the new document variant info has no attributes, simply ignore and move on to
                    // the next document variant info.
                    continue;
                }

                if (!existingDocumentVariantInfo.Equals(documentVariantInfo))
                {                    
                    // If the document variant infos are not equal, we can move on to the next one.
                    continue;
                }

                if (!existingDocumentVariantInfo.Attributes.Any())
                {
                    // If the existing document variant info has no attributes, simply point its Attributes
                    // to the Attributes in the new document variant info.
                    existingDocumentVariantInfo.Attributes = documentVariantInfo.Attributes;
                    break;
                }

                if (!existingDocumentVariantInfo.AreAttributesEquivalent(documentVariantInfo))
                {
                    // If the attributes are present in the existing document variant info but they
                    // are not equivalent to the new one, there is a conflict.
                    throw new ConflictingDocumentVariantAttributesException(
                        existingDocumentVariantInfo,
                        documentVariantInfo);
                }
            }
        }
    }
}