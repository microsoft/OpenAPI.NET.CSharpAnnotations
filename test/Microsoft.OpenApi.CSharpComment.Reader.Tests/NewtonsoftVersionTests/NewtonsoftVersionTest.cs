// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using FluentAssertions;
using Microsoft.OpenApi.CSharpComment.Reader.Models;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Readers;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.CSharpComment.Reader.Tests.NewtonsoftVersionTests
{
    [Collection("DefaultSettings")]
    public class NewtonsoftVersionTest
    {
        private const string InputDirectory = "NewtonsoftVersionTests/Input";
        private const string OutputDirectory = "NewtonsoftVersionTests/Output";

        private readonly ITestOutputHelper _output;

        public NewtonsoftVersionTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData("10.0.3", OpenApiSpecVersion.OpenApi3_0_0)]
        [InlineData("9.0.1", OpenApiSpecVersion.OpenApi3_0_0)]
        [InlineData("8.0.3", OpenApiSpecVersion.OpenApi3_0_0)]
        [InlineData("7.0.1", OpenApiSpecVersion.OpenApi3_0_0)]
        public void DocumentGenerationWithDllsReferencingAnyNewtonsoftVersionShouldSucceed(
            string newtonsoftVersion,
            OpenApiSpecVersion openApiSpecVersion)
        {
            _output.WriteLine(newtonsoftVersion);

            var document = XDocument.Load(Path.Combine(InputDirectory, "Annotation.xml"));

            var generator = new OpenApiDocumentGenerator();

            var result = generator.GenerateOpenApiDocuments(
                document,
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        $"V{newtonsoftVersion}",
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.SampleApis.dll")
                },
                openApiSpecVersion);

            result.Should().NotBeNull();

            _output.WriteLine(
                JsonConvert.SerializeObject(
                    result.ToDocumentGenerationResultSerializedDocument(openApiSpecVersion, OpenApiFormat.Json)));

            result.GenerationStatus.Should().Be(GenerationStatus.Success);
            result.MainDocument.Should().NotBeNull();
            result.OperationGenerationResults.Count.Should().Be(9);

            var actualDocument = result.MainDocument.SerializeAsJson(openApiSpecVersion);

            var expectedDocument = File.ReadAllText(
                Path.Combine(
                    OutputDirectory,
                    "Annotation.Json"));

            _output.WriteLine(actualDocument);

            var openApiStringReader = new OpenApiStringReader();
            openApiStringReader.Read(actualDocument, out var _)
                .Should()
                .BeEquivalentTo(openApiStringReader.Read(expectedDocument, out var _));
        }
    }
}