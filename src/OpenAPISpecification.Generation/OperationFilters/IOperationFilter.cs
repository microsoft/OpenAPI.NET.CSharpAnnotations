// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Xml.Linq;
using Microsoft.OpenApiSpecification.Core.Models;

namespace Microsoft.OpenApi.CSharpComment.Reader.OperationFilters
{
    /// <summary>
    /// The class representing the contract of a Operation filter.
    /// </summary>
    public interface IOperationFilter
    {
        /// <summary>
        /// Contains the required logic to populate certain parts of Operation object.
        /// </summary>
        /// <param name="operation">The operation to be upated.</param>
        /// <param name="element">The xml element representing an operation in the annotation xml.</param>
        /// <param name="settings">The operation filter settings.</param>
        /// <remarks>
        /// Care should be taken to not overwrite the existing value in Operation if already present.
        /// This guarantees the predictable behavior that the first tag in the XML will be respected.
        /// It also guarantees that common annotations in the config file do not overwrite the
        /// annotations in the main documentation.
        /// </remarks>
        void Apply(Operation operation, XElement element, OperationFilterSettings settings);
    }
}