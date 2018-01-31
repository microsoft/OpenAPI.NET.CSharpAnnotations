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
    /// The dictionary object to store multiple <see cref="OpenApiLatency"/>
    /// </summary>
    public class OpenApiLatencyDictionary : Dictionary<string, OpenApiLatency>, IOpenApiExtension
    {
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

            foreach (var item in this)
            {
                writer.WritePropertyName(item.Key);
                item.Value.Write(writer);
            }

            writer.WriteEndObject();
        }
    }
}