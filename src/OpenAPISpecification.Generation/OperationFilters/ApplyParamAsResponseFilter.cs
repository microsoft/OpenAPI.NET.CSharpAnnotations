// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.OpenApiSpecification.Core.Models;
using Microsoft.OpenApiSpecification.Generation.Extensions;

namespace Microsoft.OpenApiSpecification.Generation.OperationFilters
{
    /// <summary>
    /// Parses the value of response tag in xml documentation and apply that as response in operation.
    /// </summary>
    public class ApplyParamAsResponseFilter : IOperationFilter
    {
        /// <summary>
        /// Fetches the value of "response" tags from xml documentation and populates operation's response.
        /// </summary>
        /// <param name="operation">The operation to be updated.</param>
        /// <param name="element">The xml element representing an operation in the annotation xml.</param>
        /// <param name="settings">The operation filter settings.</param>
        public void Apply(Operation operation, XElement element, OperationFilterSettings settings)
        {
            var responseElements = element.Elements().Where(p => p.Name == "response");

            foreach (var responseElement in responseElements)
            {
                // Fetch response code
                var code = responseElement.Attribute("code")?.Value;

                if (code == null || operation.Responses.ContainsKey(code))
                {
                    continue;
                }

                var childNodes = responseElement.DescendantNodes().ToList();
                var description = string.Empty;

                var lastNode = childNodes.LastOrDefault();

                if (lastNode != null && lastNode.NodeType == XmlNodeType.Text)
                {
                    description = lastNode.ToString();
                }

                var allListedTypes = new List<string>();

                var seeNodes = responseElement.Descendants("see");

                foreach (var node in seeNodes)
                {
                    var crefValue = node.Attribute("cref")?.Value;

                    if (crefValue != null)
                    {
                        allListedTypes.Add(crefValue);
                    }
                }
                
                var responseContractType = settings.TypeFetcher.GetTypeFromCrefValues(allListedTypes);
                
                var schema = settings.ReferenceRegistryManager.SchemaReferenceRegistry.FindOrAddReference(
                    responseContractType);

                var response = new Response
                {
                    Description = description.RemoveBlankLines(),
                    Content =
                    {
                        new KeyValuePair<string, MediaType>("application/json",
                            new MediaType {Schema = schema})
                    }
                };

                operation.Responses.Add(code, response);
            }

            if (!operation.Responses.Any())
            {
                operation.Responses.Add("default", new Response {Description = "Unexpected Error!"});
            }
        }
    }
}