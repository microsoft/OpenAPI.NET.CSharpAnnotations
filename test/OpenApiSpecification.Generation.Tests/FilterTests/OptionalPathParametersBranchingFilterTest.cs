// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.Xml.Linq;
using FluentAssertions;
using Microsoft.OpenApiSpecification.Generation.PreProcessingOperationFilters;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApiSpecification.Generation.Tests.FilterTests
{
    [Collection("DefaultSettings")]
    public class OptionalPathParametersBranchingFilterTest
    {
        private readonly ITestOutputHelper _output;

        public OptionalPathParametersBranchingFilterTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [MemberData(nameof(GetTestCasesForGeneratePathsShouldYieldCorrectPossiblePaths))]
        public void GeneratePathsShouldYieldCorrectPossiblePaths(
            string testName,
            string fullPath,
            IList<XElement> pathParams,
            IList<string> possiblePaths)
        {
            _output.WriteLine(testName);

            var result = ApplyOptionalPathParametersBranchingFilter.GeneratePossiblePaths(
                fullPath,
                pathParams);

            _output.WriteLine(JsonConvert.SerializeObject(result));

            result.Should().BeEquivalentTo(possiblePaths);
        }

        private static IEnumerable<object[]> GetTestCasesForGeneratePathsShouldYieldCorrectPossiblePaths()
        {
            // Simple types

            var pathParams = new[]
            {
                XDocument.Parse(
                        "<param in=\"path\" name=\"optionalA\" type=\"string\" required=\"false\">optional A</param>")
                    .Root,
                XDocument.Parse(
                        "<param in=\"path\" name=\"optionalB\" type=\"string\" required=\"false\">optional B</param>")
                    .Root,
                XDocument.Parse(
                        "<param in=\"path\" name=\"optionalC\" type=\"string\" required=\"false\">optional C</param>")
                    .Root,
                XDocument.Parse(
                        "<param in=\"path\" name=\"optionalD\" type=\"string\" required=\"false\">optional D</param>")
                    .Root,
                XDocument.Parse(
                        "<param in=\"path\" name=\"requiredA\" type=\"string\" required=\"true\">required A</param>")
                    .Root
            };

            // Empty path
            yield return new object[]
            {
                "Empty path",
                "/",
                pathParams,
                new[] {"/"}
            };

            // No optional param
            yield return new object[]
            {
                "No optional param",
                "/AA/BB/{requiredA}/CC",
                pathParams,
                new[] {"/AA/BB/{requiredA}/CC"}
            };

            // One optional param
            yield return new object[]
            {
                "One optional param",
                "/AA/BB/{requiredA}/{optionalA}",
                pathParams,
                new[]
                {
                    "/AA/BB/{requiredA}/{optionalA}",
                    "/AA/BB/{requiredA}"
                }
            };

            // Multiple optional params
            yield return new object[]
            {
                "Multiple optional params",
                "/AA/BB/{requiredA}/{optionalA}/{optionalB}/{optionalC}",
                pathParams,
                new[]
                {
                    "/AA/BB/{requiredA}/{optionalA}/{optionalB}/{optionalC}",
                    "/AA/BB/{requiredA}/{optionalA}/{optionalB}",
                    "/AA/BB/{requiredA}/{optionalA}",
                    "/AA/BB/{requiredA}"
                }
            };

            // Multiple optional params ending with non-param path segment
            yield return new object[]
            {
                "Multiple optional params ending with non-param path segment",
                "/AA/BB/{requiredA}/{optionalA}/{optionalB}/CC",
                pathParams,
                new[]
                {
                    "/AA/BB/{requiredA}/{optionalA}/{optionalB}/CC",
                    "/AA/BB/{requiredA}/{optionalA}/CC",
                    "/AA/BB/{requiredA}/CC"
                }
            };

            // Multiple optional params in multiple chunks
            yield return new object[]
            {
                "Multiple optional params in multiple chunks",
                "/AA/BB/{requiredA}/{optionalA}/{optionalB}/CC/{optionalC}/{optionalD}",
                pathParams,
                new[]
                {
                    "/AA/BB/{requiredA}/{optionalA}/{optionalB}/CC/{optionalC}/{optionalD}",
                    "/AA/BB/{requiredA}/{optionalA}/{optionalB}/CC/{optionalC}",
                    "/AA/BB/{requiredA}/{optionalA}/{optionalB}/CC",
                    "/AA/BB/{requiredA}/{optionalA}/CC/{optionalC}/{optionalD}",
                    "/AA/BB/{requiredA}/{optionalA}/CC/{optionalC}",
                    "/AA/BB/{requiredA}/{optionalA}/CC",
                    "/AA/BB/{requiredA}/CC/{optionalC}/{optionalD}",
                    "/AA/BB/{requiredA}/CC/{optionalC}",
                    "/AA/BB/{requiredA}/CC"
                }
            };

            // Multiple optional params in multiple chunks ending with non-param path segment
            yield return new object[]
            {
                "Multiple optional params in multiple chunks ending with non-param path segment",
                "/AA/BB/{requiredA}/{optionalA}/{optionalB}/CC/{optionalC}/{optionalD}/DD",
                pathParams,
                new[]
                {
                    "/AA/BB/{requiredA}/{optionalA}/{optionalB}/CC/{optionalC}/{optionalD}/DD",
                    "/AA/BB/{requiredA}/{optionalA}/{optionalB}/CC/{optionalC}/DD",
                    "/AA/BB/{requiredA}/{optionalA}/{optionalB}/CC/DD",
                    "/AA/BB/{requiredA}/{optionalA}/CC/{optionalC}/{optionalD}/DD",
                    "/AA/BB/{requiredA}/{optionalA}/CC/{optionalC}/DD",
                    "/AA/BB/{requiredA}/{optionalA}/CC/DD",
                    "/AA/BB/{requiredA}/CC/{optionalC}/{optionalD}/DD",
                    "/AA/BB/{requiredA}/CC/{optionalC}/DD",
                    "/AA/BB/{requiredA}/CC/DD"
                }
            };
        }
    }
}