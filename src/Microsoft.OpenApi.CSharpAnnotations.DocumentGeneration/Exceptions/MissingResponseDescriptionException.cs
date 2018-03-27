// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Runtime.Serialization;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Exceptions
{
    /// <summary>
    /// The exception that is thrown when the documentation for a response is missing description.
    /// </summary>
    [Serializable]
    public class MissingResponseDescriptionException : DocumentationException
    {
        /// <summary>
        /// The default <see cref="MissingResponseDescriptionException"/>.
        /// </summary>
        public MissingResponseDescriptionException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingResponseDescriptionException"/>.
        /// </summary>
        public MissingResponseDescriptionException(string responseCode)
            : base(string.Format(SpecificationGenerationMessages.MissingResponseDescription, responseCode))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingResponseDescriptionException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected MissingResponseDescriptionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}