// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models
{
    /// <summary>
    /// Stores field values.
    /// </summary>
    public class FieldValueInfo
    {
        /// <summary>
        /// Gets or sets the field value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the error encountered while fetching field value.
        /// </summary>
        public GenerationError Error { get; set; }
    }
}