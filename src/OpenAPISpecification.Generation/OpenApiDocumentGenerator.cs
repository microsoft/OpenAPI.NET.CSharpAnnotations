// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Microsoft.OpenApiSpecification.Generation.DocumentFilters;
using Microsoft.OpenApiSpecification.Generation.Models;
using Microsoft.OpenApiSpecification.Generation.OperationFilters;
using Newtonsoft.Json;

namespace Microsoft.OpenApiSpecification.Generation
{
    /// <summary>
    /// The class that holds functionality to generate open api document.
    /// </summary>
    public class OpenApiDocumentGenerator : IOpenApiDocumentGenerator
    {
        private readonly IList<IDocumentFilter> _defaultDocumentFilters = new List<IDocumentFilter>
        {
            new ApplyAssemblyNameAsInfoFilter(),
            new ApplyUrlAsServerFilter()
        };

        private readonly IList<IOperationFilter> _defaultOperationFilters = new List<IOperationFilter>
        {
            new ApplyGroupsAsTagFilter(),
            new ApplyParamAsParameterFilter(),
            new ApplyRemarksAsDescriptionFilter(),
            new ApplySummaryFilter()
        };

        private readonly OpenApiDocumentGeneratorConfig _generatorConfig;

        /// <summary>
        /// Creates new instance of <see cref="OpenApiDocumentGenerator"/> with provided generator settings.
        /// </summary>
        /// <param name="generatorConfig">The generator settings.</param>
        public OpenApiDocumentGenerator(OpenApiDocumentGeneratorConfig generatorConfig)
        {
            _generatorConfig = generatorConfig;
        }

        /// <summary>
        /// Creates new instance of <see cref="OpenApiDocumentGenerator"/> with default generation settings
        /// </summary>
        public OpenApiDocumentGenerator()
        {
            _generatorConfig = new OpenApiDocumentGeneratorConfig(
                _defaultOperationFilters,
                _defaultDocumentFilters);
        }

        /// <summary>
        /// Generates V3 documents using the provided xdocument and contract assemblies.
        /// </summary>
        /// <param name="annotationXmlDocument">The XDocument representing the annotation xml.</param>
        /// <param name="contractAssemblyPaths">The list of relative or absolute paths to the contract assemblies.</param>
        /// <returns>The open api document generation result.</returns>
        public DocumentGenerationResult GenerateV3Documents(
            XDocument annotationXmlDocument,
            IList<string> contractAssemblyPaths)
        {
            foreach (var contractAssemblyPath in contractAssemblyPaths)
            {
                if (!File.Exists(contractAssemblyPath))
                {
                    throw new FileNotFoundException(contractAssemblyPath);
                }
            }

            using (var isolatedDomain = new AppDomainCreator<InternalOpenApiDocumentGenerator>())
            {
                var result = isolatedDomain.Object.GenerateOpenApiDocuments(
                    annotationXmlDocument.ToString(),
                    contractAssemblyPaths);

                return JsonConvert.DeserializeObject<DocumentGenerationResult>(result);
            }
        }

        /// <summary>
        /// Generates V3 documents using the provided visual studio summary comment and contract assemblies.
        /// </summary>
        /// <param name="summaryComment">The visual studio summary comment.</param>
        /// <param name="contractAssemblyPaths">The list of relative or absolute paths to the contract assemblies.</param>
        /// <returns>The open api document generation result.</returns>
        public DocumentGenerationResult GenerateV3Documents(
            string summaryComment,
            IList<string> contractAssemblyPaths)
        {
            return null;
        }
    }
}