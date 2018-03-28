// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Runtime.Serialization;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Exceptions
{
    /// <summary>
    /// The exception that is thrown when there is an undocumented path parameter in the xml documentation.
    /// </summary>
    [Serializable]
    internal class UndocumentedPathParameterException : DocumentationException
    {
        /// <summary>
        /// The default undocumented path parameter exception.
        /// </summary>
        public UndocumentedPathParameterException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the UndocumentedPathParameterException class given the
        /// undocumented parameter and the operation path which is missing it.
        /// </summary>
        /// <param name="param">The missing parameter.</param>
        /// <param name="path">The operation which is missing parameter.</param>
        public UndocumentedPathParameterException(string param, string path)
            : base(string.Format(SpecificationGenerationMessages.UndocumentedPathParameter, param, path))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UndocumentedPathParameterException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected UndocumentedPathParameterException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}