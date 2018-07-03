// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Runtime.Serialization;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Exceptions
{
    /// <summary>
    /// The exception that is recorded when example tag is documentated incorrectly.
    /// </summary>
    [Serializable]
    internal class InvalidExampleException : DocumentationException
    {
        /// <summary>
        /// The default <see cref="InvalidExampleException"/>.
        /// </summary>
        public InvalidExampleException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidExampleException"/>.
        /// </summary>
        public InvalidExampleException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidExampleException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected InvalidExampleException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}