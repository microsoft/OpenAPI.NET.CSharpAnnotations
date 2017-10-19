// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Microsoft.OpenApi.CSharpComment.Reader.Exceptions
{
    /// <summary>
    /// The exception that is thrown when there is a mistake in the xml documentation
    /// </summary>
    [Serializable]
    public class DocumentationException : Exception
    {
        /// <summary>
        /// The default documentation exception.
        /// </summary>
        public DocumentationException()
        {
        }

        /// <summary>
        /// The custom documentation exception.
        /// </summary>
        /// <param name="message">the Error message.</param>
        public DocumentationException(string message) : base(message)
        {
        }

        /// <summary>
        /// The custom documentation exception with inner exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="inner">The inner exception.</param>
        public DocumentationException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentationException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected DocumentationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}