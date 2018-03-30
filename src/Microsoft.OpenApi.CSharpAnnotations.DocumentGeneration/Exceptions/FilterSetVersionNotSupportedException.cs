// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Runtime.Serialization;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Exceptions
{
    /// <summary>
    /// The exception that is recorded when the provided filter set version is not supported.
    /// </summary>
    [Serializable]
    public class FilterSetVersionNotSupportedException : Exception
    {
        /// <summary>
        /// The default <see cref="FilterSetVersionNotSupportedException"/>.
        /// </summary>
        public FilterSetVersionNotSupportedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterSetVersionNotSupportedException"/>.
        /// </summary>
        public FilterSetVersionNotSupportedException(string filterSetVersion)
            : base(string.Format(SpecificationGenerationMessages.FilterSetVersionNotSupported, filterSetVersion))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterSetVersionNotSupportedException"/>
        /// class with serialized data.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected FilterSetVersionNotSupportedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}