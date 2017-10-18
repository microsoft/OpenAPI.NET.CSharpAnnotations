// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Microsoft.OpenApiSpecification.Generation.Exceptions
{
    /// <summary>
    /// The exception that is recorded when a URL is missing or has invalid format.
    /// </summary>
    [Serializable]
    public class InvalidUrlException : DocumentationException
    {
        /// <summary>
        /// The default <see cref="InvalidUrlException"/>.
        /// </summary>
        public InvalidUrlException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidUrlException"/>.
        /// </summary>
        /// <param name="url">The invalid url.</param>
        public InvalidUrlException(string url, string message)
            : base(string.Format(SpecificationGenerationMessages.InvalidUrl, url, message))
        {
            Url = url;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidUrlException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected InvalidUrlException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// The invalid url.
        /// </summary>
        public string Url { get; }
    }
}