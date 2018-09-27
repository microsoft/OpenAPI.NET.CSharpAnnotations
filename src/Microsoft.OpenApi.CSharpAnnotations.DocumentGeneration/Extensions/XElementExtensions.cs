// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Exceptions;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models.KnownStrings;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.ReferenceRegistries;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="XElement"/>.
    /// </summary>
    public static class XElementExtensions
    {
        /// <summary>
        /// Get the text inside the element with the following modifications:
        /// 1. The see/seealso cref value and the paramref/typeparamref name value extracted out of the inner XML elements.
        /// 2. The para tag is ignored.
        /// 3. Any blank lines are removed.
        /// 4. Beginning and trailing whitespaces are trimmed.
        /// </summary>
        public static string GetDescriptionText(this XElement element)
        {
            var description = new StringBuilder();

            var children = element.Nodes();

            foreach (var child in children)
            {
                switch (child.NodeType)
                {
                    case XmlNodeType.Text:
                        description.Append(child);
                        break;

                    case XmlNodeType.CDATA:
                        description.Append(child.ToString()
                            .Replace("<![CDATA[", string.Empty).Replace("]]>", string.Empty));
                        break;

                    case XmlNodeType.Element:
                        var childElement = (XElement) child;

                        switch (childElement.Name.ToString())
                        {
                            case KnownXmlStrings.Para:
                                description.Append(GetDescriptionText(childElement));
                                break;

                            case KnownXmlStrings.See:
                                description.Append(childElement.Attribute(KnownXmlStrings.Cref)?.Value);
                                break;

                            case KnownXmlStrings.Seealso:
                                description.Append(childElement.Attribute(KnownXmlStrings.Cref)?.Value);
                                break;

                            case KnownXmlStrings.Paramref:
                                description.Append(childElement.Attribute(KnownXmlStrings.Name)?.Value);
                                break;

                            case KnownXmlStrings.Typeparamref:
                                description.Append(childElement.Attribute(KnownXmlStrings.Name)?.Value);
                                break;
                        }
                        break;
                }
            }

            return description.ToString().Trim().RemoveBlankLines();
        }

        /// <summary>
        /// Gets the text from the last text node of the provided element's child nodes with the following modifications:
        /// 1. Any blank lines are removed.
        /// 2. Beginning and trailing whitespaces are trimmed.
        /// </summary>
        public static string GetDescriptionTextFromLastTextNode(this XElement element)
        {
            var lastTextNode = element
                .Nodes()
                .LastOrDefault(i => i.NodeType == XmlNodeType.Text);

            if (lastTextNode != null)
            {
                return lastTextNode.ToString().Trim().RemoveBlankLines();
            }

            return string.Empty;
        }

        /// <summary>
        /// Fetches list of fully qualified type names from the "cref" attribute or "see" tag of the provided XElement.
        /// </summary>
        /// <param name="xElement">The XElement to process.</param>
        /// <returns><see cref="Type"/></returns>
        internal static IList<string> GetListedTypes(
            this XElement xElement)
        {
            var cref = xElement.Attribute(KnownXmlStrings.Cref)?.Value.Trim();
            var seeNodes = xElement.Elements().Where(i => i.Name == KnownXmlStrings.See);

            var allListedTypes = seeNodes
                .Select(node => node.Attribute(KnownXmlStrings.Cref)?.Value)
                .Where(crefValue => crefValue != null).ToList();

            // If no see tags are present, add the value from cref tag.
            if (!allListedTypes.Any() && !string.IsNullOrWhiteSpace(cref))
            {
                allListedTypes.Add(cref);
            }

            return allListedTypes;
        }

        private static OpenApiExample ToOpenApiExample(this XElement element, TypeFetcher typeFetcher)
        {
            var exampleChildElements = element.Elements();

            if (!exampleChildElements.Any())
            {
                return null;
            }

            var summaryElement = exampleChildElements.FirstOrDefault(p => p.Name == KnownXmlStrings.Summary);

            var openApiExample = new OpenApiExample();

            if (summaryElement != null)
            {
                openApiExample.Summary = summaryElement.Value;
            }

            var valueElement = exampleChildElements.FirstOrDefault(p => p.Name == KnownXmlStrings.Value);
            var urlElement = exampleChildElements.FirstOrDefault(p => p.Name == KnownXmlStrings.Url);

            if (valueElement != null && urlElement != null)
            {
                throw new InvalidExampleException(SpecificationGenerationMessages.ProvideEitherValueOrUrlTag);
            }

            IOpenApiAny exampleValue = null;

            if (valueElement != null)
            {
                var seeNodes = element.Descendants(KnownXmlStrings.See);
                var crefValue = seeNodes
                    .Select(node => node.Attribute(KnownXmlStrings.Cref)?.Value)
                    .FirstOrDefault(crefVal => crefVal != null);

                if (string.IsNullOrWhiteSpace(valueElement.Value) && string.IsNullOrWhiteSpace(crefValue))
                {
                    throw new InvalidExampleException(SpecificationGenerationMessages.ProvideValueForExample);
                }

                if (!string.IsNullOrWhiteSpace(crefValue))
                {
                    var typeName = crefValue.ExtractTypeNameFromFieldCref();
                    var type = typeFetcher.LoadTypeFromCrefValues(new List<string> {typeName});
                    var fieldName = crefValue.ExtractFieldNameFromCref();

                    var fields = type.GetFields(BindingFlags.Public
                                                | BindingFlags.Static);
                    var field = fields.FirstOrDefault(f => f.Name == fieldName);

                    if (field == null)
                    {
                        var errorMessage = string.Format(
                            SpecificationGenerationMessages.FieldNotFound,
                            fieldName,
                            typeName);

                        throw new TypeLoadException(errorMessage);
                    }

                    exampleValue = new OpenApiStringReader().ReadFragment<IOpenApiAny>(
                        field.GetValue(null).ToString(),
                        OpenApiSpecVersion.OpenApi3_0,
                        out OpenApiDiagnostic _);
                }

                if (!string.IsNullOrWhiteSpace(valueElement.Value))
                {
                    exampleValue = new OpenApiStringReader()
                        .ReadFragment<IOpenApiAny>(
                            valueElement.Value,
                            OpenApiSpecVersion.OpenApi3_0,
                            out OpenApiDiagnostic _);
                }

                openApiExample.Value = exampleValue;
            }

            if (urlElement != null)
            {
                openApiExample.ExternalValue = urlElement.Value;
            }

            return openApiExample;
        }

        /// <summary>
        /// Processes the "example" tag child elements of the provide XElement
        /// and generates a map of string to OpenApiExample.
        /// </summary>
        /// <param name="xElement">The XElement to process.</param>
        /// <param name="typeFetcher">The type fetcher.</param>
        /// <returns>The map of string to OpenApiExample.</returns>
        internal static Dictionary<string, OpenApiExample> ToOpenApiExamples(
            this XElement xElement,
            TypeFetcher typeFetcher)
        {
            var exampleElements = xElement.Elements().Where(p => p.Name == KnownXmlStrings.Example);
            var examples = new Dictionary<string, OpenApiExample>();
            var exampleCounter = 1;

            foreach (var exampleElement in exampleElements)
            {
                var exampleName = exampleElement.Attribute(KnownXmlStrings.Name)?.Value.Trim();
                var example = exampleElement.ToOpenApiExample(typeFetcher);

                if (example != null)
                {
                    examples.Add(
                        string.IsNullOrWhiteSpace(exampleName) ? $"example{exampleCounter++}" : exampleName,
                        example);
                }
            }

            return examples;
        }

        /// <summary>
        /// Processes the "header" tag child elements of the provide XElement
        /// and generates a map of string to OpenApiHeader.
        /// </summary>
        /// <param name="xElement">The XElement to process.</param>
        /// <param name="typeFetcher">The type fetcher.</param>
        /// <param name="schemaReferenceRegistry">The schema reference registry.</param>
        /// <returns>The map of string to OpenApiHeader.</returns>
        internal static Dictionary<string, OpenApiHeader> ToOpenApiHeaders(
            this XElement xElement,
            TypeFetcher typeFetcher,
            SchemaReferenceRegistry schemaReferenceRegistry)
        {
            var headerElements = xElement.Elements()
                .Where(
                    p => p.Name == KnownXmlStrings.Header)
                .ToList();

            var openApiHeaders = new Dictionary<string, OpenApiHeader>();

            foreach (var headerElement in headerElements)
            {
                var name = headerElement.Attribute(KnownXmlStrings.Name)?.Value.Trim();
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new InvalidHeaderException(
                        string.Format(SpecificationGenerationMessages.UndocumentedName, "header"));
                }

                var description = headerElement
                    .Elements()
                    .FirstOrDefault(p => p.Name == KnownXmlStrings.Description)?.Value.Trim().RemoveBlankLines();

                var listedTypes = headerElement.GetListedTypes();
                var type = typeFetcher.LoadTypeFromCrefValues(listedTypes);

                var schema = schemaReferenceRegistry.FindOrAddReference(type);
                openApiHeaders.Add(
                    name,
                    new OpenApiHeader
                    {
                        Description = description,
                        Schema = schema
                    });
            }

            return openApiHeaders;
        }
    }
}