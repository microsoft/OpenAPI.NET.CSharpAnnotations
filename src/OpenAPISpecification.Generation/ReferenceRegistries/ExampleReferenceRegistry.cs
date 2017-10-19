// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OpenApiSpecification.Core.Models;

namespace Microsoft.OpenApi.CSharpComment.Reader.ReferenceRegistries
{
    /// <summary>
    /// Reference Registry for <see cref="Example"/>
    /// </summary>
    public class ExampleReferenceRegistry : ReferenceRegistry<object, Example>
    {
        /// <summary>
        /// The dictionary containing all references of the given type.
        /// </summary>
        public override IDictionary<string, Example> References { get; } = new Dictionary<string, Example>();

        /// <summary>
        /// Finds the existing reference object based on the key from the input or creates a new one.
        /// </summary>
        /// <returns>The existing or created reference object.</returns>
        internal override Example FindOrAddReference(object input)
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