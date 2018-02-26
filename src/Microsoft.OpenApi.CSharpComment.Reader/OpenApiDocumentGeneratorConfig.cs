// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.CSharpComment.Reader.DocumentConfigFilters;
using Microsoft.OpenApi.CSharpComment.Reader.DocumentFilters;
using Microsoft.OpenApi.CSharpComment.Reader.OperationConfigFilters;
using Microsoft.OpenApi.CSharpComment.Reader.OperationFilters;
using Microsoft.OpenApi.CSharpComment.Reader.PreprocessingOperationFilters;

namespace Microsoft.OpenApi.CSharpComment.Reader
{
    /// <summary>
    /// Holds the configuration needed for the generator including filters.
    /// </summary>
    public class OpenApiDocumentGeneratorConfig
    {
        /// <summary>
        /// Gets the list of document config filters.
        /// </summary>
        public IList<IDocumentConfigFilter> DocumentConfigFilters { get; set; }

        /// <summary>
        /// Gets the list of document filters.
        /// </summary>
        public IList<IDocumentFilter> DocumentFilters { get; set; }

        /// <summary>
        /// Gets the list of operation config filters.
        /// </summary>
        public IList<IOperationConfigFilter> OperationConfigFilters { get; set; }

        /// <summary>
        /// Gets the list of operation filters.
        /// </summary>
        public IList<IOperationFilter> OperationFilters { get; set; }

        /// <summary>
        /// Gets the list of preprocessing operation filters.
        /// </summary>
        public IList<IPreprocessingOperationFilter> PreprocessingOperationFilters { get; set; }
    }
}