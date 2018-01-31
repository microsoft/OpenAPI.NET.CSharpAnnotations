// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.CSharpComment.Reader.Exceptions;
using Microsoft.OpenApi.CSharpComment.Reader.Extensions;
using Microsoft.OpenApi.CSharpComment.Reader.Models.KnownStrings;
using Microsoft.OpenApi.CSharpComment.Reader.Models.SpecificationExtensions;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpComment.Reader.OperationFilters
{
    /// <summary>
    /// Parses the value of latencies tag in xml documentation and apply that as x-latencies in operation
    /// </summary>
    public class LatenciesToLatenciesFilter : IOperationFilter
    {
        /// <summary>
        /// Fetches the value of "latencies" tags and populate the x-latencies extension in the operation.
        /// </summary>
        /// <param name="operation">The operation to be updated.</param>
        /// <param name="element">The xml element representing an operation in the annotation xml.</param>
        /// <param name="settings">The operation filter settings.</param>
        /// <remarks>
        /// Care should be taken to not overwrite the existing value in Operation if already present.
        /// This guarantees the predictable behavior that the first tag in the XML will be respected.
        /// It also guarantees that common annotations in the config file do not overwrite the
        /// annotations in the main documentation.
        /// </remarks>
        public void Apply(OpenApiOperation operation, XElement element, OperationFilterSettings settings)
        {
            var latenciesElements = element.Elements()
                .Where( e => e.Name == KnownXmlStrings.Latencies )
                .ToList();
            
            foreach (var latenciesElement in latenciesElements)
            {
                var latency = new OpenApiLatency();

                var latencyElements = latenciesElement.Elements()
                    .Where(
                        e => e.Name == KnownXmlStrings.Latency)
                    .ToList();

                foreach (var latencyElement in latencyElements)
                {
                    var latencyType = latencyElement.Attribute(KnownXmlStrings.Type)?.Value ?? OpenApiExtensionConstants.Timeout;

                    if (latencyType != OpenApiExtensionConstants.Timeout)
                    {
                        // Types of latencies other than "timeout" are not supported at the moment.
                        continue;
                    }

                    if (!int.TryParse(latencyElement.Value, out var latencyNumber))
                    {
                        throw new InvalidLatencyException(latencyElement.Value);
                    }

                    var latencyClass = latencyElement.Attribute(KnownXmlStrings.Class)?.Value ?? OpenApiExtensionConstants.DefaultLatencyClass;

                    latency.Timeout[latencyClass] = new OpenApiInteger(latencyNumber);
                }
                
                var host = latenciesElement.Attribute(KnownXmlStrings.Host)?.Value ?? OpenApiExtensionConstants.DefaultHost;

                if (!operation.Extensions.ContainsKey(OpenApiExtensionConstants.XLatencies))
                {
                    operation.Extensions[OpenApiExtensionConstants.XLatencies] = new OpenApiLatencyDictionary();
                }

                ((OpenApiLatencyDictionary)operation.Extensions[OpenApiExtensionConstants.XLatencies])[host] = latency;
            }
        }
    }
}