// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Xml.Linq;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.OperationFilters;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.PostProcessingDocumentFilters;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.PreprocessingOperationFilters
{
    /// <summary>
    /// The class representing the contract of a filter to preprocess the <see cref="OpenApiOperation"/>
    /// objects in <see cref="OpenApiPaths"/> before each <see cref="OpenApiOperation"/> is processed by the
    /// <see cref="OperationFilter"/>.
    /// </summary>
    public abstract class PreProcessingOperationFilter : IFilter
    {
        /// <summary>
        /// Applies the filter to preprocess the the <see cref="OpenApiOperation"/> objects in
        /// <see cref="OpenApiPaths"/> before each <see cref="OpenApiOperation"/> is processed by the
        /// <see cref="OperationFilter"/>.
        /// </summary>
        /// <param name="paths">The paths to be upated.</param>
        /// <param name="element">The xml element representing an operation in the annotation xmls provided in
        /// <see cref="OpenApiGeneratorConfig.AnnotationXmlDocuments"/>.
        /// </param>
        /// <param name="settings"><see cref="PreProcessingOperationFilterSettings"/></param>
        public abstract void Apply(OpenApiPaths paths, XElement element, PreProcessingOperationFilterSettings settings);

        /// <summary>
        /// 
        /// </summary>
        public Type FilterType { get; } = typeof(PreProcessingOperationFilter);
    }
}