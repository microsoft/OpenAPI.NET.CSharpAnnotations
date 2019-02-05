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

        public static IEnumerable<object[]> GetTestCasesForToCamelCaseShouldUpdateCorrectly()
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

        public static IEnumerable<object[]> GetTestCasesForRemoveRoslynDuplicateStringShouldUpdateCorrectly()
        {
            yield return new object[]
            {
                "String with even length starting from -",
                "abc-ef-efg",
                "abc-ef-efg"
            };

            yield return new object[]
            {
                "String with odd length starting from -",
                "abc-de-de",
                "abc-de"
            };

            yield return new object[]
            {
                "String staring with @ and odd length starting from -",
                "@abc-de-de",
                "@abc-de"
            };

            yield return new object[]
            {
                "String staring with @ and odd length starting from - and no duplicates",
                "@abc-de-fg",
                "@abc-de-fg"
            };

            yield return new object[]
            {
                "String staring with @ and even length starting from -",
                "@abc-ef-efg",
                "@abc-ef-efg"
            };

            yield return new object[]
            {
                "String with special character $",
                "$abc$abc",
                "$abc"
            };

            yield return new object[]
            {
                "String with special character @",
                "@abc@de@fg@@de@fg@",
                "@abc@de@fg@"
            };

            yield return new object[]
            {
                "String with special character @ and duplicate not introduced by roslyn",
                "@abc@abc",
                "@abc@abc"
            };

            yield return new object[]
            {
                "String with special character @ and no duplicate",
                "@abc",
                "@abc"
            };

            yield return new object[]
            {
                "String with multiple special characters",
                "abc-$de-fg-h-$de-fg-h",
                "abc-$de-fg-h"
            };

            yield return new object[]
            {
                "String with no special character",
                "abc",
                "abc"
            };
            yield return new object[]
            {
                "String with @ not in starting",
                "abc@abc@abc",
                "abc@abc"
            };
        }

        [Theory]
        [MemberData(nameof(GetTestCasesForToCamelCaseShouldUpdateCorrectly))]
        public void ToCamelCaseShouldUpdateCorrectly(
            string testCaseName,
            string inputString,
            string expectedUpdatedString)
        {
            _output.WriteLine(testCaseName);

            inputString = inputString.ToCamelCase();

            inputString.Should().Be(expectedUpdatedString);
        }

        [Theory]
        [MemberData(nameof(GetTestCasesForRemoveRoslynDuplicateStringShouldUpdateCorrectly))]
        public void RemoveRoslynDuplicateStringShouldUpdateCorrectly(
            string testCaseName,
            string inputString,
            string expectedUpdatedString)
        {
            _output.WriteLine(testCaseName);

            inputString = inputString.RemoveRoslynDuplicateString();

            inputString.Should().Be(expectedUpdatedString);
        }
    }
}