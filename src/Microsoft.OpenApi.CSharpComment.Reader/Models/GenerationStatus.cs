// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.OpenApi.CSharpComment.Reader.Models
{
    /// <summary>
    /// The enum representing various generation status.
    /// </summary>
    public enum GenerationStatus
    {
        /// <summary>
        /// Generation passes.
        /// </summary>
        Success,

        /// <summary>
        /// Generation fails at a critical step.
        /// This means the entire operation or entire document cannot be generated.
        /// </summary>
        Failure,

        /// <summary>
        /// Generation fails in the operation or operation-config filters.
        /// The generation was still completed for that operation on a best-effort basis but not all
        /// the information was processed correctly.
        /// </summary>
        Warning
    }
}