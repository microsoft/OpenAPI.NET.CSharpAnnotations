// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApiSpecification.Generation.Models;

namespace Microsoft.OpenApiSpecification.Generation.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="Type"/>.
    /// </summary>
    public static class TypeExtensions
    {
        private static readonly Dictionary<Type, OpenApiDataTypeFormatPair> _simpleTypeToOpenApiDataTypeFormat =
            new Dictionary<Type, OpenApiDataTypeFormatPair>
            {
                [typeof(bool)] = new OpenApiDataTypeFormatPair {DataType = "boolean"},
                [typeof(byte)] = new OpenApiDataTypeFormatPair {DataType = "string", Format = "byte"},
                [typeof(int)] = new OpenApiDataTypeFormatPair {DataType = "integer", Format = "int32"},
                [typeof(uint)] = new OpenApiDataTypeFormatPair {DataType = "integer", Format = "int32"},
                [typeof(long)] = new OpenApiDataTypeFormatPair {DataType = "integer", Format = "int64"},
                [typeof(ulong)] = new OpenApiDataTypeFormatPair {DataType = "integer", Format = "int64"},
                [typeof(float)] = new OpenApiDataTypeFormatPair {DataType = "number", Format = "float"},
                [typeof(double)] = new OpenApiDataTypeFormatPair {DataType = "number", Format = "double"},
                [typeof(decimal)] = new OpenApiDataTypeFormatPair {DataType = "number", Format = "double"},
                [typeof(DateTime)] = new OpenApiDataTypeFormatPair {DataType = "string", Format = "date-time"},
                [typeof(DateTimeOffset)] = new OpenApiDataTypeFormatPair {DataType = "string", Format = "date-time"},
                [typeof(Guid)] = new OpenApiDataTypeFormatPair {DataType = "string", Format = "uuid"},
                [typeof(string)] = new OpenApiDataTypeFormatPair {DataType = "string"}
            };

        /// <summary>
        /// Gets the item type in an array or an IEnumerable.
        /// </summary>
        /// <param name="type">An array or IEnumerable type to get the item type from.</param>
        /// <returns>The type of the item in the array or IEnumerable.</returns>
        public static Type GetEnumerableItemType(this Type type)
        {
            if (type.IsArray)
            {
                return type.GetElementType();
            }

            if (typeof(IEnumerable).IsAssignableFrom(type) && type.GetGenericArguments().Any())
            {
                return type.GetGenericArguments().First();
            }

            return null;
        }

        /// <summary>
        /// Determines whether the given type is a dictionary.
        /// </summary>
        public static bool IsDictionary(this Type type)
        {
            return type.IsGenericType
                && (typeof(IDictionary<,>).IsAssignableFrom(type.GetGenericTypeDefinition())
                    || typeof(Dictionary<,>).IsAssignableFrom(type.GetGenericTypeDefinition()));
        }

        /// <summary>
        /// Determines whether the given type is enumerable.
        /// </summary>
        /// <remarks>
        /// Even though string is technically an IEnumerable of char, this method will
        /// return false for string since it is generally expected to behave like a simple type.
        /// </remarks>
        public static bool IsEnumerable(this Type type)
        {
            return type != typeof(string) && (type.IsArray || typeof(IEnumerable).IsAssignableFrom(type));
        }

        /// <summary>
        /// Determines whether the given type is a "simple" type.
        /// </summary>
        /// <remarks>
        /// A simple type is defined to match with what OpenAPI generally recognizes.
        /// This includes the so-called C# primitive types and a few other types such as string, DateTime, etc.
        /// </remarks>
        public static bool IsSimple(this Type type)
        {
            return _simpleTypeToOpenApiDataTypeFormat.ContainsKey(type);
        }

        /// <summary>
        /// Maps a simple type to an OpenAPI data type and format.
        /// </summary>
        /// <param name="type">Simple type.</param>
        /// <remarks>
        /// From http://swagger.io/specification/#data-types-12
        /// Common Name      type    format      Comments
        /// ===========      ======= ======      =========================================
        /// integer          integer int32       signed 32 bits
        /// long             integer int64       signed 64 bits
        /// float            number  float
        /// double           number  double
        /// string           string  [empty]
        /// byte             string  byte        base64 encoded characters
        /// binary           string  binary      any sequence of octets
        /// boolean          boolean [empty]
        /// date             string  date        As defined by full-date - RFC3339
        /// dateTime         string  date-time   As defined by date-time - RFC3339
        /// password         string  password    Used to hint UIs the input needs to be obscured.
        /// If the type is not recognized as "simple", System.String will be returned.
        /// </remarks>
        public static OpenApiDataTypeFormatPair MapToOpenApiDataTypeFormatPair(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            OpenApiDataTypeFormatPair result;

            return _simpleTypeToOpenApiDataTypeFormat.TryGetValue(type, out result)
                ? result
                : new OpenApiDataTypeFormatPair {DataType = "string"};
        }
    }
}