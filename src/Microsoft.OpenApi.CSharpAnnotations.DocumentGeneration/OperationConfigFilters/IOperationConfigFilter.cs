// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.OperationFilters;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.OperationConfigFilters
{
    /// <summary>
    /// The class representing the contract of a filter to process the <see cref="OpenApiOperation"/>
    /// based on the information in the operation config element in
    /// <see cref="OpenApiGeneratorConfig.AdvancedConfigurationXmlDocument"/>, after its processed by the
    /// <see cref="IOperationFilter"/>.
    /// </summary>
    public interface IOperationConfigFilter : IFilter
    {
        /// <summary>
        /// Contains the required logic to manipulate the Operation object based on information
        /// in the operation config element.
        /// </summary>
        /// <param name="operation">The operation to be updated.</param>
        /// <param name="element">The xml element containing operation-level config in the config xml,
        /// <see cref="OpenApiGeneratorConfig.AdvancedConfigurationXmlDocument"/>.
        /// </param>
        /// <param name="settings"><see cref="OperationConfigFilterSettings"/></param>
        /// <returns>The list of generation errors, if any produced when processing the filter.</returns>
        IList<GenerationError> Apply(
            OpenApiOperation operation,
            XElement element,
            OperationConfigFilterSettings settings);
    }
}