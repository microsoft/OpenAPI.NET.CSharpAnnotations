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
    internal static class DictionaryExtensions
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
        /// Converts <see cref="Dictionary{TKey,TValue}"/>where TKey is <see cref="DocumentVariantInfo"/>
        /// and TValue is <see cref="string"/>
        /// To <see cref="Dictionary{TKey,TValue}"/>where TKey is <see cref="DocumentVariantInfo"/>
        /// and TValue is <see cref="OpenApiDocument"/>.
        /// </summary>
        /// <param name="sourceDictionary">The serialized OpenAPI documents dictionary.</param>
        /// <returns>Dictionary mapping document variant metadata to their respective OpenAPI document.
        /// </returns>
        public static IDictionary<DocumentVariantInfo, OpenApiDocument> ToOpenApiDocuments(
            this IDictionary<DocumentVariantInfo, string> sourceDictionary)
        {
            if (sourceDictionary == null)
            {
                throw new ArgumentNullException(nameof(sourceDictionary));
            }

            IDictionary<DocumentVariantInfo, OpenApiDocument> openApiDocuments =
                new Dictionary<DocumentVariantInfo, OpenApiDocument>();

            foreach (var variantInfoDocumentKeyValuePair in sourceDictionary)
            {
                openApiDocuments[new DocumentVariantInfo(variantInfoDocumentKeyValuePair.Key)]
                    = new OpenApiStringReader().Read(variantInfoDocumentKeyValuePair.Value, out var _);
            }

            return openApiDocuments;
        }

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

        /// <summary>
        /// Converts <see cref="IDictionary{TKey, TValue}"/>where TKey is <see cref="string"/>
        /// and TValue is <see cref="string"/>to comma separated string e.g. {"key1":"value1","key2":"value2"}.
        /// </summary>
        /// <param name="sourceDictionary">The dictionary to convert.</param>
        /// <returns>The comma seperated string representation of the provided dictionary.</returns>
        public static string ToSerializedString(this IDictionary<string, string> sourceDictionary)
        {
            if (sourceDictionary == null)
            {
                return string.Empty;
            }

            return $"{{{string.Join(",", sourceDictionary.Select(x => $"\"{x.Key}\":\"{x.Value}\""))}}}";
        }
    }
}