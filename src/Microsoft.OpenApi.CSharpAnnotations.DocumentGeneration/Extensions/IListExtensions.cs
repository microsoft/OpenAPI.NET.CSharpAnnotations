// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Extensions
{
    /// <summary>
    /// Extensions for <see cref="IList"/>.
    /// </summary>
    internal static class IListExtensions
    {
        /// <summary>
        /// Converts list of cref values to a unique key.
        /// </summary>
        /// <param name="value">The list of cref values.</param>
        /// <returns>The unique key.</returns>
        public static string ToCrefKey( this IList<string> value )
        {
            return string.Join( "_", value );
        }
    }
}