// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.CSharpComment.Reader.Models;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

namespace Microsoft.OpenApi.CSharpComment.Reader.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IDictionary{TKey,TValue}"/>.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Copies entries from the source dictionary into the target dictionary.
        /// The existing entry in the target dictionary will not be overwritten.
        /// </summary>        
        /// <typeparam name="TKey">Type of key.</typeparam>
        /// <typeparam name="TValue">Type of value.</typeparam>
        /// <param name="sourceDictionary">The source dictionary.</param>
        /// <param name="targetDictionary">The target dictionary.</param>
        public static void CopyInto<TKey, TValue>(
            this IDictionary<TKey, TValue> sourceDictionary,
            IDictionary<TKey, TValue> targetDictionary)
        {
            if (targetDictionary == null)
            {
                throw new ArgumentNullException(nameof(targetDictionary));
            }

            if (sourceDictionary == null)
            {
                throw new ArgumentNullException(nameof(sourceDictionary));
            }

            foreach (var key in sourceDictionary.Keys)
            {
                if (!targetDictionary.ContainsKey(key))
                {
                    targetDictionary.Add(key, sourceDictionary[key]);
                }
            }
        }

        /// <summary>
        /// Compares whether the two dictionary are "equivalent", meaning that the two have
        /// equal keys and values based on their definitions of Equals.
        /// </summary>
        /// <typeparam name="TKey">Type of key.</typeparam>
        /// <typeparam name="TValue">Type of value.</typeparam>
        /// <param name="sourceDictionary">The source dictionary.</param>
        /// <param name="targetDictionary">The target dictionary.</param>
        /// <returns>True if both dictionaries are null or both dictionary contains the same keys and values based on
        /// their definition of Equals. False otherwise.</returns>
        public static bool EquivalentTo<TKey, TValue>(
            this IDictionary<TKey, TValue> sourceDictionary,
            IDictionary<TKey, TValue> targetDictionary)
        {
            if (targetDictionary == null && sourceDictionary == null)
            {
                return true;
            }

            if (targetDictionary == null)
            {
                throw new ArgumentNullException(nameof(targetDictionary));
            }

            if (sourceDictionary == null)
            {
                throw new ArgumentNullException(nameof(sourceDictionary));
            }

            return sourceDictionary.Count == targetDictionary.Count &&
                !sourceDictionary.Except(targetDictionary).Any();
        }

        /// <summary>
        /// Converts dictionary of serialized open api documents to dictionary of open api documents.</summary>
        /// <param name="sourceDictionary">The serialized open api documents dictionary.</param>
        /// <returns><see cref="IDictionary{DocumentVariantInfo,OpenApiDocument}"/></returns>
        public static IDictionary<DocumentVariantInfo, OpenApiDocument> ToOpenApiDocuments(
            this IDictionary<DocumentVariantInfo, string> sourceDictionary)
        {
            IDictionary<DocumentVariantInfo, OpenApiDocument> result =
                new Dictionary<DocumentVariantInfo, OpenApiDocument>();

            foreach (var variantInfoDocumentKeyValuePair in sourceDictionary)
            {
                result[new DocumentVariantInfo(variantInfoDocumentKeyValuePair.Key)]
                    = new OpenApiStringReader().Read(variantInfoDocumentKeyValuePair.Value, out var _);
            }

            return result;
        }

        /// <summary>
        /// Converts dictionary of open api documents to serialized open api documents.
        /// </summary>
        /// <param name="sourceDictionary">The dictionary of open api documents.</param>
        /// <param name="openApiSpecVersion">The open api spec version to serialize to.</param>
        /// <param name="openApiFormat">The open api format to serialize to.</param>
        /// <returns>Dictionary of serialized open api documents </returns>
        public static IDictionary<DocumentVariantInfo, string> ToSerializedOpenApiDocuments(
            this IDictionary<DocumentVariantInfo, OpenApiDocument> sourceDictionary,
            OpenApiSpecVersion openApiSpecVersion = OpenApiSpecVersion.OpenApi3_0,
            OpenApiFormat openApiFormat = OpenApiFormat.Json)
        {
            IDictionary<DocumentVariantInfo, string> result =
                new Dictionary<DocumentVariantInfo, string>();

            foreach (var variantInfoDocumentKeyValuePair in sourceDictionary)
            {
                result[new DocumentVariantInfo(variantInfoDocumentKeyValuePair.Key)]
                    = variantInfoDocumentKeyValuePair.Value.Serialize(openApiSpecVersion, openApiFormat);
            }

            return result;
        }
    }
}