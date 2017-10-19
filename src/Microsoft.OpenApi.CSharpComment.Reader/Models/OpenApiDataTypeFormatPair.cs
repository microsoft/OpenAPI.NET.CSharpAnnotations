// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.OpenApi.CSharpComment.Reader.Models
{
    /// <summary>
    /// Simple string pair that represents a data type and format pairing specific to Open API Specification.
    /// </summary>
    public class OpenApiDataTypeFormatPair
    {
        /// <summary>
        /// The open api specification data type.
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// The format of the open api specification api data type if applicable.
        /// </summary>
        public string Format { get; set; }
    }
}