// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Microsoft.OpenApi.CSharpComment.Reader.Exceptions
{
    /// <summary>
    /// The exception that is recorded when a verb that is missing or 
    /// cannot be parsed into a recognized HTTP operation method.
    /// </summary>
    [Serializable]
    internal class InvalidVerbException : DocumentationException
    {
        /// <summary>
        /// The default <see cref="InvalidVerbException"/>.
        /// </summary>
        public InvalidVerbException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidVerbException"/>.
        /// </summary>
        /// <param name="verb">The invalid verb.</param>
        public InvalidVerbException(string verb)
            : base(string.Format(SpecificationGenerationMessages.InvalidHttpMethod, verb))
        {
            Verb = verb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidVerbException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected InvalidVerbException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// The invalid verb that cannot be parsed into an operation method.
        /// </summary>
        public string Verb { get; }
    }
}