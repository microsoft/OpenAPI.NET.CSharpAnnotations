// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Runtime.Serialization;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Exceptions
{
    /// <summary>
    /// The exception that is recorded when the documentation for a security tag is invalid.
    /// </summary>
    [Serializable]
    public class InvalidSecurityTagException : DocumentationException
    {
        /// <summary>
        /// The default <see cref="InvalidSecurityTagException"/>.
        /// </summary>
        public InvalidSecurityTagException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidSecurityTagException"/>.
        /// </summary>
        public InvalidSecurityTagException(string message)
            : base( message )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidSecurityTagException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected InvalidSecurityTagException(SerializationInfo info, StreamingContext context)
            : base( info, context )
        {
        }
    }
}