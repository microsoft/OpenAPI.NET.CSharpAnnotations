// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.ReferenceRegistryTests.Types
{
    internal class SampleGenericType<T>
    {
        public static readonly OpenApiSchema schemaString = new OpenApiSchema
        {
            Type = "object",
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["SamplePropertyGeneric"] = new OpenApiSchema
                {
                    Type = "string"
                },
                ["SamplePropertyString"] = new OpenApiSchema
                {
                    Type = "string"
                }
            }
        };

        public T SamplePropertyGeneric { get; set; }

        public string SamplePropertyString { get; set; }
    }
}