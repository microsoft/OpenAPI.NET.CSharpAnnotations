// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.OpenApi.CSharpComment.Reader.DocumentConfigFilters;
using Microsoft.OpenApi.CSharpComment.Reader.DocumentFilters;
using Microsoft.OpenApi.CSharpComment.Reader.OperationConfigFilters;
using Microsoft.OpenApi.CSharpComment.Reader.OperationFilters;
using Microsoft.OpenApi.CSharpComment.Reader.PostProcessingDocumentFilters;
using Microsoft.OpenApi.CSharpComment.Reader.PreprocessingOperationFilters;

namespace Microsoft.OpenApi.CSharpComment.Reader
{
    /// <summary>
    /// The class encapsulating all the filters that will be applied while generating/processing OpenAPI document from
    /// C# comments.
    /// </summary>
    public class CSharpCommentOpenApiGeneratorFilterConfig
    {
        /// <summary>
        /// List of default document config filters.
        /// </summary>
        public static readonly IReadOnlyList<IDocumentConfigFilter> DefaultDocumentConfigFilters =
            new List<IDocumentConfigFilter>
            {
                new DocumentVariantAttributesFilter()
            };

        /// <summary>
        /// List of default document filters.
        /// </summary>
        public static readonly IReadOnlyList<IDocumentFilter> DefaultDocumentFilters =
            new List<IDocumentFilter>
            {
                new AssemblyNameToInfoFilter(),
                new UrlToServerFilter(),
                new MemberSummaryToSchemaDescriptionFilter()
            };

        /// <summary>
        /// List of default operation config filters.
        /// </summary>
        public static readonly IReadOnlyList<IOperationConfigFilter> DefaultOperationConfigFilters =
            new List<IOperationConfigFilter>
            {
                new CommonAnnotationFilter()
            };

        /// <summary>
        /// List of default operation filters.
        /// </summary>
        public static readonly IReadOnlyList<IOperationFilter> DefaultOperationFilters =
            new List<IOperationFilter>
            {
                new GroupToTagFilter(),
                new ParamToParameterFilter(),
                new ParamToRequestBodyFilter(),
                new ResponseToResponseFilter(),
                new RemarksToDescriptionFilter(),
                new SummaryToSummaryFilter()
            };

        /// <summary>
        /// List of default pre processing operation filters.
        /// </summary>
        public static readonly IReadOnlyList<IPreProcessingOperationFilter> DefaultPreProcessingOperationFilters =
            new List<IPreProcessingOperationFilter>
            {
                new ConvertAlternativeParamTagsFilter(),
                new PopulateInAttributeFilter(),
                new BranchOptionalPathParametersFilter()
            };

        /// <summary>
        /// List of default post processing document filters.
        /// </summary>
        public static readonly IReadOnlyList<IPostProcessingDocumentFilter> DefaultPostProcessingDocumentFilters =
            new List<IPostProcessingDocumentFilter>
            {
                new RemoveFailedGenerationOperationFilter()
            };

        /// <summary>
        /// Creates a new instance of <see cref="CSharpCommentOpenApiGeneratorFilterConfig"/> with default filters.
        /// </summary>
        public CSharpCommentOpenApiGeneratorFilterConfig()
            : this(
                DefaultDocumentFilters.ToList(),
                DefaultOperationFilters.ToList(),
                DefaultOperationConfigFilters.ToList(),
                DefaultDocumentConfigFilters.ToList(),
                DefaultPostProcessingDocumentFilters.ToList(),
                DefaultPreProcessingOperationFilters.ToList())
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="CSharpCommentOpenApiGeneratorFilterConfig"/> with provided filters.
        /// </summary>
        /// <param name="documentFilters">The list of document filters.</param>
        /// <param name="operationFilters">The list of operation filters.</param>
        /// <param name="operationConfigFilters">The list of operation config filters.</param>
        /// <param name="documentConfigFilters">The list of document config filters.</param>
        /// <param name="postProcessingDocumentFilters">The list of post processing document filters.</param>
        /// <param name="preProcessingOperationFilters">The list of pre processing operation filters.</param>
        public CSharpCommentOpenApiGeneratorFilterConfig(
            IList<IDocumentFilter> documentFilters,
            IList<IOperationFilter> operationFilters,
            IList<IOperationConfigFilter> operationConfigFilters,
            IList<IDocumentConfigFilter> documentConfigFilters,
            IList<IPostProcessingDocumentFilter> postProcessingDocumentFilters,
            IList<IPreProcessingOperationFilter> preProcessingOperationFilters)
        {
            this.DocumentFilters = documentFilters ?? throw new ArgumentNullException(nameof(documentFilters));

            this.OperationFilters = operationFilters ?? throw new ArgumentNullException(nameof(operationFilters));

            this.OperationConfigFilters = operationConfigFilters 
                ?? throw new ArgumentNullException(nameof(operationConfigFilters));

            this.DocumentConfigFilters = documentConfigFilters 
                ?? throw new ArgumentNullException(nameof(documentConfigFilters));

            this.PostProcessingDocumentFilters = postProcessingDocumentFilters 
                ?? throw new ArgumentNullException(nameof(postProcessingDocumentFilters));

            this.PreProcessingOperationFilters = preProcessingOperationFilters 
                ?? throw new ArgumentNullException(nameof(preProcessingOperationFilters));
        }

        /// <summary>
        /// Gets the list of document config filters.
        /// </summary>
        public IList<IDocumentConfigFilter> DocumentConfigFilters { get; }

        /// <summary>
        /// Gets the list of document filters.
        /// </summary>
        public IList<IDocumentFilter> DocumentFilters { get; }

        /// <summary>
        /// Gets the list of operation config filters.
        /// </summary>
        public IList<IOperationConfigFilter> OperationConfigFilters { get; }

        /// <summary>
        /// Gets the list of operation filters.
        /// </summary>
        public IList<IOperationFilter> OperationFilters { get; }

        /// <summary>
        /// Gets the list of post processing document filters.
        /// </summary>
        public IList<IPostProcessingDocumentFilter> PostProcessingDocumentFilters { get; }

        /// <summary>
        /// Gets the list of preprocessing operation filters.
        /// </summary>
        public IList<IPreProcessingOperationFilter> PreProcessingOperationFilters { get; }
    }
}