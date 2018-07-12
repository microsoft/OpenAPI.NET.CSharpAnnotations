// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Exceptions;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Extensions;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models.KnownStrings;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.ReferenceRegistries;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.OperationFilters
{
    /// <summary>
    /// Parses the value of param tag in xml documentation and apply that as request body in operation.
    /// </summary>
    public class ParamToRequestBodyFilter : IOperationFilter
    {
        /// <summary>
        /// Fetches the value of "param" tags from xml documentation with in valus of "body"
        /// and populates operation's request body.
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
            var bodyElements = element.Elements()
                .Where(
                    p => p.Name == KnownXmlStrings.Param &&
                        p.Attribute(KnownXmlStrings.In)?.Value == KnownXmlStrings.Body)
                .ToList();

            SchemaReferenceRegistry schemaReferenceRegistry = settings.ReferenceRegistryManager.SchemaReferenceRegistry;

            foreach (var bodyElement in bodyElements)
            {
                var name = bodyElement.Attribute(KnownXmlStrings.Name)?.Value.Trim();
                var mediaType = bodyElement.Attribute(KnownXmlStrings.Type)?.Value ?? "application/json";

                var description = bodyElement.GetDescriptionTextFromLastTextNode();

                var allListedTypes = bodyElement.GetListedTypes();

                if (!allListedTypes.Any())
                {
                    throw new InvalidRequestBodyException(
                        string.Format(SpecificationGenerationMessages.MissingSeeCrefTag, name));
                }

                var type = settings.TypeFetcher.LoadTypeFromCrefValues(allListedTypes);
                var schema = schemaReferenceRegistry.FindOrAddReference(type);

                var examples = bodyElement.ToOpenApiExamples(settings.TypeFetcher);

                if (examples.Count > 0)
                {
                    var firstExample = examples.First().Value?.Value;

                    if (firstExample != null)
                    {
                        // In case a schema is a reference, find that schmea object in schema registry
                        // and update the example.
                        if (schema.Reference != null)
                        {
                            var key = schemaReferenceRegistry.GetKey(type);

                            if (schemaReferenceRegistry.References.ContainsKey(key))
                            {
                                settings.ReferenceRegistryManager.SchemaReferenceRegistry.References[key].Example
                                    = firstExample;
                            }
                        }
                        else
                        {
                            schema.Example = firstExample;
                        }
                    }
                }

                if (operation.RequestBody == null)
                {
                    operation.RequestBody = new OpenApiRequestBody
                    {
                        Description = description.RemoveBlankLines(),
                        Content =
                        {
                            [mediaType] = new OpenApiMediaType {Schema = schema}
                        },
                        Required = true
                    };
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(operation.RequestBody.Description))
                    {
                        operation.RequestBody.Description = description.RemoveBlankLines();
                    }

                    if (!operation.RequestBody.Content.ContainsKey(mediaType))
                    {
                        operation.RequestBody.Content[mediaType] = new OpenApiMediaType
                        {
                            Schema = schema
                        };
                    }
                    else
                    {
                        if (!operation.RequestBody.Content[mediaType].Schema.AnyOf.Any())
                        {
                            var existingSchema = operation.RequestBody.Content[mediaType].Schema;
                            var newSchema = new OpenApiSchema();
                            newSchema.AnyOf.Add(existingSchema);

                            operation.RequestBody.Content[mediaType].Schema = newSchema;
                        }

                        operation.RequestBody.Content[mediaType].Schema.AnyOf.Add(schema);
                    }
                }

                if (examples.Count > 0)
                {
                    if (operation.RequestBody.Content[mediaType].Examples.Any())
                    {
                        examples.CopyInto(operation.RequestBody.Content[mediaType].Examples);
                    }
                    else
                    {
                        operation.RequestBody.Content[mediaType].Examples = examples;
                    }
                }
            }
        }
    }
}