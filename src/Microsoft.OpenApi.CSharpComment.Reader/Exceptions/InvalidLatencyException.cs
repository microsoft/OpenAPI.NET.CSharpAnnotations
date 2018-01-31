// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Runtime.Serialization;

namespace Microsoft.OpenApi.CSharpComment.Reader.Exceptions
{
    /// <summary>
    /// The exception that is recorded when the latency given in invalid.
    /// </summary>
    [Serializable]
    internal class InvalidLatencyException : DocumentationException
    {
        /// <summary>
        /// The default <see cref="InvalidLatencyException"/>.
        /// </summary>
        public InvalidLatencyException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidLatencyException"/>.
        /// </summary>
        public InvalidLatencyException(string latency)
            : base(string.Format(SpecificationGenerationMessages.InvalidRequestBody, latency))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidLatencyException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected InvalidLatencyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}