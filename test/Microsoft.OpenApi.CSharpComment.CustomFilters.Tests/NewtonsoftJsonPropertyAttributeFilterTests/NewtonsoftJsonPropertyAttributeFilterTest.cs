// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using FluentAssertions;
using Microsoft.OpenApi.CSharpComment.CustomFilters;
using Microsoft.OpenApi.CSharpComment.Reader;
using Microsoft.OpenApi.CSharpComment.Reader.DocumentFilters;
using Microsoft.OpenApi.CSharpComment.Reader.Models;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.CSharpComment.CustomFilter.Tests.NewtonsoftJsonPropertyAttributeFilterTests
{
    [Collection("DefaultSettings")]
    public class NewtonsoftJsonPropertyAttributeFilterTest
    {
        private const string InputDirectory = "NewtonsoftJsonPropertyAttributeFilterTests/Input";
        private const string OutputDirectory = "NewtonsoftJsonPropertyAttributeFilterTests/Output";

        private readonly ITestOutputHelper _output;

        public NewtonsoftJsonPropertyAttributeFilterTest(ITestOutputHelper output)
        {
            _output = output;
        }

        public static IEnumerable<object[]> GetTestCasesForApplyFilterAndShouldReturnCorrectDocument()
        {
            // Standard, original valid XML document
            yield return new object[]
            {
                "Standard valid Open Api document should be updated with JSON Property attribute",
                new List<string>
                {
                    Path.Combine(InputDirectory, "Annotation.xml"),
                    Path.Combine(InputDirectory, "Microsoft.OpenApi.CSharpComment.Reader.Tests.Contracts.xml")
                },
                new List<string>
                {
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.SampleApis.dll"),
                    Path.Combine(
                        InputDirectory,
                        "Microsoft.OpenApi.CSharpComment.Reader.Tests.Contracts.dll")
                },
                Path.Combine(
                    InputDirectory,
                    "Annotation.Json"),
                Path.Combine(
                    OutputDirectory,
                    "AnnotationUpdatedWithJsonProperty.Json")
            };
        }

        [Theory]
        [MemberData(nameof(GetTestCasesForApplyFilterAndShouldReturnCorrectDocument))]
        public void ApplyFilterAndShouldReturnCorrectDocument(
           string testCaseName,
           IList<string> inputXmlFiles,
           IList<string> inputBinaryFiles,
           string jsonFileToUpdate,
           string expectedJsonFile)
        {
            _output.WriteLine(testCaseName);

            var documents = new List<XDocument>();

            documents.AddRange(inputXmlFiles.Select(XDocument.Load));
            var openApiStringReader = new OpenApiStringReader();

            var typeFetcher = new TypeFetcher(inputBinaryFiles);
            var settings = new DocumentFilterSettings
            {
                TypeFetcher = typeFetcher
            };

            var filter = new NewtonsoftJsonPropertyAttributeFilter();
            var actualDocument = openApiStringReader.Read(File.ReadAllText(jsonFileToUpdate), out var _);
            filter.Apply(actualDocument, documents, settings);

            var actualDocumentAsString = actualDocument.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
            var expectedDocument = File.ReadAllText(expectedJsonFile);

            _output.WriteLine(actualDocumentAsString);

            // We are doing serialization and deserialization to force the resulting actual document
            // to have the exact fields we will see in the resulting document based on the contract resolver.
            // Without serialization and deserialization, the actual document may have fields that should
            // not be present, such as empty list fields.
            var actualDeserializedDocument = openApiStringReader.Read(
                actualDocumentAsString,
                out OpenApiDiagnostic diagnostic);

            actualDeserializedDocument
                .Should()
                .BeEquivalentTo(openApiStringReader.Read(expectedDocument, out var _));
        }
    }
}