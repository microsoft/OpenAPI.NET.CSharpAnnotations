// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Extensions;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.ReferenceRegistryTests.Types
{
    internal class SampleSelfReferencingType
    {
        public static readonly OpenApiSchema schema = new OpenApiSchema
        {
            Type = "object",
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["SampleSelfReferencingListProperty"] = new OpenApiSchema
                {
                    Type = "array",
                    Items = new OpenApiSchema
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.Schema,
                            Id = typeof(SampleSelfReferencingType).ToString().SanitizeClassName()
                        }
                    }
                },
                ["SampleSelfReferencingProperty"] = new OpenApiSchema
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.Schema,
                        Id = typeof(SampleSelfReferencingType).ToString().SanitizeClassName()
                    }
                }
            }
        };

        [JsonIgnore]
        public SampleSelfReferencingType SampleSelfReferencingIgnoredProperty { get; set; }

        public IList<SampleSelfReferencingType> SampleSelfReferencingListProperty { get; set; }

        public SampleSelfReferencingType SampleSelfReferencingProperty { get; set; }
    }
}