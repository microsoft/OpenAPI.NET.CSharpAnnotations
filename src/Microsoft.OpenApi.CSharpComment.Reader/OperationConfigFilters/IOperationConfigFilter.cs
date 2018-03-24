// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Xml.Linq;
using Microsoft.OpenApi.CSharpComment.Reader.OperationFilters;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpComment.Reader.OperationConfigFilters
{
    /// <summary>
    /// The class representing the contract of a filter to process the <see cref="OpenApiOperation"/>
    /// based on the information in the operation config element, after its processed by the
    /// <see cref="IOperationFilter"/>.
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
        void Apply(OpenApiOperation operation, XElement element, OperationConfigFilterSettings settings);
    }
}