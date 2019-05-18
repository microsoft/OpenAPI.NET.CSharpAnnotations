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
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models.KnownStrings;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.DocumentFilters
{
    /// <summary>
    /// Filter to parse the values from the security tags for the type member
    /// to populate <see cref="OpenApiSecurityRequirement"/> in <see cref="OpenApiDocument"/>.
    /// </summary>
    public class SecurityToSecurityRequirementDocumentFilter : IDocumentFilter
    {
        /// <summary>
        /// Parses the values from the the security tags for the type member
        /// to populate <see cref="OpenApiSecurityRequirement"/> in <see cref="OpenApiDocument"/>.
        /// </summary>
        /// <param name="openApiDocument">The Open API specification document to be updated.</param>
        /// <param name="xmlDocuments">The list of documents representing the annotation xmls.</param>
        /// <param name="settings">Settings for document filters.</param>
        /// <param name="openApiDocumentGenerationSettings"><see cref="OpenApiDocumentGenerationSettings"/></param>
        /// <returns>The list of generation errors, if any produced when processing the filter.</returns>
        public IList<GenerationError> Apply(
            OpenApiDocument openApiDocument,
            IList<XDocument> xmlDocuments,
            DocumentFilterSettings settings,
            OpenApiDocumentGenerationSettings openApiDocumentGenerationSettings)
        {
            var generationErrors = new List<GenerationError>();

            try
            {
                if (openApiDocument == null)
                {
                    throw new ArgumentNullException(nameof(openApiDocument));
                }

                if (settings == null)
                {
                    throw new ArgumentNullException(nameof(settings));
                }

                if (xmlDocuments == null)
                {
                    throw new ArgumentNullException(nameof(xmlDocuments));
                }

                var securityElements = new List<XElement>();

                foreach (var xmlDocument in xmlDocuments)
                {
                    securityElements.AddRange(xmlDocument.XPathSelectElements("//doc/members/member")
                        .Where(
                            m => m.Attribute(KnownXmlStrings.Name) != null &&
                                 m.Attribute(KnownXmlStrings.Name).Value.StartsWith("T:"))
                        .Elements()
                        .Where(p => p.Name == KnownXmlStrings.Security));
                }

                if (!securityElements.Any())
                {
                    return generationErrors;
                }

                var openApiSecurityRequirement = new OpenApiSecurityRequirement();
                var securitySchemeRegistry = settings.ReferenceRegistryManager.SecuritySchemeReferenceRegistry;

                foreach (var securityElement in securityElements)
                {
                    var securityScheme = securitySchemeRegistry.FindOrAddReference(securityElement);

                    openApiSecurityRequirement.Add(securityScheme, securitySchemeRegistry.Scopes);
                }

                openApiDocument.SecurityRequirements.Add(openApiSecurityRequirement);
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