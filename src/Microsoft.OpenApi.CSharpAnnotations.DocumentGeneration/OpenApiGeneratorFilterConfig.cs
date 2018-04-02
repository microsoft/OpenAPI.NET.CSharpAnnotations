// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration
{
    /// <summary>
    /// The class encapsulating all the filters that will be applied while generating/processing OpenAPI document from
    /// C# annotations.
    /// </summary>
    public class OpenApiGeneratorFilterConfig
    {
        /// <summary>
        /// Creates a new instance of <see cref="OpenApiGeneratorFilterConfig"/> with default filters.
        /// </summary>
        public OpenApiGeneratorFilterConfig(FilterSetVersion filterSetVersion)
        {
            Filters = FilterSet.GetDefaultFilterSet(filterSetVersion);
        }

        /// <summary>
        /// Creates a new instance of <see cref="OpenApiGeneratorFilterConfig"/> with provided filters.
        /// </summary>
        /// <param name="filters">The list of document filters.</param>
        public OpenApiGeneratorFilterConfig(IEnumerable<IFilter> filters)
        {
            if (filters == null)
            {
                throw new ArgumentNullException(nameof(filters));
            }

            foreach (var filter in filters)
            {
                Filters.Add(filter);
            }
        }

        /// <summary>
        /// Gets the list of filters.
        /// </summary>
        public FilterSet Filters { get; } = new FilterSet();
    }
}