// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.OpenApiSpecification.Generation.Extensions
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
        /// <param name="sourceDictionary">The source dictionary.</param>
        /// <param name="targetDictionary">The target dictionary.</param>
        public static void CopyInto<TKey, TValue>(this IDictionary<TKey, TValue> sourceDictionary,
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
    }
}