// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Microsoft.OpenApiSpecification.Generation.ConfigFilters;
using Microsoft.OpenApiSpecification.Generation.DocumentConfigFilters;
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
        private static readonly IList<IDocumentConfigFilter> _defaultDocumentConfigFilters =
            new List<IDocumentConfigFilter>
            {
                new ApplyDocumentVariantAttributesFilter()
            };

        private static readonly IList<IDocumentFilter> _defaultDocumentFilters = new List<IDocumentFilter>
        {
            new ApplyAssemblyNameAsInfoFilter(),
            new ApplyUrlAsServerFilter(),
            new ApplyMemberSummaryAsSchemaDescriptionFilter()
        };

        private static readonly IList<IOperationConfigFilter> _defaultOperationConfigFilters =
            new List<IOperationConfigFilter>
            {
                new ApplyCommonAnnotationFilter()
            };

        private static readonly IList<IOperationFilter> _defaultOperationFilters = new List<IOperationFilter>
        {
            new ApplyGroupAsTagFilter(),
            new ApplyParamAsParameterFilter(),
            new ApplyParamAsRequestBodyFilter(),
            new ApplyResponseAsResponseFilter(),
            new ApplyRemarksAsDescriptionFilter(),
            new ApplySummaryFilter()
        };

        // TO DO: Figure out a way to serialize this and pass as parameter from OpenApiDocumentGenerator.
        private readonly OpenApiDocumentGeneratorConfig _generatorConfig = new OpenApiDocumentGeneratorConfig(
            _defaultOperationFilters,
            _defaultDocumentFilters,
            _defaultOperationConfigFilters,
            _defaultDocumentConfigFilters);

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
                _defaultDocumentFilters,
                _defaultOperationConfigFilters,
                _defaultDocumentConfigFilters);
        }

        /// <summary>
        /// Generates V3 documents using the provided xdocument and contract assemblies.
        /// </summary>
        /// <param name="annotationXmlDocument">The XDocument representing the annotation xml.</param>
        /// <param name="contractAssemblyPaths">The list of relative or absolute paths to the contract assemblies.</param>
        /// <param name="configurationXmlDocument">The XDocument representing the generation configuration.</param>
        /// <returns>The open api document generation result.</returns>
        public DocumentGenerationResult GenerateV3Documents(
            XDocument annotationXmlDocument,
            IList<string> contractAssemblyPaths,
            XDocument configurationXmlDocument)
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
                    contractAssemblyPaths,
                    configurationXmlDocument?.ToString());

                return JsonConvert.DeserializeObject<DocumentGenerationResult>(result);
            }
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
            return GenerateV3Documents(
                annotationXmlDocument,
                contractAssemblyPaths,
                configurationXmlDocument: null);
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