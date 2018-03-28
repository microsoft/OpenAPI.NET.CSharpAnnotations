// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Runtime.Serialization;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Exceptions
{
    /// <summary>
    /// The exception that is recorded when unable to generate documentation for all the operations.
    /// </summary>
    [Serializable]
    public class UnableToGenerateAllOperationsException : DocumentationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnableToGenerateAllOperationsException"/> class.
        /// </summary>
        /// <param name="successOperations">The number of operations for which generation succeeded.</param>
        /// <param name="totalOperations">The total number of operations documented.</param>
        public UnableToGenerateAllOperationsException(int successOperations, int totalOperations)
            : base(string.Format(
                SpecificationGenerationMessages.UnableToGenerateAllOperations,
                successOperations,
                totalOperations))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnableToGenerateAllOperationsException"/>
        /// class with serialized data.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected UnableToGenerateAllOperationsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}