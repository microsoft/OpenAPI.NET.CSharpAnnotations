// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.OpenApiSpecification.Core.Models
{
    /// <summary>
    /// Enumeration of the parameter kinds.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ParameterKind
    {
        /// <summary>
        /// An undefined kind.
        /// </summary>
        [EnumMember(Value = "undefined")]
        Undefined,

        /// <summary>
        /// A cookie.
        /// </summary>
        [EnumMember( Value = "cookie" )]
        Cookie,

        /// <summary>
        /// A query key-value pair.
        /// </summary>
        [EnumMember(Value = "query")]
        Query,

        /// <summary>
        /// An URL path placeholder.
        /// </summary>
        [EnumMember(Value = "path")]
        Path,

        /// <summary>
        /// A HTTP header parameter.
        /// </summary>
        [EnumMember(Value = "header")]
        Header
    }
}