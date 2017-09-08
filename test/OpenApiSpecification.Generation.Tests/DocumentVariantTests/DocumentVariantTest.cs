// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Microsoft.OpenApiSpecification.Core.Models;
using Microsoft.OpenApiSpecification.Core.Serialization;
using Microsoft.OpenApiSpecification.Generation.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Microsoft.OpenApiSpecification.Generation.Tests.DocumentVariantTests
{
    [TestClass]
    public class DocumentVariantTest
    {
        private const string TestFilesDirectory = "DocumentVariantTests/TestFiles";
        private const string TestValidationDirectory = "DocumentVariantTests/TestValidation";

        [TestMethod]
        public void GenerateV3DocumentMultipleVariantsShouldSucceed()
        {
            // Arrange
            var path = Path.Combine(TestFilesDirectory, "Annotation.xml");
            var document = XDocument.Load(path);

            var generator = new OpenApiDocumentGenerator();

            var variantInfoDefault = DocumentVariantInfo.Default;

            var variantInfo1 = new DocumentVariantInfo
            {
                Categorizer = "swagger",
                Title = "Variant1"
            };

            var variantInfo2 = new DocumentVariantInfo
            {
                Categorizer = "swagger",
                Title = "Variant2"
            };

            // Act
            var result = generator.GenerateV3Documents(
                document,
                new List<string> {Path.Combine(TestFilesDirectory, "NativeXml.exe")});

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(GenerationStatus.Success, result.GenerationStatus);

            Assert.AreEqual(3, result.Documents.Keys.Count);
            Assert.AreEqual(7, result.PathGenerationResults.Count);

            OpenApiV3SpecificationDocument specificationDocument;
            string actualDocument;
            string expectedDocument;

            // Default Document Variant
            result.Documents.TryGetValue(variantInfoDefault, out specificationDocument);
            Assert.IsNotNull(specificationDocument);

            actualDocument = JsonConvert.SerializeObject(
                specificationDocument,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()}
            );

            expectedDocument = File.ReadAllText(Path.Combine(TestValidationDirectory, "AnnotationDefaultVariant.json"));

            Assert.IsTrue(TestHelper.AreJsonEqual(expectedDocument, actualDocument));

            // Document Variant 1
            // Only contains info from operations with tags with title "Variant1" in Annotation.xml 
            result.Documents.TryGetValue(variantInfo1, out specificationDocument);
            Assert.IsNotNull(specificationDocument);

            actualDocument = JsonConvert.SerializeObject(
                specificationDocument,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()}
            );

            expectedDocument = File.ReadAllText(Path.Combine(TestValidationDirectory, "AnnotationVariant1.json"));

            Assert.IsTrue(TestHelper.AreJsonEqual(expectedDocument, actualDocument));

            // Document Variant 2
            // Only contains info from operations with tags with title "Variant2" in Annotation.xml 
            result.Documents.TryGetValue(variantInfo2, out specificationDocument);
            Assert.IsNotNull(specificationDocument);

            actualDocument = JsonConvert.SerializeObject(
                specificationDocument,
                new JsonSerializerSettings {ContractResolver = new EmptyCollectionContractResolver()}
            );

            expectedDocument = File.ReadAllText(Path.Combine(TestValidationDirectory, "AnnotationVariant2.json"));

            Assert.IsTrue(TestHelper.AreJsonEqual(expectedDocument, actualDocument));
        }
    }
}