// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.DocumentConfigFilters;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.DocumentFilters;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.OperationConfigFilters;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.OperationFilters;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.PostProcessingDocumentFilters;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.PreprocessingOperationFilters;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration
{
    /// <summary>
    /// The filter set that will be used while generating OpenAPI specification document.
    /// </summary>
    public sealed class FilterSet : IEnumerable<IFilter>
    {
        private IDictionary<Type, IList<IFilter>> _filters = new Dictionary<Type, IList<IFilter>>();
        private static FilterSet _defaultFilterSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterSet"/> class.
        /// </summary>
        public FilterSet()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterSet"/> class.
        /// </summary>
        /// <param name="filterSet">Filter set to be copied from.</param>
        public FilterSet(FilterSet filterSet)
        {
            if (filterSet == null)
            {
                return;
            }

            // We create a new instance of FilterSet per call as a safeguard
            // against unintentional modification of the private _defaultFilterSet.
            foreach (IFilter filter in filterSet)
            {
                Add(filter);
            }
        }

        /// <summary>
        /// Add the new filter into the filter set.
        /// </summary>
        /// <param name="filter">The filter.</param>
        public void Add(IFilter filter)
        {
            if (!_filters.ContainsKey(filter.FilterType))
            {
                _filters[filter.FilterType] = new List<IFilter>();
            }

            _filters[filter.FilterType].Add(filter);
        }

        /// <summary>
        /// Gets the default filter sets.
        /// </summary>
        public static FilterSet GetDefaultFilterSet(FilterSetVersion version)
        {
            if (_defaultFilterSet == null)
            {
                _defaultFilterSet = new FilterSet();

                if (version == FilterSetVersion.V1)
                {
                    //Document config filters
                    _defaultFilterSet.Add(new DocumentVariantAttributesFilter());

                    //Document filters
                    _defaultFilterSet.Add(new AssemblyNameToInfoFilter());
                    _defaultFilterSet.Add(new UrlToServerFilter());
                    _defaultFilterSet.Add(new MemberSummaryToSchemaDescriptionFilter());

                    //Operation config filters
                    _defaultFilterSet.Add(new CommonAnnotationFilter());

                    //Operation filters
                    _defaultFilterSet.Add(new GroupToTagFilter());
                    _defaultFilterSet.Add(new ParamToParameterFilter());
                    _defaultFilterSet.Add(new ParamToRequestBodyFilter());
                    _defaultFilterSet.Add(new RemarksToDescriptionFilter());
                    _defaultFilterSet.Add(new ResponseToResponseFilter());
                    _defaultFilterSet.Add(new SummaryToSummaryFilter());

                    //Pre processing operation filters
                    _defaultFilterSet.Add(new ConvertAlternativeParamTagsFilter());
                    _defaultFilterSet.Add(new PopulateInAttributeFilter());
                    _defaultFilterSet.Add(new BranchOptionalPathParametersFilter());

                    //Post processing document filters
                    _defaultFilterSet.Add(new RemoveFailedGenerationOperationFilter());
                }

                return _defaultFilterSet;
            }

            return new FilterSet(_defaultFilterSet);
        }

        /// <summary>
        /// Get the enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<IFilter> GetEnumerator()
        {
            foreach (var filterList in _filters.Values)
            {
                foreach (var filter in filterList)
                {
                    yield return filter;
                }
            }
        }

        /// <summary>
        /// Get the enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}