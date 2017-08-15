// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Newtonsoft.Json.Linq;

namespace Microsoft.OpenApiSpecification.Generation.Tests
{
    internal class TestHelper
    {
        internal static bool AreJsonEqual(string expected, string actual)
        {
            var actualObject = JObject.Parse(actual);
            var expectedObject = JObject.Parse(expected);

            return JToken.DeepEquals(expectedObject, actualObject);
        }
    }
}