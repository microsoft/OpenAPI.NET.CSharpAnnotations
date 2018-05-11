// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using FluentAssertions;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.OperationFilters;
using Microsoft.OpenApi.Models;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.FilterTests
{
    [Collection("DefaultSettings")]
    public class RemarksToDescriptionFilterTest
    {
        private const string InputDirectory = "FilterTests/Input";
        private readonly ITestOutputHelper _output;

        public RemarksToDescriptionFilterTest(ITestOutputHelper output)
        {
            _output = output;
        }

        public static IEnumerable<object[]> GetTestCasesForRemarksToDescriptionFilterShouldSucceed()
        {
            // Remarks containing CDATA
            yield return new object[]
            {
                "Remarks containing CDATA",
                XElement.Load(Path.Combine(InputDirectory, "RemarksContainingCData.xml")),
                new OpenApiOperation
                {
                    Description = "See our documentation\r\n     "
                        + "<a href=\"https://github.com/Microsoft/OpenAPI.NET.CSharpAnnotations/tree/master/test/Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.SampleApis\">"
                        + "here</a>\r\n    for more details."
                }
            };
        }

        [Theory]
        [MemberData(nameof(GetTestCasesForRemarksToDescriptionFilterShouldSucceed))]
        public void RemarksToDescriptionFilterShouldSucceed(
            string testName,
            XElement xElement,
            OpenApiOperation expectedOpenApiOperation)
        {
            var filter = new RemarksToDescriptionFilter();
            var settings = new OperationFilterSettings();

            var openApiOperation = new OpenApiOperation();
            _output.WriteLine(testName);

            filter.Apply(openApiOperation, xElement, settings);

            // Operation should be populated with description
            openApiOperation.Should().BeEquivalentTo(expectedOpenApiOperation);
        }
    }
}