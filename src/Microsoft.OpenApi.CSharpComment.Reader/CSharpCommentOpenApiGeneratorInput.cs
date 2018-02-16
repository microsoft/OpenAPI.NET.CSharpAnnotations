// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.Xml.Linq;

namespace Microsoft.OpenApi.CSharpComment.Reader
{
    /// <summary>
    /// The class to store the inputs required to generate the open api document from csharp documentation.
    /// </summary>
    public class CSharpCommentOpenApiGeneratorInput
    {
        /// <summary>
        /// Creates a new instance of <see cref="CSharpCommentOpenApiGeneratorInput"/>.
        /// </summary>
        /// <param name="annotationXmlDocument">The XDocument representing the annotation xml.</param>
        /// <param name="contractAssemblyPaths">The list of relative or absolute paths to the contract assemblies.</param>
        /// <param name="openApiSpecVersion">The specification version of the OpenAPI document to generate.</param>
        public CSharpCommentOpenApiGeneratorInput(
            XDocument annotationXmlDocument,
            IList<string> contractAssemblyPaths,
            OpenApiSpecVersion openApiSpecVersion)
        {
            AnnotationXmlDocument = annotationXmlDocument;
            ContractAssemblyPaths = contractAssemblyPaths;
            OpenApiSpecVersion = openApiSpecVersion;
        }

        /// <summary>
        /// The XDocument representing the annotation xml.
        /// </summary>
        public XDocument AnnotationXmlDocument { get; }

        /// <summary>
        /// The XDocument representing the generation configuration.
        /// </summary>
        public XDocument ConfigurationXmlDocument { get; set; }

        /// <summary>
        /// The list of relative or absolute paths to the contract assemblies.
        /// </summary>
        public IList<string> ContractAssemblyPaths { get; }

        /// <summary>
        /// The format (YAML or JSON) of the OpenAPI document to generate.
        /// </summary>
        public OpenApiFormat OpenApiFormat { get; set; } = OpenApiFormat.Json;

        /// <summary>
        /// The specification version of the OpenAPI document to generate.
        /// </summary>
        public OpenApiSpecVersion OpenApiSpecVersion { get; }
    }
}