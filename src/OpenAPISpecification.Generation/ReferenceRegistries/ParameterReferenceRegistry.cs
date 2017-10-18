// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OpenApiSpecification.Core.Models;

namespace Microsoft.OpenApiSpecification.Generation.ReferenceRegistries
{
    /// <summary>
    /// Reference Registry for <see cref="Parameter"/>
    /// </summary>
    public class ParameterReferenceRegistry : ReferenceRegistry<object, Parameter>
    {
        private ExampleReferenceRegistry _exampleReferenceRegistry;
        private SchemaReferenceRegistry _schemaReferenceRegistry;

        /// <summary>
        /// Creates an instance of <see cref="ParameterReferenceRegistry"/> class.
        /// </summary>
        /// <param name="schemaReferenceRegistry">Reference registry for the Schema.</param>
        /// <param name="exampleReferenceRegistry">Reference registry for the Example.</param>
        public ParameterReferenceRegistry(
            SchemaReferenceRegistry schemaReferenceRegistry,
            ExampleReferenceRegistry exampleReferenceRegistry)
        {
            _schemaReferenceRegistry = schemaReferenceRegistry;
            _exampleReferenceRegistry = exampleReferenceRegistry;
        }

        /// <summary>
        /// The dictionary containing all references of the given type.
        /// </summary>
        public override IDictionary<string, Parameter> References { get; } = new Dictionary<string, Parameter>();

        /// <summary>
        /// Finds the existing reference object based on the key from the input or creates a new one.
        /// </summary>
        /// <returns>The existing or created reference object.</returns>
        internal override Parameter FindOrAddReference(object input)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the key from the input object to use as reference string.
        /// </summary>
        /// <remarks>This must match the regular expression ^[a-zA-Z0-9\.\-_]+$</remarks>
        protected override string GetKey(object input)
        {
            throw new NotImplementedException();
        }
    }
}