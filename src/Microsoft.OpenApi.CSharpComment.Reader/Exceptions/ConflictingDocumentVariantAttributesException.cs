// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Runtime.Serialization;
using Microsoft.OpenApi.CSharpComment.Reader.Models;

namespace Microsoft.OpenApi.CSharpComment.Reader.Exceptions
{
    /// <summary>
    /// The exception that is thrown when different set of attributes and their values
    /// are given for a document variant.
    /// </summary>
    [Serializable]
    internal class ConflictingDocumentVariantAttributesException : DocumentationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConflictingDocumentVariantAttributesException"/> class.
        /// </summary>
        /// <param name="existingDocumentVariantInfo">The existing document variant info.</param>
        /// <param name="newDocumentVariantInfo">The new equivalent document variant info but with different attributes.</param>
        public ConflictingDocumentVariantAttributesException(
            DocumentVariantInfo existingDocumentVariantInfo,
            DocumentVariantInfo newDocumentVariantInfo)
            : base(
                string.Format(
                    SpecificationGenerationMessages.ConflictingDocumentVariantAttributes,
                    existingDocumentVariantInfo.Categorizer,
                    existingDocumentVariantInfo.Title,
                    existingDocumentVariantInfo.Attributes.ToString(),
                    newDocumentVariantInfo.Attributes.ToString()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConflictingDocumentVariantAttributesException"/> class
        /// with serialized data.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected ConflictingDocumentVariantAttributesException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}