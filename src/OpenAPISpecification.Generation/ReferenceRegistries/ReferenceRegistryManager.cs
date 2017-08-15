// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.OpenApiSpecification.Core.Models;

namespace Microsoft.OpenApiSpecification.Generation.ReferenceRegistries
{
    /// <summary>
    /// A class encapsulating reference registries for all <see cref="IReferenceable"/> types.
    /// </summary>
    public class ReferenceRegistryManager
    {
        /// <summary>
        /// Creates an instance of <see cref="ReferenceRegistryManager"/> class.
        /// </summary>
        public ReferenceRegistryManager()
        {
            SchemaReferenceRegistry = new SchemaReferenceRegistry();
            ExampleReferenceRegistry = new ExampleReferenceRegistry();
            ParameterReferenceRegistry = new ParameterReferenceRegistry(
                SchemaReferenceRegistry,
                ExampleReferenceRegistry);
        }

        /// <summary>
        /// Reference registry for the <see cref="Example"/> class.
        /// </summary>
        public ExampleReferenceRegistry ExampleReferenceRegistry { get; }

        /// <summary>
        /// Reference registry for the <see cref="Parameter"/> class.
        /// </summary>
        public ParameterReferenceRegistry ParameterReferenceRegistry { get; }

        /// <summary>
        /// Reference registry for the <see cref="Schema"/> class.
        /// </summary>
        public SchemaReferenceRegistry SchemaReferenceRegistry { get; }

        /// <summary>
        /// Finds an existing reference of an <see cref="Example"/> class or creates a new one.
        /// </summary>
        public Example FindOrAddExampleReference(object input)
        {
            return ExampleReferenceRegistry.FindOrAddReference(input);
        }

        /// <summary>
        /// Finds an existing reference of an <see cref="Parameter"/> class or creates a new one.
        /// </summary>
        public Parameter FindOrAddParameterReference(object input)
        {
            return ParameterReferenceRegistry.FindOrAddReference(input);
        }

        /// <summary>
        /// Finds an existing reference of an <see cref="Schema"/> class or creates a new one.
        /// </summary>
        public Schema FindOrAddSchemaReference(Type input)
        {
            return SchemaReferenceRegistry.FindOrAddReference(input);
        }
    }
}