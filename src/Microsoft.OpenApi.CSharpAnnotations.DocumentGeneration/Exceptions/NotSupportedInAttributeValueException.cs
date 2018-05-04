// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models.KnownStrings;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Exceptions
{
    /// <summary>
    /// The exception that is thrown when one or more params have "in" attribute value which is not supported.
    /// </summary>
    [Serializable]
    public class NotSupportedInAttributeValueException : DocumentationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingInAttributeException"/>.
        /// </summary>
        /// <param name="parameters">The parameters that have unsupported values for "in" attribute.</param>
        /// <param name="inValues">The provided unsupported values for "in" attribute.</param>
        public NotSupportedInAttributeValueException(IEnumerable<string> parameters, IEnumerable<string> inValues)
            : base(string.Format(
                SpecificationGenerationMessages.NotSupportedInAttributeValue,
                string.Join(", ", inValues),
                string.Join(", ", parameters),
                string.Join(", ", KnownXmlStrings.AllowedInValues)))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingInAttributeException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected NotSupportedInAttributeValueException( SerializationInfo info, StreamingContext context )
            : base( info, context )
        {
        }
    }
}