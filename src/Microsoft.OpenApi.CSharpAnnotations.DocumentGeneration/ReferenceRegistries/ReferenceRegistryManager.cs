// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Xml.Linq;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.ReferenceRegistries
{
    /// <summary>
    /// A class encapsulating reference registries for all <see cref="IOpenApiReferenceable"/> types.
    /// </summary>
    public class ReferenceRegistryManager
    {
        /// <summary>
        /// Creates an instance of <see cref="ReferenceRegistryManager"/> class.
        /// <param name="openApiDocumentGenerationSettings">The Open API document generation settings.</param>
        /// </summary>
        public ReferenceRegistryManager(OpenApiDocumentGenerationSettings openApiDocumentGenerationSettings)
        {
            if (openApiDocumentGenerationSettings == null)
            {
                throw new ArgumentNullException(nameof(openApiDocumentGenerationSettings));
            }

            SchemaReferenceRegistry = new SchemaReferenceRegistry(
                openApiDocumentGenerationSettings.SchemaGenerationSettings);
            ExampleReferenceRegistry = new ExampleReferenceRegistry();
            ParameterReferenceRegistry = new ParameterReferenceRegistry(
                SchemaReferenceRegistry,
                ExampleReferenceRegistry);
            SecuritySchemeReferenceRegistry = new SecuritySchemeReferenceRegistry();
        }

        /// <summary>
        /// Reference registry for the <see cref="OpenApiExample"/> class.
        /// </summary>
        public ExampleReferenceRegistry ExampleReferenceRegistry { get; }

        /// <summary>
        /// Reference registry for the <see cref="OpenApiParameter"/> class.
        /// </summary>
        public ParameterReferenceRegistry ParameterReferenceRegistry { get; }

        /// <summary>
        /// Reference registry for the <see cref="OpenApiSchema"/> class.
        /// </summary>
        public SchemaReferenceRegistry SchemaReferenceRegistry { get; }

        /// <summary>
        /// Reference registry for the <see cref="OpenApiSecurityScheme"/> class.
        /// </summary>
        public SecuritySchemeReferenceRegistry SecuritySchemeReferenceRegistry { get; }

        /// <summary>
        /// Finds an existing reference of an <see cref="OpenApiExample"/> class or creates a new one.
        /// </summary>
        public OpenApiExample FindOrAddExampleReference(object input)
        {
            return ExampleReferenceRegistry.FindOrAddReference(input);
        }

        /// <summary>
        /// Finds an existing reference of an <see cref="OpenApiParameter"/> class or creates a new one.
        /// </summary>
        public OpenApiParameter FindOrAddParameterReference(object input)
        {
            return ParameterReferenceRegistry.FindOrAddReference(input);
        }

        /// <summary>
        /// Finds an existing reference of an <see cref="OpenApiSchema"/> class or creates a new one.
        /// </summary>
        public OpenApiSchema FindOrAddSchemaReference(Type input)
        {
            return SchemaReferenceRegistry.FindOrAddReference(input);
        }

        /// <summary>
        /// Finds an existing reference of an <see cref="OpenApiSecurityScheme"/> class or creates a new one.
        /// </summary>
        public OpenApiSecurityScheme FindOrAddSecuritySchemeReference(XElement input)
        {
            return SecuritySchemeReferenceRegistry.FindOrAddReference(input);
        }
    }
}