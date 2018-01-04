// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Runtime.Serialization;

namespace Microsoft.OpenApi.CSharpComment.Reader.Exceptions
{
    /// <summary>
    /// The exception that is recorded when the documentation for a request body is invalid.
    /// </summary>
    [Serializable]
    internal class InvalidRequestBodyException : DocumentationException
    {
        /// <summary>
        /// The default <see cref="InvalidRequestBodyException"/>.
        /// </summary>
        public InvalidRequestBodyException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRequestBodyException"/>.
        /// </summary>
        public InvalidRequestBodyException(string requestBody)
            : base(string.Format(SpecificationGenerationMessages.InvalidRequestBody, requestBody))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRequestBodyException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected InvalidRequestBodyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}