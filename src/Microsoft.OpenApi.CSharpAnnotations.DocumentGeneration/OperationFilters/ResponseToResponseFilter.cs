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
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models.KnownStrings;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.ReferenceRegistries;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.OperationFilters
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
        /// <returns>The list of generation errors, if any produced when processing the filter."></returns>
        /// <remarks>
        /// Care should be taken to not overwrite the existing value in Operation if already present.
        /// This guarantees the predictable behavior that the first tag in the XML will be respected.
        /// It also guarantees that common annotations in the config file do not overwrite the
        /// annotations in the main documentation.
        /// </remarks>
        public IList<GenerationError> Apply(
            OpenApiOperation operation,
            XElement element,
            OperationFilterSettings settings)
        {
            var generationErrors = new List<GenerationError>();

            try
            {
                var responseElements = element.Elements()
                    .Where(
                        p => p.Name == KnownXmlStrings.Response ||
                            p.Name == KnownXmlStrings.ResponseType);

                var schemaTypeInfo = settings.SchemaTypeInfo;

                foreach (var responseElement in responseElements)
                {
                    var code = responseElement.Attribute(KnownXmlStrings.Code)?.Value;

                    if (string.IsNullOrWhiteSpace(code))
                    {
                        // Most APIs only document responses for a successful operation, so if code is not specified, 
                        // we will assume it is for a successful operation. This also allows us to comply with OpenAPI spec:
                        //     The Responses Object MUST contain at least one response code, 
                        //     and it SHOULD be the response for a successful operation call.
                        code = "200";
                    }

                    var mediaType = responseElement.Attribute(KnownXmlStrings.Type)?.Value ?? "application/json";

                    var description = responseElement.GetDescriptionTextFromLastTextNode();

                    var allListedTypes = responseElement.GetListedTypes();

                    var crefKey = allListedTypes.GetCrefKey();

                    OpenApiSchema schema = null;
                    if (schemaTypeInfo.CrefToSchemaMap.ContainsKey(crefKey))
                    {
                        var schemaInfo = schemaTypeInfo.CrefToSchemaMap[crefKey];

                        if (schemaInfo.error.ExceptionType != null)
                        {
                            generationErrors.Add(schemaInfo.error);

                            return generationErrors;
                        }

                        schema = new OpenApiStringReader().ReadFragment<OpenApiSchema>(
                            schemaInfo.schema,
                            OpenApiSpecVersion.OpenApi3_0,
                            out OpenApiDiagnostic diagnostic);
                    }

                    var examples = responseElement.ToOpenApiExamples(settings.SchemaTypeInfo.CrefToFieldValueMap);

                    var headers = responseElement.ToOpenApiHeaders(schemaTypeInfo.CrefToSchemaMap, generationErrors);

                    var schemaReferenceDefaultVariant = schemaTypeInfo.VariantSchemaReferenceMap[DocumentVariantInfo.Default];

                    if (schema != null)
                    {
                        if (examples.Count > 0)
                        {
                            var firstExample = examples.First().Value?.Value;

                            if (firstExample != null)
                            {
                                if (schema.Reference != null)
                                {
                                    if (schemaReferenceDefaultVariant.ContainsKey(schema.Reference.Id))
                                    {
                                        schema = new OpenApiStringReader().ReadFragment<OpenApiSchema>(
                                            schemaReferenceDefaultVariant[schema.Reference.Id],
                                            OpenApiSpecVersion.OpenApi3_0,
                                            out var _);

                                        schema.Example = firstExample;

                                        schemaReferenceDefaultVariant[schema.Reference.Id] =
                                            schema.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
                                    }
                                }
                                else
                                {
                                    schema.Example = firstExample;
                                }
                            }
                        }
                    }

                    if (operation.Responses.ContainsKey(code))
                    {
                        if (string.IsNullOrWhiteSpace(operation.Responses[code].Description))
                        {
                            operation.Responses[code].Description = description.RemoveBlankLines();
                        }

                        if (schema != null)
                        {
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
                    }
                    else
                    {
                        var response = new OpenApiResponse
                        {
                            Description = description.RemoveBlankLines(),
                        };

                        if (schema != null)
                        {
                            response.Content[mediaType] = new OpenApiMediaType { Schema = schema };
                        }

                        if (headers != null && headers.Any())
                        {
                            response.Headers = headers;
                        }

                        operation.Responses.Add(code, response);
                    }

                    if (examples.Count > 0)
                    {
                        if (operation.Responses[code].Content[mediaType].Examples.Any())
                        {
                            examples.CopyInto(operation.Responses[code].Content[mediaType].Examples);
                        }
                        else
                        {
                            operation.Responses[code].Content[mediaType].Examples = examples;
                        }
                    }
                }

                if (!operation.Responses.Any())
                {
                    operation.Responses.Add(
                        "default",
                        new OpenApiResponse { Description = "Responses cannot be located for this operation." });
                }
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