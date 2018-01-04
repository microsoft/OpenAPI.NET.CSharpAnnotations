// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.OpenApi.CSharpComment.Reader.Exceptions
{
    /// <summary>
    /// The exception that is thrown when one or more params are missing in attribute and the best effort
    /// to populate it using other context clues failed.
    /// </summary>
    [Serializable]
    internal class MissingInAttributeException : DocumentationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingInAttributeException"/> class.
        /// </summary>
        /// <param name="parameters">The parameters that miss in attributes.</param>
        public MissingInAttributeException(IList<string> parameters)
            : base(string.Format(SpecificationGenerationMessages.MissingInAttribute, string.Join(", ", parameters)))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingInAttributeException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected MissingInAttributeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}