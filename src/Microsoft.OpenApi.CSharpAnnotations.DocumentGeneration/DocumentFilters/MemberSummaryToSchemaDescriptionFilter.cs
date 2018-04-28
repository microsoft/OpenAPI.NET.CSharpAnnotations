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
        public void Apply(
            OpenApiDocument openApiDocument,
            IList<XDocument> xmlDocuments,
            DocumentFilterSettings settings)
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

            foreach (var propertyMember in propertyMembers)
            {
                var fullPropertyName = propertyMember.Attribute(KnownXmlStrings.Name).Value;

                var splitPropertyName = fullPropertyName.Split('.');

                // Take everything before the last period and remove the "P:" prefix.
                var className =
                    string.Join(".", splitPropertyName.Take(splitPropertyName.Length - 1))
                        .Substring(startIndex: 2);

                // We need to sanitize class name to match the format in the schema reference registry.
                // Note that this class may also match several classes in the registry given that generics
                // with different types are treated as different schemas.
                // For example, summary information for properties in class name A 
                // should apply to those properties in schema A, A_B_, and A_B_C__ as well.
                var sanitizedClassName = className.SanitizeClassName();

                var schemas = openApiDocument.Components.Schemas.Where(
                        s => s.Key == sanitizedClassName ||
                            s.Key.StartsWith(sanitizedClassName + "_"))
                    .ToList();

                if (!schemas.Any())
                {
                    continue;
                }

                var propertyName =
                    splitPropertyName[splitPropertyName.Length - 1];

                var propertyInfo = settings.TypeFetcher.LoadType(className)
                        ?.GetProperties()
                        .FirstOrDefault(p => p.Name == propertyName);

                if (propertyInfo != null)
                {
                    var attributes = propertyInfo.GetCustomAttributes(false);

                    foreach (var attribute in attributes)
                    {
                        if (attribute.GetType().FullName == "Newtonsoft.Json.JsonPropertyAttribute")
                        {
                            Type type = attribute.GetType();
                            PropertyInfo propertyNameInfo = type.GetProperty("PropertyName");

                            if (propertyNameInfo != null)
                            {
                                var jsonPropertyName = (string)propertyNameInfo.GetValue(attribute, null);

                                if(!string.IsNullOrWhiteSpace(jsonPropertyName))
                                {
                                    propertyName = jsonPropertyName;
                                }
                            }
                        }
                    }
                }

                foreach (var schema in schemas)
                {
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