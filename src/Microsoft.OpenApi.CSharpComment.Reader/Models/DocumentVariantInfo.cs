// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.CSharpComment.Reader.Extensions;

namespace Microsoft.OpenApi.CSharpComment.Reader.Models
{
    /// <summary>
    /// Class encapsulating information for a variant of a document.
    /// </summary>
    public class DocumentVariantInfo
    {
        private static readonly DocumentVariantInfo _defaultSpecificationDocumentVariantInfo
            = new DocumentVariantInfo();

        /// <summary>
        /// Initializes the <see cref="DocumentVariantInfo"/> object.
        /// </summary>
        public DocumentVariantInfo()
        {
        }

        /// <summary>
        /// Initializes the <see cref="DocumentVariantInfo"/> object.
        /// </summary>
        /// <param name="other">Other object to copy from.</param>
        public DocumentVariantInfo(DocumentVariantInfo other)
        {
            foreach (var attribute in other.Attributes)
            {
                Attributes[attribute.Key] = attribute.Value;
            }

            Categorizer = other.Categorizer;
            Title = other.Title;
        }

        /// <summary>
        /// Gets or sets the other attributes related to this document variant.
        /// </summary>
        public IDictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets the categorizer used to fork the document.
        /// </summary>
        public string Categorizer { get; set; }

        /// <summary>
        /// Gets the default value of this object.
        /// </summary>
        public static DocumentVariantInfo Default { get; }
            = _defaultSpecificationDocumentVariantInfo;

        /// <summary>
        /// Gets or sets the title of this document variant.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Determines whether this <see cref="Attributes"/> are equivalent to the <see cref="Attributes"/>
        /// in the other document variant info.
        /// </summary>
        public bool AreAttributesEquivalent(DocumentVariantInfo documentVariantInfo)
        {
            return Attributes.EquivalentTo(documentVariantInfo.Attributes);
        }

        /// <summary>
        /// Determines whether this object is equal to the given object.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as DocumentVariantInfo);
        }

        /// <summary>
        /// Determines whether this object is equal to the given <see cref="DocumentVariantInfo"/>.
        /// </summary>
        public bool Equals(DocumentVariantInfo specificationDocumentVariantInfo)
        {
            return specificationDocumentVariantInfo != null &&
                specificationDocumentVariantInfo.Categorizer == Categorizer &&
                specificationDocumentVariantInfo.Title == Title;
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        public override int GetHashCode() => new {Title}.GetHashCode();
    }
}