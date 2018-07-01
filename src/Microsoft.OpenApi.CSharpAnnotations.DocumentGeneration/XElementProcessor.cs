// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Exceptions;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Extensions;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models.KnownStrings;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.ReferenceRegistries;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration
{
    /// <summary>
    /// Holds common functionality to process xelement that are used across various <see cref="IFilter"/>.
    /// </summary>
    public static class XElementProcessor
    {
        /// <summary>
        /// Fetches type from the "cref" attribute or "see" tag of the provided XElement.
        /// </summary>
        /// <param name="xElement">The XElement to process.</param>
        /// <param name="typeFetcher">The type fetcher.</param>
        /// <param name="useDefaultTypeAsString">Whether to use "string" as default type or not.</param>
        /// <returns><see cref="Type"/></returns>
        public static Type GetType(
            XElement xElement,
            TypeFetcher typeFetcher,
            bool useDefaultTypeAsString = false)
        {
            Type type = null;

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

            if (useDefaultTypeAsString)
            {
                type = typeof(string);
            }

            if (allListedTypes.Any())
            {
                type = typeFetcher.LoadTypeFromCrefValues(allListedTypes);
            }

            return type;
        }

        /// <summary>
        /// Processes the "example" tag child elements of the provide XElement
        /// and generates a map of string to OpenApiExample.
        /// </summary>
        /// <param name="xElement">The XElement to process.</param>
        /// <param name="typeFetcher">The type fetcher.</param>
        /// <returns>The map of string to OpenApiExample.</returns>
        public static Dictionary<string, OpenApiExample> GetOpenApiExamples(
            XElement xElement,
            TypeFetcher typeFetcher)
        {
            var exampleElements = xElement.Elements().Where(p => p.Name == KnownXmlStrings.Example);
            var examples = new Dictionary<string, OpenApiExample>();
            int exampleCounter = 1;

            foreach (var exampleElement in exampleElements)
            {
                var exampleName = exampleElement.Attribute(KnownXmlStrings.Name)?.Value.Trim();
                var example = GetOpenApiExample(exampleElement, typeFetcher);

                if (example != null)
                {
                    examples.Add(
                        string.IsNullOrWhiteSpace(exampleName) ? $"example{exampleCounter++}" : exampleName,
                        example);
                }
            }

            return examples;
        }

        private static OpenApiExample GetOpenApiExample(XElement element, TypeFetcher typeFetcher)
        {
            var exampleChildElements = element.Elements();

            if (!exampleChildElements.Any())
            {
                return null;
            }

            var summaryElement = exampleChildElements.Where(p => p.Name == KnownXmlStrings.Summary).FirstOrDefault();

            var openApiExample = new OpenApiExample();

            if (summaryElement != null)
            {
                openApiExample.Summary = summaryElement.Value;
            }

            var valueElement = exampleChildElements.Where(p => p.Name == KnownXmlStrings.Value).FirstOrDefault();
            var urlElement = exampleChildElements.Where(p => p.Name == KnownXmlStrings.Url).FirstOrDefault();

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
                    .Where(crefVal => crefVal != null).FirstOrDefault();

                if (string.IsNullOrWhiteSpace(valueElement.Value) && string.IsNullOrWhiteSpace(crefValue))
                {
                    throw new InvalidExampleException(SpecificationGenerationMessages.ProvideValueForExample);
                }

                if (!string.IsNullOrWhiteSpace(crefValue))
                {
                    var typeName = crefValue.ExtractTypeNameFromFieldCref();
                    var type = typeFetcher.LoadTypeFromCrefValues(new List<string>() { typeName });
                    var fieldName = crefValue.ExtractFieldNameFromCref();

                    FieldInfo[] fields = type.GetFields(BindingFlags.Public
                        | BindingFlags.Static);
                    var field = fields.Where(f => f.Name == fieldName).FirstOrDefault();

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
                        out OpenApiDiagnostic openApiDiagnostic);
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
        /// Processes the "header" tag child elements of the provide XElement
        /// and generates a map of string to OpenApiHeader.
        /// </summary>
        /// <param name="xElement">The XElement to process.</param>
        /// <param name="typeFetcher">The type fetcher.</param>
        /// <param name="schemaReferenceRegistry">The schema reference registry.</param>
        /// <returns>The map of string to OpenApiHeader.</returns>
        public static Dictionary<string,OpenApiHeader> GetOpenApiHeaders(
            XElement xElement,
            TypeFetcher typeFetcher,
            SchemaReferenceRegistry schemaReferenceRegistry)
        {
            var headerElements = xElement.Elements()
                .Where(
                    p => p.Name == KnownXmlStrings.Header)
                .ToList();

            var openApiHeaders = new Dictionary<string, OpenApiHeader>();

            foreach(var headerElement in headerElements)
            {
                var name = headerElement.Attribute(KnownXmlStrings.Name)?.Value.Trim();
                if(string.IsNullOrWhiteSpace(name))
                {
                    throw new InvalidHeaderException(
                        string.Format(SpecificationGenerationMessages.MissingNameAttribute, "header"));
                }

                var description = headerElement.Elements()
                    .Where(p => p.Name == KnownXmlStrings.Description)
                    .FirstOrDefault()?.Value.Trim().RemoveBlankLines();

                var type = GetType(headerElement, typeFetcher);

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