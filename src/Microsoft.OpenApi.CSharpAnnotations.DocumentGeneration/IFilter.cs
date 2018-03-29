// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration
{
    /// <summary>
    /// Contract for the filters.
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        /// The filter type.
        /// </summary>
        Type FilterType { get; }
    }
}