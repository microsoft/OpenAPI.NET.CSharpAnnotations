// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Xml.Linq;
using Microsoft.OpenApi.CSharpComment.Reader.OperationFilters;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpComment.Reader.PreprocessingOperationFilters
{
    /// <summary>
    /// The class representing the contract of a filter to preprocess the <see cref="OpenApiOperation"/>
    /// objects in <see cref="OpenApiPaths"/> before each <see cref="OpenApiOperation"/> is processed by the
    /// <see cref="IOperationFilter"/>.
    /// </summary>
    public interface IPreprocessingOperationFilter
    {
        /// <summary>
        /// Applies the filter to preprocess the the <see cref="OpenApiOperation"/> objects in
        /// <see cref="OpenApiPaths"/> before each <see cref="OpenApiOperation"/> is processed by the
        /// <see cref="IOperationFilter"/>.
        /// </summary>
        /// <param name="paths">The paths to be upated.</param>
        /// <param name="element">The xml element representing an operation in the annotation xml.</param>
        /// <param name="settings">The operation filter settings.</param>
        void Apply(OpenApiPaths paths, XElement element, PreprocessingOperationFilterSettings settings);
    }
}