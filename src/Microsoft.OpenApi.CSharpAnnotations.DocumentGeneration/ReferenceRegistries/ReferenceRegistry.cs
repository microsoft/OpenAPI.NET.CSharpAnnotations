// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.ReferenceRegistries
{
    /// <summary>
    /// Reference Registry for an <see cref="IOpenApiReferenceable"/> class.
    /// </summary>
    public abstract class ReferenceRegistry<TInput, TOutput>
        where TOutput : IOpenApiReferenceable
    {
        /// <summary>
        /// The dictionary containing all references of the given type.
        /// </summary>
        public abstract IDictionary<string, TOutput> References { get; }

        /// <summary>
        /// Finds the existing reference object based on the key from the input or creates a new one.
        /// </summary>
        /// <returns>The existing or created reference object.</returns>
        internal abstract TOutput FindOrAddReference(TInput input);

        /// <summary>
        /// Gets the key from the input object to use as reference string.
        /// </summary>
        /// <remarks>This must match the regular expression ^[a-zA-Z0-9\.\-_]+$</remarks>
        internal abstract string GetKey(TInput input);
    }
}