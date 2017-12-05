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
        /// This means the entire object in this scope (e.g. operation or document) cannot be generated.
        /// </summary>
        Failure,

        /// <summary>
        /// Generation process hits some unexpected issues but the process was still completed for that operation 
        /// on a best-effort basis. 
        /// </summary>
        Warning
    }
}