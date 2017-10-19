// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Microsoft.OpenApi.CSharpComment.Reader.Exceptions
{
    /// <summary>
    /// The exception that is thrown when the URL contains the same parameter in both path and query.
    /// </summary>
    [Serializable]
    public class ConflictingPathAndQueryParametersException : DocumentationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConflictingPathAndQueryParametersException"/> class.
        /// </summary>
        /// <param name="paramName">The parameter name</param>
        /// <param name="url">The entire url</param>
        public ConflictingPathAndQueryParametersException(string paramName, string url)
            : base(string.Format(SpecificationGenerationMessages.ConflictingPathAndQueryParameters, paramName, url))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConflictingPathAndQueryParametersException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected ConflictingPathAndQueryParametersException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}