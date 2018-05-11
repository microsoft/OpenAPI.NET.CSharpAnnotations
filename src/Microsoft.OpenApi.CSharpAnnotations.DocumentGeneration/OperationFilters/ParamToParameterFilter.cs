// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Extensions;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models.KnownStrings;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.OperationFilters
{
    /// <summary>
    /// Parses the value of param tag in xml documentation and apply that as parameter in operation.
    /// </summary>
    public class ParamToParameterFilter : IOperationFilter
    {
        /// <summary>
        /// Fetches the value of "param" tags from xml documentation and populates operation's parameters values.
        /// </summary>
        /// <param name="operation">The operation to be updated.</param>
        /// <param name="element">The xml element representing an operation in the annotation xml.</param>
        /// <param name="settings">The operation filter settings.</param>
        /// <remarks>
        /// Care should be taken to not overwrite the existing value in Operation if already present.
        /// This guarantees the predictable behavior that the first tag in the XML will be respected.
        /// It also guarantees that common annotations in the config file do not overwrite the
        /// annotations in the main documentation.
        /// </remarks>
        public void Apply(OpenApiOperation operation, XElement element, OperationFilterSettings settings)
        {
            var paramElements = element.Elements()
                .Where(
                    p => p.Name == KnownXmlStrings.Param)
                .ToList();

            // Query paramElements again to get all the parameter elements that have "in" attribute.
            // This will include those whose "in" attribute were just populated in PopulateInAttributeFilter, but exclude
            // those that have "in" attribute being "body" since they will be handled as a request body.
            var paramElementsWithIn = paramElements.Where(
                    p =>
                        KnownXmlStrings.InValuesTranslatableToParameter.Contains(
                            p.Attribute(KnownXmlStrings.In)?.Value))
                .ToList();

            foreach (var paramElement in paramElementsWithIn)
            {
                var inValue = paramElement.Attribute(KnownXmlStrings.In)?.Value.Trim();
                var name = paramElement.Attribute(KnownXmlStrings.Name)?.Value.Trim();

                if (inValue == KnownXmlStrings.Path &&
                    !settings.Path.Contains($"{{{name}}}", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                var isRequired = paramElement.Attribute(KnownXmlStrings.Required)?.Value.Trim();
                var cref = paramElement.Attribute(KnownXmlStrings.Cref)?.Value.Trim();

                var childNodes = paramElement.DescendantNodes().ToList();
                var description = string.Empty;

                var lastNode = childNodes.LastOrDefault();

                if (lastNode != null && lastNode.NodeType == XmlNodeType.Text)
                {
                    description = lastNode.ToString().Trim().RemoveBlankLines();
                }

                // Fetch if any see tags are present, if present populate listed types with it.
                var seeNodes = paramElement.Descendants(KnownXmlStrings.See);

                var allListedTypes = seeNodes
                    .Select(node => node.Attribute(KnownXmlStrings.Cref)?.Value)
                    .Where(crefValue => crefValue != null).ToList();

                // If no see tags are present, add the value from cref tag.
                if (!allListedTypes.Any() && !string.IsNullOrWhiteSpace(cref))
                {
                    allListedTypes.Add(cref);
                }

                var schema = GenerateSchemaFromCref(allListedTypes, settings);
                var parameterLocation = GetParameterKind(inValue);

                operation.Parameters.Add(
                    new OpenApiParameter
                    {
                        Name = name,
                        In = parameterLocation,
                        Description = description,
                        Required = parameterLocation == ParameterLocation.Path || Convert.ToBoolean(isRequired),
                        Schema = schema
                    });
            }
        }

        /// <summary>
        /// Generates schema from type names in cref.
        /// </summary>
        /// <returns>
        /// Schema from type in cref if the type is resolvable.
        /// Otherwise, default to schema for string type.
        /// </returns>
        private static OpenApiSchema GenerateSchemaFromCref(IList<string> crefValues, OperationFilterSettings settings)
        {
            var type = typeof(string);

            if (crefValues.Any())
            {
                type = settings.TypeFetcher.LoadTypeFromCrefValues(crefValues);
            }

            return settings.ReferenceRegistryManager.SchemaReferenceRegistry.FindOrAddReference(type);
        }

        private static ParameterLocation GetParameterKind(string parameterKind)
        {
            switch (parameterKind)
            {
                case KnownXmlStrings.Header:
                    return ParameterLocation.Header;

                case KnownXmlStrings.Query:
                    return ParameterLocation.Query;

                default:
                    return ParameterLocation.Path;
            }
        }
    }
}