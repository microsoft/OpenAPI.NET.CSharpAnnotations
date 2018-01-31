// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.CSharpComment.Reader.Models.SpecificationExtensions
{
    /// <summary>
    /// The latency object for x-latencies extension in <see cref="OpenApiOperation"/>.
    /// </summary>
    public class OpenApiLatency : IOpenApiExtension
    {
        // TODO: Change OpenApiInteger to just int once WriteOptionalMap is fixed to handle primitive types.
        /// <summary>
        /// Describes timeout latencies for each "latency class" (e.g. default, single DC)
        /// </summary>
        public IDictionary<string, OpenApiInteger> Timeout { get; set; } = new Dictionary<string, OpenApiInteger>();
        
        /// <summary>
        /// Writes the object.
        /// </summary>
        public void Write(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            writer.WriteStartObject();

            writer.WriteOptionalMap(
                OpenApiExtensionConstants.Timeout,
                Timeout,
                (w, value) => w.WriteAny(value));

            writer.WriteEndObject();
        }
    }
}