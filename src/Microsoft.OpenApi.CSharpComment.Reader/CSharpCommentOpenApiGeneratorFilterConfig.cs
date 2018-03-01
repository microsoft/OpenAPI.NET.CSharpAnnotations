// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

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
        /// Creates a new instance of <see cref="CSharpCommentOpenApiGeneratorFilterConfig"/>.
        /// </summary>
        /// <param name="documentFilters">The list of document filters.</param>
        /// <param name="operationFilters">The list of operation filters.</param>
        public CSharpCommentOpenApiGeneratorFilterConfig(
            IList<IDocumentFilter> documentFilters,
            IList<IOperationFilter> operationFilters)
        {
            this.DocumentFilters = documentFilters;
            this.OperationFilters = operationFilters;
        }

        /// <summary>
        /// Gets the list of document config filters.
        /// </summary>
        public IList<IDocumentConfigFilter> DocumentConfigFilters { get; set; }

        /// <summary>
        /// Gets the list of document filters.
        /// </summary>
        public IList<IDocumentFilter> DocumentFilters { get; }

        /// <summary>
        /// Gets the list of operation config filters.
        /// </summary>
        public IList<IOperationConfigFilter> OperationConfigFilters { get; set; }

        /// <summary>
        /// Gets the list of operation filters.
        /// </summary>
        public IList<IOperationFilter> OperationFilters { get; }

        /// <summary>
        /// Gets the list of post processing document filters.
        /// </summary>
        public IList<IPostProcessingDocumentFilter> PostProcessingDocumentFilters { get; set; }

        /// <summary>
        /// Gets the list of preprocessing operation filters.
        /// </summary>
        public IList<IPreProcessingOperationFilter> PreProcessingOperationFilters { get; set; }
    }
}