// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.OpenApi.CSharpComment.Reader.Extensions;
using Microsoft.OpenApi.CSharpComment.Reader.Models.KnownStrings;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpComment.Reader.OperationFilters
{
    /// <summary>
    /// Parses the value of response tag in xml documentation and apply that as response in operation.
    /// </summary>
    public class ResponseToResponseFilter : IOperationFilter
    {
        /// <summary>
        /// Fetches the value of "response" tags from xml documentation and populates operation's response.
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
            var responseElements = element.Elements().Where(p => p.Name == KnownXmlStrings.Response ||
            p.Name == KnownXmlStrings.ResponseType);

            foreach (var responseElement in responseElements)
            {
                var code = responseElement.Attribute(KnownXmlStrings.Code)?.Value;

                if (code == null)
                {
                    continue;
                }

                var mediaType = responseElement.Attribute(KnownXmlStrings.Type)?.Value ?? "application/json";

                var childNodes = responseElement.DescendantNodes().ToList();
                var description = string.Empty;

                var lastNode = childNodes.LastOrDefault();

                if (lastNode != null && lastNode.NodeType == XmlNodeType.Text)
                {
                    description = lastNode.ToString();
                }

                var allListedTypes = new List<string>();

                var seeNodes = responseElement.Descendants(KnownXmlStrings.See);

                foreach (var node in seeNodes)
                {
                    var crefValue = node.Attribute(KnownXmlStrings.Cref)?.Value;

                    if (crefValue != null)
                    {
                        allListedTypes.Add(crefValue);
                    }
                }

                var responseContractType = settings.TypeFetcher.LoadTypeFromCrefValues(allListedTypes);

                var schema = settings.ReferenceRegistryManager.SchemaReferenceRegistry.FindOrAddReference(
                    responseContractType);

                if (operation.Responses.ContainsKey(code))
                {
                    if (string.IsNullOrWhiteSpace(operation.Responses[code].Description))
                    {
                        operation.Responses[code].Description = description.RemoveBlankLines();
                    }

                    if (!operation.Responses[code].Content.ContainsKey(mediaType))
                    {
                        operation.Responses[code].Content[mediaType] = new OpenApiMediaType
                        {
                            Schema = schema
                        };
                    }
                    else
                    {
                        // If the existing schema is just a single schema (not a list of AnyOf), then
                        // we create a new schema and add that schema to AnyOf to allow us to add
                        // more schemas to it later.
                        if (!operation.Responses[code].Content[mediaType].Schema.AnyOf.Any())
                        {
                            var existingSchema = operation.Responses[code].Content[mediaType].Schema;
                            var newSchema = new OpenApiSchema();

                            newSchema.AnyOf.Add(existingSchema);

                            operation.Responses[code].Content[mediaType].Schema = newSchema;
                        }
                        
                        operation.Responses[code].Content[mediaType].Schema.AnyOf.Add(schema);
                    }
                }
                else
                {
                    var response = new OpenApiResponse
                    {
                        Description = description.RemoveBlankLines(),
                        Content =
                        {
                            [mediaType] = new OpenApiMediaType {Schema = schema}
                        }
                    };

                    operation.Responses.Add(code, response);
                }
            }

            if (!operation.Responses.Any())
            {
                operation.Responses.Add("default", new OpenApiResponse { Description = "Cannot locate responses in the documentation!"});
            }
        }
    }
}