// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.CSharpComment.Reader.Models;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpComment.Reader.Tests
{
    /// <summary>
    /// Extension methods for <see cref="IDictionary{TKey,TValue}"/>.
    /// </summary>
    internal static class DictionaryExtensions
    {
        /// <summary>
        /// Converts <see cref="Dictionary{TKey,TValue}"/>where TKey is <see cref="DocumentVariantInfo"/>
        /// and TValue is <see cref="OpenApiDocument"/>
        /// To <see cref="Dictionary{TKey,TValue}"/>where TKey is <see cref="DocumentVariantInfo"/>
        /// and TValue is <see cref="string"/>.
        /// </summary>
        /// <param name="sourceDictionary">The dictionary of OpenAPI documents.</param>
        /// <param name="openApiSpecVersion">The OpenAPI spec version to serialize to.</param>
        /// <param name="openApiFormat">The OpenAPI format to serialize to.</param>
        /// <returns>Dictionary mapping document variant metadata to their respective serialized OpenAPI document.
        /// </returns>
        public static IDictionary<DocumentVariantInfo, string> ToSerializedOpenApiDocuments(
            this IDictionary<DocumentVariantInfo, OpenApiDocument> sourceDictionary,
            OpenApiSpecVersion openApiSpecVersion = OpenApiSpecVersion.OpenApi3_0,
            OpenApiFormat openApiFormat = OpenApiFormat.Json)
        {
            if (sourceDictionary == null)
            {
                throw new ArgumentNullException(nameof(sourceDictionary));
            }

            IDictionary<DocumentVariantInfo, string> openApiDocuments =
                new Dictionary<DocumentVariantInfo, string>();

            foreach (var variantInfoDocumentKeyValuePair in sourceDictionary)
            {
                openApiDocuments[new DocumentVariantInfo(variantInfoDocumentKeyValuePair.Key)]
                    = variantInfoDocumentKeyValuePair.Value.Serialize(openApiSpecVersion, openApiFormat);
            }

            return openApiDocuments;
        }
    }
}