// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Runtime.Serialization;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Exceptions
{
    /// <summary>
    /// The exception that is recorded when an error occurs while adding a schema to the schema reference registry.
    /// </summary>
    [Serializable]
    internal class AddingSchemaReferenceFailedException : DocumentationException
    {
        /// <summary>
        /// The default <see cref="AddingSchemaReferenceFailedException"/>.
        /// </summary>
        public AddingSchemaReferenceFailedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddingSchemaReferenceFailedException"/>.
        /// </summary>
        public AddingSchemaReferenceFailedException(string schema, string error)
            : base(string.Format(SpecificationGenerationMessages.AddingSchemaReferenceFailed, schema, error))
        {
            Schema = schema;
            Error = error;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddingSchemaReferenceFailedException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected AddingSchemaReferenceFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// The schema name causing an error.
        /// </summary>
        public string Schema { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public string Error { get; }
    }
}