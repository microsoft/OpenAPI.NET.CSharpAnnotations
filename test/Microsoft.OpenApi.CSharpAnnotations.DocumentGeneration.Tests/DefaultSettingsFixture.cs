// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using FluentAssertions;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests
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
            AssertionOptions.AssertEquivalencyUsing(o => o.AllowingInfiniteRecursion());
        }
    }
}