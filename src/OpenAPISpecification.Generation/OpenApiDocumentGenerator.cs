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

        private readonly InternalOpenApiDocumentGenerator _internalOpenApiDocumentGenerator;

        /// <summary>
        /// Creates new instance of <see cref="OpenApiDocumentGenerator"/> with provided generator settings.
        /// </summary>
        /// <param name="generatorSettings">The generator settings.</param>
        public OpenApiDocumentGenerator(OpenApiDocumentGeneratorSettings generatorSettings)
        {
            _internalOpenApiDocumentGenerator = new InternalOpenApiDocumentGenerator(generatorSettings);
        }

        /// <summary>
        /// Creates new instance of <see cref="OpenApiDocumentGenerator"/> with default generation settings
        /// </summary>
        public OpenApiDocumentGenerator()
        {
            var generatorSettings = new OpenApiDocumentGeneratorSettings(
                _defaultOperationFilters, _defaultDocumentFilters);

            _internalOpenApiDocumentGenerator = new InternalOpenApiDocumentGenerator(generatorSettings);
        }

        /// <summary>
        /// Generates V3 document using the provided xdocument and contract assemblies.
        /// </summary>
        /// <param name="annotationXmlDocument">The XDocument representing the annotation xml.</param>
        /// <param name="contractAssemblyPaths">The list of relative or absolute paths to the contract assemblies.</param>
        /// <returns>The open api document generation result.</returns>
        public OpenApiDocumentGenerationResult GenerateV3Document(
            XDocument annotationXmlDocument,
            IEnumerable<string> contractAssemblyPaths)
        {
            foreach (var contractAssemblyPath in contractAssemblyPaths)
            {
                if (!File.Exists(contractAssemblyPath))
                {
                    throw new FileNotFoundException(contractAssemblyPath);
                }
            }

            return _internalOpenApiDocumentGenerator.GenerateOpenApiDocument(annotationXmlDocument);
        }

        /// <summary>
        /// Generates V3 document using the provided visual studio summary comment and contract assemblies.
        /// </summary>
        /// <param name="summaryComment">The visual studio summary comment.</param>
        /// <param name="contractAssemblyPaths">The list of relative or absolute paths to the contract assemblies.</param>
        /// <returns>The open api document generation result.</returns>
        public OpenApiDocumentGenerationResult GenerateV3Document(
            string summaryComment,
            IEnumerable<string> contractAssemblyPaths)
        {
            return null;
        }
    }
}