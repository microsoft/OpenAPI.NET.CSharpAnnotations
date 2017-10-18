// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Xml.Linq;
using Microsoft.OpenApiSpecification.Core.Models;

namespace Microsoft.OpenApiSpecification.Generation.OperationConfigFilters
{
    /// <summary>
    /// The class representing the contract of an operation config filter.
    /// </summary>
    public interface IOperationConfigFilter
    {
        /// <summary>
        /// Contains the required logic to manipulate the Operation object based on information
        /// in the operation config element.
        /// </summary>
        /// <param name="operation">The operation to be updated.</param>
        /// <param name="element">The xml element containing operation-level config in the config xml.</param>
        /// <param name="settings">The operation config filter settings.</param>
        void Apply(Operation operation, XElement element, OperationConfigFilterSettings settings);
    }
}