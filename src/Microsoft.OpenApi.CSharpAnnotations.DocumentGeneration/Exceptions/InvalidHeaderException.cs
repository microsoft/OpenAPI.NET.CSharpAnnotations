// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Runtime.Serialization;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Exceptions
{
    /// <summary>
    /// The exception that is recorded when the documentation for a header tag is invalid.
    /// </summary>
    [Serializable]
    internal class InvalidHeaderException : DocumentationException
    {
        /// <summary>
        /// The default <see cref="InvalidHeaderException"/>.
        /// </summary>
        public InvalidHeaderException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidHeaderException"/>.
        /// </summary>
        public InvalidHeaderException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidHeaderException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected InvalidHeaderException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
