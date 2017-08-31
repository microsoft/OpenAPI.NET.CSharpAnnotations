// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Newtonsoft.Json;

namespace Microsoft.OpenApiSpecification.Generation.Models
{
    /// <summary>
    /// Class encapsulating information for a variant of a document.
    /// </summary>
    public class DocumentVariantInfo
    {
        private static readonly DocumentVariantInfo _defaultSpecificationDocumentVariantInfo
            = new DocumentVariantInfo();

        /// <summary>
        /// Gets or sets the categorizer used to fork the document.
        /// </summary>
        [JsonProperty]
        public string Categorizer { get; set; }

        /// <summary>
        /// Gets the default value of this object.
        /// </summary>
        public static DocumentVariantInfo Default { get; }
            = _defaultSpecificationDocumentVariantInfo;

        /// <summary>
        /// Gets or sets the title of this document variant.
        /// </summary>
        [JsonProperty]
        public string Title { get; set; }

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
        public override int GetHashCode() => new {Title, Categorizer}.GetHashCode();

        /// <summary>
        /// Gets the string representation.
        /// </summary>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}