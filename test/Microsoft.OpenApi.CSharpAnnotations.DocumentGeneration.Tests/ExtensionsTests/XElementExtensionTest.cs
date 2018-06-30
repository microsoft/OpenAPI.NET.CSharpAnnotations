// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.ExtensionsTests
{
    [Collection("DefaultSettings")]
    public class XElementExtensionTest
    {
        private const string InputDirectory = "ExtensionsTests/Input";
        private readonly ITestOutputHelper _output;
        private readonly TypeFetcher typeFetcher = new TypeFetcher(
            new List<string>() { Path.Combine(
                InputDirectory,
                "Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.Contracts.dll") });

        public XElementExtensionTest(ITestOutputHelper output)
        {
            _output = output;
        }

        public static IEnumerable<object[]> GetTestCasesForXElementExampleExtensionShouldSucceed()
        {
            yield return new object[]
            {
                "Empty example element",
                XElement.Parse("<example></example>"),
                null
            };

            yield return new object[]
            {
                "Example with url",
                XElement.Parse(
                    "<example><summary>Test Example</summary><url>https://localhost/test.json</url></example>"),
                new OpenApiExample
                {
                    Summary = "Test Example",
                    ExternalValue = "https://localhost/test.json"
                }
            };

            yield return new object[]
            {
                "Example element with cref",
                XElement.Parse(@"<example><value><see cref=""F:Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.Contracts.Examples.SampleObject1Example""/></value></example>"),
                new OpenApiExample
                {
                    Value = new OpenApiStringReader()
                    .ReadFragment<IOpenApiAny>(
                        ExpectedExamples.SampleObject1Example,
                        OpenApiSpecVersion.OpenApi3_0,
                        out OpenApiDiagnostic _)
                }
            };

            yield return new object[]
            {
                "Example element with inline value",
                XElement.Parse(@"<example><value>Test Example</value></example>"),
                new OpenApiExample
                {
                    Value = new OpenApiString("Test Example")
                }
            };
        }

        public static IEnumerable<object[]> GetTestCasesForXElementExampleExtensionShouldFail()
        {
            yield return new object[]
            {
                "Example element contain both value and url.",
                XElement.Parse("<example><value></value><url></url></example>"),
                SpecificationGenerationMessages.ProvideEitherValueOrUrlTag
            };

            yield return new object[]
            {
                "Example value with no cref and value.",
                XElement.Parse(
                    @"<example><summary>Test Example</summary><value></value></example>"),
                SpecificationGenerationMessages.ProvideValueForExample
            };

            yield return new object[]
            {
                "Example element with cref containing type that doesn't exists in provided assembly.",
                XElement.Parse(@"<example><value><see cref=""F:Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration."
                + @"Tests.DoesnotExists.DoesnotExists""/></value></example>"),
                "Type \"Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.DoesnotExists\" could not be found."
                + " Ensure that it exists in one of the following assemblies: "
                + "Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.Contracts.dll"
            };

            yield return new object[]
            {
                "Example element with cref containing filed that doesn't exists in provided type.",
                XElement.Parse(@"<example><value><see cref=""F:Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration."
                + @"Tests.Contracts.Examples.DoesNotExists""/></value></example>"),
                "Field \"DoesNotExists\" could not be found for type: "
                + "\"Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.Contracts.Examples\"."
            };
        }

        [Theory]
        [MemberData(nameof(GetTestCasesForXElementExampleExtensionShouldFail))]
        public void XElementExampleExtensionShouldFail(
            string testCaseName,
            XElement xElement,
            string expectedExceptionMessage)
        {
            _output.WriteLine(testCaseName);

            Action action = () => xElement.ToOpenApiExample(typeFetcher);
            action.Should().Throw<Exception>(expectedExceptionMessage);
        }

        [Theory]
        [MemberData(nameof(GetTestCasesForXElementExampleExtensionShouldSucceed))]
        public void XElementExampleExtensionShouldSucceed(
            string testCaseName,
            XElement xElement,
            OpenApiExample expectedOpenApiExample)
        {
            _output.WriteLine(testCaseName);

            var openApiExample = xElement.ToOpenApiExample(typeFetcher);
            openApiExample.Should().BeEquivalentTo(expectedOpenApiExample);
        }
    }
}