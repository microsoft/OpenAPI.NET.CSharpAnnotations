// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Xml.Linq;
using Microsoft.OpenApiSpecification.Core.Models;
using Microsoft.OpenApiSpecification.Generation.OperationFilters;

namespace Microsoft.OpenApiSpecification.Generation.PreProcessingOperationFilters
{
    /// <summary>
    /// The class representing the contract of a filter to preprocess the <see cref="Operation"/> objects in <see cref="Paths"/>
    /// before each <see cref="Operation"/> is processed by the <see cref="IOperationFilter"/>.
    /// </summary>
    public interface IPreprocessingOperationFilter
    {
        /// <summary>
        /// Applies the filter to preprocess the the <see cref="Operation"/> objects in <see cref="Paths"/>
        /// before each <see cref="Operation"/> is processed by the <see cref="IOperationFilter"/>.
        /// </summary>
        /// <param name="paths">The paths to be upated.</param>
        /// <param name="element">The xml element representing an operation in the annotation xml.</param>
        /// <param name="settings">The operation filter settings.</param>
        void Apply(Paths paths, XElement element, PreprocessingOperationFilterSettings settings);
    }
}