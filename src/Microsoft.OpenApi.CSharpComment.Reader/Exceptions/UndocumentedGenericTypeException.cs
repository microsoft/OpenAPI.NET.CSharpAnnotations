// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Microsoft.OpenApi.CSharpComment.Reader.Exceptions
{
    /// <summary>
    /// The exception that is thrown when there is an undocumented generic type in the xml documentation.
    /// </summary>
    [Serializable]
    public class UndocumentedGenericTypeException : DocumentationException
    {
        /// <summary>
        /// Default undocumented generic type exception.
        /// </summary>
        public UndocumentedGenericTypeException()
            : this(SpecificationGenerationMessages.UndocumentedGenericType)
        {
        }

        /// <summary>
        /// Custom undocumented generic type exception.
        /// </summary>
        /// <param name="message">Error message.</param>
        public UndocumentedGenericTypeException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UndocumentedGenericTypeException"/> class with serialized data.
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Streaming context.</param>
        protected UndocumentedGenericTypeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}