// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.OpenApiSpecification.Generation.Models;

namespace Microsoft.OpenApiSpecification.Generation
{
    /// <summary>
    /// Mapping class for primitive types to open api specification and json schema data types and formats.
    /// </summary>
    public class PrimitiveTypeToDataTypeAndFormatMapper
    {
        /// <summary>
        /// Map primitive type that conforms to the open api spec.
        /// </summary>
        /// <param name="type">Primitive type.</param>
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
        /// </remarks>
        public static OpenApiDataTypeFormatPair Map(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var typeName = type.FullName;

            switch (typeName)
            {
                case "System.Boolean":
                    return new OpenApiDataTypeFormatPair {DataType = "boolean"};

                case "System.Byte":
                    return new OpenApiDataTypeFormatPair {DataType = "string", Format = "byte"};

                case "System.Array":
                    return new OpenApiDataTypeFormatPair {DataType = "array"};

                case "System.Binary":
                    return new OpenApiDataTypeFormatPair {DataType = "string", Format = "binary"};

                case "System.Int32":
                case "System.UInt32":
                    return new OpenApiDataTypeFormatPair {DataType = "integer", Format = "int32"};

                case "System.Int64":
                case "System.UInt64":
                    return new OpenApiDataTypeFormatPair {DataType = "integer", Format = "int64"};

                case "System.Single":
                    return new OpenApiDataTypeFormatPair {DataType = "number", Format = "float"};

                case "System.Double":
                case "System.Decimal":
                    return new OpenApiDataTypeFormatPair {DataType = "number", Format = "double"};

                case "System.DateTime":
                case "System.DateTimeOffset":
                    return new OpenApiDataTypeFormatPair {DataType = "string", Format = "date-time"};

                case "System.Guid":
                    return new OpenApiDataTypeFormatPair {DataType = "string", Format = "uuid"};

                default:
                    return new OpenApiDataTypeFormatPair {DataType = "string"};
            }
        }
    }
}