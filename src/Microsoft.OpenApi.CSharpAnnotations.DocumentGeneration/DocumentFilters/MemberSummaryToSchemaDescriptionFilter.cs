// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Extensions;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models.KnownStrings;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.DocumentFilters
{
    /// <summary>
    /// Filter to parse the values from the summary tags for all the properties
    /// to populate descriptions in the schema.
    /// </summary>
    public class MemberSummaryToSchemaDescriptionFilter : IDocumentFilter
    {
        /// <summary>
        /// Parses the values from the summary for all the properties to populate descriptions in the schema.
        /// </summary>
        /// <param name="openApiDocument">The Open API specification document to be updated.</param>
        /// <param name="xmlDocuments">The list of documents representing the annotation xmls.</param>
        /// <param name="settings">Settings for document filters.</param>
        /// <param name="openApiDocumentGenerationSettings"><see cref="OpenApiDocumentGenerationSettings"/></param>
        public void Apply(
            OpenApiDocument openApiDocument,
            IList<XDocument> xmlDocuments,
            DocumentFilterSettings settings,
            OpenApiDocumentGenerationSettings openApiDocumentGenerationSettings)
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

            var propertyMembers = new List<XElement>();

            foreach (var xmlDocument in xmlDocuments)
            {
                propertyMembers.AddRange(xmlDocument.XPathSelectElements("//doc/members/member")
                    .Where(
                        m => m.Attribute(KnownXmlStrings.Name) != null &&
                             m.Attribute(KnownXmlStrings.Name).Value.StartsWith("P:")));
            }

            var typeNames = propertyMembers.Attributes(KnownXmlStrings.Name).Select(i => string
                .Join(".", i.Value.Split('.').Take(i.Value.Split('.').Length - 1))
                .Substring(2)).Distinct().ToList();

            foreach (var typeName in typeNames)
            {
                // We need to sanitize class name to match the format in the schema reference registry.
                // Note that this class may also match several classes in the registry given that generics
                // with different types are treated as different schemas.
                // For example, summary information for properties in class name A 
                // should apply to those properties in schema A, A_B_, and A_B_C__ as well.
                var sanitizedClassName = typeName.SanitizeClassName();

                var schemas = openApiDocument.Components.Schemas.Where(
                        s => s.Key == sanitizedClassName ||
                             s.Key.StartsWith(sanitizedClassName + "_"))
                    .ToList();

                if (!schemas.Any())
                {
                    continue;
                }

                var typeInfo = settings.TypeFetcher.LoadType(typeName);

                var typesToFetchPropertiesFor = settings.TypeFetcher.GetBaseTypes(typeInfo);
                typesToFetchPropertiesFor.Add(typeInfo);

                var propertiesMap = new Dictionary<string, PropertyInfo>();

                foreach (var type in typesToFetchPropertiesFor)
                {
                    foreach (var property in type.GetProperties())
                    {
                        if (!propertiesMap.ContainsKey(property.Name))
                        {
                            propertiesMap.Add(property.Name, property);
                        }
                    }
                }

                foreach (var property in propertiesMap.Keys)
                {
                    var propertyName = openApiDocumentGenerationSettings
                        .SchemaGenerationSettings
                        .PropertyNameResolver
                        .ResolvePropertyName(propertiesMap[property]);

                    foreach (var schema in schemas)
                    {
                        var propertyMember = propertyMembers.FirstOrDefault(
                            i => i.Attribute(KnownXmlStrings.Name)?.Value ==
                                 $"P:{propertiesMap[property].DeclaringType}.{property}");

                        if (propertyMember == null)
                        {
                            continue;
                        }

                        if (schema.Value.Properties.ContainsKey(propertyName))
                        {
                            schema.Value.Properties[propertyName].Description =
                                propertyMember.Element(KnownXmlStrings.Summary)?.Value.RemoveBlankLines();
                        }
                    }
                }
            }
        }
    }
}