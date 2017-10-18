// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OpenApiSpecification.Core.Serialization;
using Newtonsoft.Json;

namespace Microsoft.OpenApiSpecification.Generation.Tests
{
    /// <summary>
    /// Fixture containing default settings for external libraries.
    /// </summary>
    public class DefaultSettingsFixture
    {
        /// <summary>
        /// Initializes an intance of <see cref="DefaultSettingsFixture"/>.
        /// </summary>
        public DefaultSettingsFixture()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                ContractResolver = new EmptyCollectionContractResolver()
            };

            AssertionOptions.AssertEquivalencyUsing(o => o.AllowingInfiniteRecursion());
        }
    }
}