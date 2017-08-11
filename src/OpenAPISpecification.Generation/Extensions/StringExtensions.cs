// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Text.RegularExpressions;

namespace Microsoft.OpenApiSpecification.Generation.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="string"/>.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Removes blank lines.
        /// </summary>
        /// <param name="value">The string that needs to be updated.</param>
        /// <returns>The updated string.</returns>
        public static string RemoveBlankLines(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            return Regex.Replace(input: value, pattern: @"^\s*$(\r\n|\r|\n)",
                replacement: string.Empty, options: RegexOptions.Multiline).Trim();
        }
    }
}