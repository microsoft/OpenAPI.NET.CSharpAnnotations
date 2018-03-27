// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.DocumentFilters;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Extensions;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models.KnownStrings;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.OpenApiDocumentGeneratorTests
{
    /// <summary>
    /// Filter to grab newtonsoft json property attribute and if it contains name and required information for a
    /// property, update the corresponding property schema with it.
    /// </summary>
    public class UpdateSchemaWithNewtonsoftJsonPropertyAttributeFilter : IDocumentFilter
    {
        /// <summary>
        /// Fetches the newtonsoft json property attribute and if it contains name and required information for a
        /// property, update the corresponding property schema with it.
        /// </summary>
        /// <param name="specificationDocument">The Open Api V3 specification document to be updated.</param>
        /// <param name="xmlDocuments">The list of documents representing the annotation xmls.</param>
        /// <param name="settings">Settings for document filters.</param>
        public void Apply(OpenApiDocument specificationDocument, IList<XDocument> xmlDocuments, DocumentFilterSettings settings)
        {
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

                var schemas = specificationDocument.Components.Schemas.Where(
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

                string jsonPropertyName = null;
                bool isPropertyRequired = false;

                if (propertyInfo != null)
                {
                    var jsonPropertyAttributes =
                        (JsonPropertyAttribute[])propertyInfo.GetCustomAttributes(
                            typeof(JsonPropertyAttribute),
                            inherit: false);
                    if (jsonPropertyAttributes.Any())
                    {
                        // Extract the property name in JsonProperty if given.
                        if (jsonPropertyAttributes[0].PropertyName != null)
                        {
                            jsonPropertyName = jsonPropertyAttributes[0].PropertyName;
                        }

                        // Check if the property is required.
                        if (jsonPropertyAttributes[0].Required == Required.Always)
                        {
                            isPropertyRequired = true;
                        }
                    }
                }

                foreach (var schema in schemas)
                {
                    if (schema.Value.Properties.ContainsKey(propertyName))
                    {
                        if (isPropertyRequired)
                        {
                            schema.Value.Required.Add(string.IsNullOrWhiteSpace(jsonPropertyName)
                                ? propertyName : jsonPropertyName);
                        }

                        if (!string.IsNullOrWhiteSpace(jsonPropertyName) && propertyName != jsonPropertyName)
                        {
                            var propertySchema = schema.Value.Properties[propertyName];

                            //Remove old key.
                            schema.Value.Properties.Remove(propertyName);

                            //Add with new json property name.
                            schema.Value.Properties.Add(jsonPropertyName, propertySchema);
                        }
                    }
                }
            }
        }
    }
}