// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Xml.Linq;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.OperationFilters
{
    /// <summary>
    /// The class representing the contract of a filter to process the <see cref="OpenApiOperation"/>
    /// objects in <see cref="OpenApiPaths"/> based on the information provided in the operation xml element.
    /// </summary>
    public interface IOperationFilter : IFilter
    {
        /// <summary>
        /// Contains the required logic to populate certain parts of Operation object in <see cref="OpenApiOperation"/>.
        /// </summary>
        /// <param name="operation">The operation to be upated.</param>
        /// <param name="element">The xml element representing an operation in the annotation xmls provided in
        /// <see cref="OpenApiGeneratorConfig.AnnotationXmlDocuments"/>.</param>
        /// <param name="settings"><see cref="OperationFilterSettings"/></param>
        /// <remarks>
        /// Care should be taken to not overwrite the existing value in Operation if already present.
        /// This guarantees the predictable behavior that the first tag in the XML will be respected.
        /// It also guarantees that common annotations in the config file do not overwrite the
        /// annotations in the main documentation.
        /// </remarks>
        void Apply(OpenApiOperation operation, XElement element, OperationFilterSettings settings);
    }
}