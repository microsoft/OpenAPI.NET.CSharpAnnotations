// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using FluentAssertions;
using Microsoft.OpenApi.CSharpComment.Reader.Models;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Readers;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.CSharpComment.Reader.Tests.SampleServiceTests
{
    [Collection("DefaultSettings")]
    public class SampleServiceTest
    {
        private const string InputDirectory = "SampleServiceTests/Input";
        private const string OutputDirectory = "SampleServiceTests/Output";

        private readonly ITestOutputHelper _output;

        public SampleServiceTest(ITestOutputHelper output)
        {
            _output = output;
        }

        public static IEnumerable<object[]> GetTestCasesForSampleServiceShouldPassGeneration()
        {
            //// Swagger Sample
            //yield return new object[]
            //{
            //    "Swagger Sample",
            //    Path.Combine(InputDirectory, "SwaggerSample", "NativeXml.xml"),
            //    new List<string>
            //    {
            //        Path.Combine(
            //            InputDirectory,
            //            "SwaggerSample",
            //            "NativeXml.exe")
            //    },
            //    OpenApiSpecVersion.OpenApi3_0_0,
            //    7,
            //    Path.Combine(
            //        OutputDirectory,
            //        "SwaggerSample",
            //        "Annotation.Json")
            //};

            //// DCat FD
            //yield return new object[]
            //{
            //    "DCat FD",
            //    Path.Combine(InputDirectory, "DCatFD", "DCatFD.xml"),
            //    new List<string>
            //    {
            //        Path.Combine(
            //            InputDirectory,
            //            "DCatFD",
            //            "Microsoft.MarketplaceServices.DisplayCatalog.Contracts.dll"),
            //        Path.Combine(
            //            InputDirectory,
            //            "DCatFD",
            //            "Microsoft.MarketplaceServices.DisplayCatalog.DCatFDPricingContracts.dll"),
            //        Path.Combine(
            //            InputDirectory,
            //            "DCatFD",
            //            "DCatFD.exe"),
            //    },
            //    OpenApiSpecVersion.OpenApi3_0_0,
            //    28,
            //    Path.Combine(
            //        OutputDirectory,
            //        "DCatFD",
            //        "Annotation.Json")
            //};

            // StoreEdge FD
            yield return new object[]
            {
                "StoreEdge FD",
                Path.Combine(InputDirectory, "StoreEdgeFD", "StoreEdgeFD.xml"),
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "StoreEdgeFD",
                        "Microsoft.Marketplace.Storefront.Contracts.dll"),
                },
                OpenApiSpecVersion.OpenApi3_0_0,
                28,
                Path.Combine(
                    OutputDirectory,
                    "StoreEdgeFD",
                    "Annotation.Json")
            };
        }

        [Theory]
        [MemberData(nameof(GetTestCasesForSampleServiceShouldPassGeneration))]
        public void SampleServiceShouldPassGeneration(
            string testCaseName,
            string inputXmlFile,
            IList<string> inputBinaryFiles,
            OpenApiSpecVersion openApiSpecVersion,
            int expectedPathGenerationResultsCount,
            string expectedJsonFile)
        {
            _output.WriteLine(testCaseName);

            var document = XDocument.Load(inputXmlFile);

            var generator = new OpenApiDocumentGenerator();

            var result = generator.GenerateOpenApiDocuments(
                document,
                inputBinaryFiles,
                openApiSpecVersion);

            result.Should().NotBeNull();

            _output.WriteLine(
                JsonConvert.SerializeObject(
                    result.ToDocumentGenerationResultSerializedDocument(openApiSpecVersion)));

            result.MainDocument.Should().NotBeNull();

            var actualDocument = result.MainDocument.SerializeAsJson(openApiSpecVersion);

            var expectedDocument = File.ReadAllText(expectedJsonFile, Encoding.Default);

            _output.WriteLine(actualDocument);
            
            var openApiStringReader = new OpenApiStringReader();
            openApiStringReader.Read(actualDocument, out var _)
                .Should()
                .BeEquivalentTo(
                    openApiStringReader.Read(expectedDocument, out var _));

            result.GenerationStatus.Should().Be(GenerationStatus.Success);
            result.PathGenerationResults.Count.Should().Be(expectedPathGenerationResultsCount);
        }
    }
}