// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using FluentAssertions;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Exceptions;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models.KnownStrings;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.PreprocessingOperationFilters;
using Microsoft.OpenApi.Models;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.FilterTests
{
    [Collection("DefaultSettings")]
    public class PopulateInAttributeFilterTest
    {
        private const string InputDirectory = "FilterTests/Input";
        private readonly ITestOutputHelper _output;

        public PopulateInAttributeFilterTest(ITestOutputHelper output)
        {
            _output = output;
        }

        public static IEnumerable<object[]> GetTestCasesForPopulateInAttributeShouldFail()
        {
            // Param missing In attribute
            yield return new object[]
            {
                "Param missing in attribute",
                XElement.Load(Path.Combine(InputDirectory, "ParamMissingInAttribute.xml")),
                string.Format(
                    SpecificationGenerationMessages.MissingInAttribute,
                    "sampleHeaderParam2, sampleHeaderParam3")
            };

            // Param with not supported In value
            yield return new object[]
            {
                "Param with not supported in value",
                XElement.Load(Path.Combine(InputDirectory, "ParamWithNotSupportedInValue.xml")),
                string.Format(
                    SpecificationGenerationMessages.NotSupportedInAttributeValue,
                    "notSupported, notSupported2",
                    "sampleHeaderParam2, sampleHeaderParam3",
                    string.Join(", ", KnownXmlStrings.AllowedInValues))
            };
        }

        [Theory]
        [MemberData(nameof(GetTestCasesForPopulateInAttributeShouldFail))]
        public void PopulateInAttributeShouldSucceed(
            string testName,
            XElement xElement,
            string expectedExceptionMessage)
        {
            var filter = new PopulateInAttributeFilter();
            var settings = new PreProcessingOperationFilterSettings();

            var openApiPaths = new OpenApiPaths();
            _output.WriteLine(testName);

            Action action = () => filter.Apply(openApiPaths, xElement, settings);
            action.Should().Throw<DocumentationException>(expectedExceptionMessage);
        }
    }
}