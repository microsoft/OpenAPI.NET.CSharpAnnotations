// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    internal static class IListExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetCrefKey( this IList<string> value )
        {
            return string.Join( "_", value );
        }
    }
}