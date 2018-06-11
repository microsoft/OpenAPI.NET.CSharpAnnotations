// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.ExtensionsTests
{
    [Collection("DefaultSettings")]
    public class StringExtensionTest
    {
        private readonly ITestOutputHelper _output;

        public StringExtensionTest(ITestOutputHelper output)
        {
            _output = output;
        }

        public static IEnumerable<object[]> GetTestCasesForStringExtensionShouldSucceed()
        {
            yield return new object[]
            {
                "Dictionary with value of type array",
                "FooBar",
                "fooBar"
            };

            yield return new object[]
            {
                "Dictionary with value of type array",
                "fooBar",
                "fooBar"
            };

            yield return new object[]
            {
                "Dictionary with value of type array",
                "FBAR",
                "fBAR"
            };
        }


        [Theory]
        [MemberData(nameof(GetTestCasesForStringExtensionShouldSucceed))]
        public void StringExtensionShouldUpdateCorrectly(
            string testCaseName,
            string inputString,
            string expectedUpdatedString)
        {
            _output.WriteLine(testCaseName);

            inputString = inputString.ToCamelCase();

            inputString.Should().Be(expectedUpdatedString);
        }
    }
}