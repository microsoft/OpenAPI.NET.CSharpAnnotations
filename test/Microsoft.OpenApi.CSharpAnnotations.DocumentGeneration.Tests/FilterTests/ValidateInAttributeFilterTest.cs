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
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models.KnownStrings;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.PreprocessingOperationFilters;
using Microsoft.OpenApi.Models;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.FilterTests
{
    [Collection("DefaultSettings")]
    public class ValidateInAttributeFilterTest
    {
        private const string InputDirectory = "FilterTests/Input";
        private readonly ITestOutputHelper _output;

        public ValidateInAttributeFilterTest(ITestOutputHelper output)
        {
            _output = output;
        }

        public static IEnumerable<object[]> GetTestCasesForValidateInAttributeShouldFail()
        {
            // Param with not supported In value
            yield return new object[]
            {
                "Param with not supported in value",
                XElement.Load(Path.Combine(InputDirectory, "ParamWithNotSupportedInValue.xml")),
                new List<GenerationError>
                {
                    new GenerationError
                    {
                        ExceptionType = nameof(NotSupportedInAttributeValueException),
                        Message = string.Format(
                    SpecificationGenerationMessages.NotSupportedInAttributeValue,
                    "sampleHeaderParam2, sampleHeaderParam3",
                    "notSupported, notSupported2",
                    string.Join(", ", KnownXmlStrings.AllowedInValues))
                    }
                }
                
            };
        }

        [Theory]
        [MemberData(nameof(GetTestCasesForValidateInAttributeShouldFail))]
        public void ValidateInAttributeShouldGenerateErrors(
            string testName,
            XElement xElement,
            IList<GenerationError> expectedGenerationErrors)
        {
            var filter = new ValidateInAttributeFilter();
            var settings = new PreProcessingOperationFilterSettings();

            var openApiPaths = new OpenApiPaths();
            _output.WriteLine(testName);

            var actualGenerationErrors = filter.Apply(openApiPaths, xElement, settings);
            actualGenerationErrors.Should().BeEquivalentTo(expectedGenerationErrors);
        }
    }
}