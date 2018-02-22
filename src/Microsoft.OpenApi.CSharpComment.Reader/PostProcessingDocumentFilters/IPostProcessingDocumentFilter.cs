// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.CSharpComment.Reader.Models;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpComment.Reader.PostProcessingDocumentFilters
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPostProcessingDocumentFilter
    {
        /// <summary>
        /// Applies the filter to post process the <see cref="OpenApiDocument"/> objects in
        /// <see cref="IDictionary{DocumentVariantInfo, OpenApiDocument}"/>.
        /// </summary>
        /// <param name="specificationDocuments"></param>
        /// <param name="settings"></param>
        void Apply(
            IDictionary<DocumentVariantInfo, OpenApiDocument> specificationDocuments,
            PostProcessingDocumentFilterSettings settings);
    }
}