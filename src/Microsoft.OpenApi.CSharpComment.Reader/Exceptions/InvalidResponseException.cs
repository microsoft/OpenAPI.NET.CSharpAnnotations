// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Microsoft.OpenApi.CSharpComment.Reader.Exceptions
{
    /// <summary>
    /// The exception that is recorded when the documentation for a response is invalid.
    /// </summary>
    [Serializable]
    public class InvalidResponseException : DocumentationException
    {
        /// <summary>
        /// The default <see cref="InvalidResponseException"/>
        /// </summary>
        public InvalidResponseException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidResponseException"/>.
        /// </summary>
        public InvalidResponseException(string requestBody)
            : base(string.Format(SpecificationGenerationMessages.InvalidResponse, requestBody))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidResponseException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected InvalidResponseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}