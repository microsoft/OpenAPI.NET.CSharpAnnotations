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

        public static IEnumerable<object[]> GetTestCasesForToCamelCaseShouldSucceed()
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

        public static IEnumerable<object[]> GetTestCasesForRemoveRoslynDuplicateStringShouldSucceed()
        {
            yield return new object[]
            {
                "String with special character $",
                "$skip$skip",
                "$skip"
            };

            yield return new object[]
            {
                "String with special character @",
                "@service_Catalog_Id@Id@test@Id@test",
                "@service_Catalog_Id@Id@test"
            };

            yield return new object[]
            {
                "String with special character @ and duplicate not introduced by roslyn",
                "@skip@skip",
                "@skip@skip"
            };

            yield return new object[]
            {
                "String with special character @ and no duplicate",
                "@skip",
                "@skip"
            };

            yield return new object[]
            {
                "String with multiple special characters",
                "service-$Catalog-Id-1-$Catalog-Id-1",
                "service-$Catalog-Id-1"
            };

            yield return new object[]
            {
                "String with special character -",
                "ms-cv-cv",
                "ms-cv"
            };

            yield return new object[]
            {
                "string with no special character",
                "testParam",
                "testParam"
            };
        }

        [Theory]
        [MemberData(nameof(GetTestCasesForToCamelCaseShouldSucceed))]
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
        [MemberData(nameof(GetTestCasesForRemoveRoslynDuplicateStringShouldSucceed))]
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