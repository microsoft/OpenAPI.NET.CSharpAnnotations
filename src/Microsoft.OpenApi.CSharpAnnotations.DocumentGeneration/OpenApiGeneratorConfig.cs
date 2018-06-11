// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration
{
    /// <summary>
    /// The class to store the configuration that will be used to generate the OpenAPI document from csharp
    /// documentation.
    /// </summary>
    public class OpenApiGeneratorConfig
    {
        private static readonly SchemaGenerationSettings DefaultSchemaGenerationSettings
            = new SchemaGenerationSettings(new DefaultPropertyNameResolver());

        /// <summary>
        /// Creates a new instance of <see cref="OpenApiGeneratorConfig"/>.
        /// </summary>
        /// <param name="annotationXmlDocuments">The XDocuments representing the annotation xmls.</param>
        /// <param name="assemblyPaths">The list of relative or absolute paths to the assemblies that will be used to
        /// reflect into the types provided in the xml.
        /// </param>
        /// <param name="openApiDocumentVersion">The version of the OpenAPI document.</param>
        /// <param name="filterSetVersion">The version of the filter set to use to generate an OpenAPI document.
        /// </param>
        public OpenApiGeneratorConfig(
            IList<XDocument> annotationXmlDocuments,
            IList<string> assemblyPaths,
            string openApiDocumentVersion,
            FilterSetVersion filterSetVersion)
            : this(
                annotationXmlDocuments,
                assemblyPaths,
                openApiDocumentVersion,
                new OpenApiGeneratorFilterConfig(filterSetVersion),
                DefaultSchemaGenerationSettings)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="OpenApiGeneratorConfig"/>.
        /// </summary>
        /// <param name="annotationXmlDocuments">The XDocuments representing the annotation xmls.</param>
        /// <param name="assemblyPaths">The list of relative or absolute paths to the assemblies that will be used to
        /// reflect into the types provided in the xml.
        /// </param>
        /// <param name="openApiDocumentVersion">The version of the OpenAPI document.</param>
        /// <param name="openApiGeneratorFilterConfig">The configuration encapsulating all the filters
        /// that will be applied while generating/processing OpenAPI document from C# annotations.</param>
        /// <param name="schemaGenerationSettings">The settings that will be used while generating schema.</param>
        public OpenApiGeneratorConfig(
            IList<XDocument> annotationXmlDocuments,
            IList<string> assemblyPaths,
            string openApiDocumentVersion,
            OpenApiGeneratorFilterConfig openApiGeneratorFilterConfig,
            SchemaGenerationSettings schemaGenerationSettings)
        {
            AnnotationXmlDocuments = annotationXmlDocuments
                ?? throw new ArgumentNullException(nameof(annotationXmlDocuments));

            AssemblyPaths = assemblyPaths
                ?? throw new ArgumentNullException(nameof(assemblyPaths));

            OpenApiGeneratorFilterConfig = openApiGeneratorFilterConfig
                ?? throw new ArgumentNullException(nameof(openApiGeneratorFilterConfig));

            if (string.IsNullOrWhiteSpace(openApiDocumentVersion))
            {
                throw new ArgumentNullException(nameof(openApiDocumentVersion));
            }

            OpenApiDocumentVersion = openApiDocumentVersion;
            SchemaGenerationSettings = schemaGenerationSettings;
        }

        /// <summary>
        /// The XDocument representing the advanced generation configuration.
        /// </summary>
        public XDocument AdvancedConfigurationXmlDocument { get; set; }

        /// <summary>
        /// The XDocuments representing the annotation xmls.
        /// </summary>
        public IList<XDocument> AnnotationXmlDocuments { get; }

        /// <summary>
        /// The list of relative or absolute paths to the assemblies that will be used to reflect into the
        /// types provided in the xml.
        /// </summary>
        public IList<string> AssemblyPaths { get; }

        /// <summary>
        /// The version of the OpenAPI document.
        /// </summary>
        public string OpenApiDocumentVersion { get; }

        /// <summary>
        /// The configuration encapsulating all the filters that will be applied while generating/processing OpenAPI
        /// document from C# annotations.
        /// </summary>
        public OpenApiGeneratorFilterConfig OpenApiGeneratorFilterConfig { get; }

        /// <summary>
        /// The settings to use while generating schema.
        /// </summary>
        public SchemaGenerationSettings SchemaGenerationSettings { get; }
    }
}