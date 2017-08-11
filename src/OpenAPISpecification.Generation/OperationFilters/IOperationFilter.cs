// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Xml.Linq;
using Microsoft.OpenApiSpecification.Core.Models;

namespace Microsoft.OpenApiSpecification.Generation.OperationFilters
{
    /// <summary>
    /// The class representing the contract of a Operation filter
    /// </summary>
    public interface IOperationFilter
    {
        /// <summary>
        /// Contains the required logic to populate certain parts of Operation object.
        /// </summary>
        /// <param name="operation">The operation to be updated.</param>
        /// <param name="element">The xml element representing an operation in the annotation xml.</param>
        void Apply(Operation operation, XElement element);
    }
}