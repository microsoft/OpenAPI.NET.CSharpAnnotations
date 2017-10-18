// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OpenApiSpecification.Core.Models
{
    /// <summary>
    /// Represents the available paths and operations for the API.
    /// </summary>
    public class Paths : Dictionary<string, PathItem>
    {
    }
}