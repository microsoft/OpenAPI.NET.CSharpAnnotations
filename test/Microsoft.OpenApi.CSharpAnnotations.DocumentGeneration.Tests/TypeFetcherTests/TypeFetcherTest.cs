// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.TypeFetcherTests
{
    [Collection("DefaultSettings")]
    public class TypeFetcherTest
    {
        private const string InputDirectory = "TypeFetcherTests/Input";
        private readonly ITestOutputHelper _output;

        public TypeFetcherTest(ITestOutputHelper output)
        {
            _output = output;
        }

        public static IEnumerable<object[]> GetTestCasesForTypeFetcherShouldSucceed()
        {
            yield return new object[]
            {
                "Dictionary with value of type array",
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.SampleApis.dll"),
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.Contracts.dll")
                },
                new List<string>
                {
                    "T:System.Collections.Generic.Dictionary`2",
                    "T:System.String",
                    "T:Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.Contracts.SampleObject1[]"
                },
                "System.Collections.Generic.Dictionary`2[System.String,System.Collections.Generic.IList`1" 
                + "[Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.Contracts.SampleObject1]]"
            };

            yield return new object[]
            {
                "Dictionary with key of type array",
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.SampleApis.dll"),
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.Contracts.dll")
                },
                new List<string>
                {
                    "T:System.Collections.Generic.Dictionary`2",
                    "T:Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.Contracts.SampleObject1[]",
                    "T:System.String"
                },
                "System.Collections.Generic.Dictionary`2[System.Collections.Generic.IList`1"
                + "[Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.Contracts.SampleObject1],System.String]"
            };
        }

        [Theory]
        [MemberData(nameof(GetTestCasesForTypeFetcherShouldSucceed))]
        public void TypeFetcherShouldReturnCorrectTypeData(
            string testCaseName,
            IList<string> inputBinaryFiles,
            IList<string> crefValues,
            string expectedTypeAsString)
        {
            _output.WriteLine(testCaseName);

            var typeFetcher = new TypeFetcher(inputBinaryFiles);
            var type = typeFetcher.LoadTypeFromCrefValues(crefValues);

            type.ToString().Should().Be(expectedTypeAsString);
        }
    }
}