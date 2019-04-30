// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Extensions;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models.KnownStrings;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.ReferenceRegistries;
using Microsoft.OpenApi.Extensions;
using Newtonsoft.Json;

#if NETCORE
using System.Runtime.Loader;
#endif

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.AssemblyLoader
{
#if !NETCORE
    internal class AssemblyLoader : MarshalByRefObject
    {
#else
    internal class AssemblyLoader
    {
        public CustomAssemblyLoadContext Context { get; }

        public AssemblyLoader()
        {
            Context = new CustomAssemblyLoadContext();
            AssemblyLoadContext.Default.Resolving += (context, name) => Context.Resolve(name);
        }
#endif

        internal void RegisterAssemblyPaths(IList<string> assemblyPaths)
        {
#if NETCORE
            Context.AssemblyPaths = assemblyPaths;
#else
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                Version version = null;

                var separatorIndex = args.Name.IndexOf(",", StringComparison.Ordinal);
                var assemblyName = separatorIndex > 0 ? args.Name.Substring(0, separatorIndex) : args.Name;

                if (separatorIndex > 0)
                {
                    separatorIndex = args.Name.IndexOf("=", separatorIndex, StringComparison.Ordinal);
                    if (separatorIndex > 0)
                    {
                        var endIndex = args.Name.IndexOf(",", separatorIndex, StringComparison.Ordinal);
                        if (endIndex > 0)
                        {
                            var parts = args.Name
                                .Substring(separatorIndex + 1, endIndex - separatorIndex - 1)
                                .Split('.');

                            version = new Version(int.Parse(parts[0]),
                                int.Parse(parts[1]),
                                int.Parse(parts[2]),
                                int.Parse(parts[3]));
                        }
                    }
                }

                var existingAssembly = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.FullName == args.Name);
                if (existingAssembly != null)
                {
                    return existingAssembly;
                }

                if (version != null)
                {
                    var assemblyByVersion = AssemblyLoadUtility.TryLoadByVersion(
                        assemblyPaths,
                        assemblyName,
                        version.Major + "." + version.Minor + "." + version.Build + ".");

                    if (assemblyByVersion != null)
                    {
                        return assemblyByVersion;
                    }

                    assemblyByVersion = AssemblyLoadUtility.TryLoadByVersion(
                        assemblyPaths,
                        assemblyName,
                        version.Major + "." + version.Minor + ".");

                    if (assemblyByVersion != null)
                    {
                        return assemblyByVersion;
                    }

                    assemblyByVersion =
                        AssemblyLoadUtility.TryLoadByVersion(assemblyPaths, assemblyName, version.Major + ".");

                    if (assemblyByVersion != null)
                    {
                        return assemblyByVersion;
                    }
                }

                var assembly = AssemblyLoadUtility.TryLoadByName(assemblyPaths, assemblyName);
                if (assembly != null)
                {
                    return assembly;
                }

                assembly = AssemblyLoadUtility.TryLoadByName(assemblyPaths, assemblyName);
                if (assembly != null)
                {
                    return assembly;
                }

                return null;
            };
#endif
        }

        /// <summary>
        /// Builds <see cref="SchemaTypeInfo"/> by reflecting into contract assemblies.
        /// </summary>
        /// <param name="contractAssembliesPaths"></param>
        /// <param name="operationElements"></param>
        /// <param name="propertyElements"></param>
        /// <param name="documentVariantElementName"></param>
        /// <param name="internalSchemaGenerationSettings"></param>
        /// <returns>Serialized <see cref="SchemaTypeInfo"/></returns>
        public string BuildSchemaTypeInfo(
            IList<string> contractAssembliesPaths,
            IList<string> operationElements,
            IList<string> propertyElements,
            string documentVariantElementName,
            InternalSchemaGenerationSettings internalSchemaGenerationSettings)
        {
            var crefSchemaMap = new Dictionary<string, SchemaInfo>();

            List<XElement> xPropertyElements = propertyElements.Select(XElement.Parse).ToList();

            var propertyMap = new Dictionary<string, string>();

            foreach (var xPropertyElement in xPropertyElements)
            {
                var name = xPropertyElement
                    .Attributes(KnownXmlStrings.Name)?.FirstOrDefault()?.Value?.Split(':')[1];
                var description = xPropertyElement.Element(KnownXmlStrings.Summary)?.Value.RemoveBlankLines();

                if (!propertyMap.ContainsKey(name))
                {
                    propertyMap.Add(name, description);
                }
            }

            var referenceRegistryMap = new Dictionary<DocumentVariantInfo, SchemaReferenceRegistry>();

            var schemaGenerationSettings = new SchemaGenerationSettings(new DefaultPropertyNameResolver());

            if (internalSchemaGenerationSettings.PropertyNameResolverName ==
                 typeof(CamelCasePropertyNameResolver).FullName)
            {
                schemaGenerationSettings = new SchemaGenerationSettings(new CamelCasePropertyNameResolver());
            }

            var schemaTypeInfo = new SchemaTypeInfo();

#if !NETCORE
            var typeFetcher = new TypeFetcher(contractAssembliesPaths);
#else
            var typeFetcher = new TypeFetcher(contractAssembliesPaths, Context);
#endif

            List<XElement> xOperationElements = operationElements.Select(XElement.Parse).ToList();

            foreach (var xOperationElement in xOperationElements)
            {
                if(!referenceRegistryMap.ContainsKey(DocumentVariantInfo.Default))
                {
                    referenceRegistryMap.Add(
                        DocumentVariantInfo.Default,
                        new SchemaReferenceRegistry(schemaGenerationSettings, propertyMap));
                }

                BuildMap(
                    xOperationElement,
                    crefSchemaMap,
                    schemaTypeInfo.CrefToFieldValueMap,
                    typeFetcher,
                    referenceRegistryMap[DocumentVariantInfo.Default]);

                var customElements = xOperationElement.Descendants(documentVariantElementName);

                foreach (var customElement in customElements)
                {
                    var documentVariantInfo = new DocumentVariantInfo
                    {
                        Categorizer = customElement.Name.LocalName.Trim(),
                        Title = customElement.Value.Trim()
                    };

                    if (!referenceRegistryMap.ContainsKey(documentVariantInfo))
                    {
                        referenceRegistryMap.Add(
                            documentVariantInfo,
                            new SchemaReferenceRegistry(schemaGenerationSettings, propertyMap));
                    }

                    BuildMap(
                        xOperationElement,
                        crefSchemaMap,
                        schemaTypeInfo.CrefToFieldValueMap,
                        typeFetcher,
                        referenceRegistryMap[documentVariantInfo]);
                }          
            }

            foreach(var key in referenceRegistryMap.Keys)
            {
                var references = referenceRegistryMap[key].References;

                schemaTypeInfo.VariantSchemaReferenceMap.Add(
                    key.ToString(),
                    references.ToDictionary(k => k.Key, k => k.Value.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0)));
            }

            schemaTypeInfo.CrefToSchemaMap = crefSchemaMap.ToDictionary(
                k => k.Key,
                k => JsonConvert.SerializeObject(k.Value));

            // Fetch all the property members
            return JsonConvert.SerializeObject(schemaTypeInfo);
        }

        private void BuildMap(
            XElement currentXElement,
            Dictionary<string, SchemaInfo> crefSchemaMap,
            Dictionary<string, string> crefFieldMap,
            TypeFetcher typeFetcher,
            SchemaReferenceRegistry schemaReferenceRegistry)
        {
            if (currentXElement == null)
            {
                return;
            }

            var cref = currentXElement.Attribute(KnownXmlStrings.Cref)?.Value.Trim();

            if (!string.IsNullOrWhiteSpace(cref))
            {
                var allListedTypes = new List<string>() { cref };

                if (allListedTypes.Where(i => i.StartsWith("F:")).Any())
                {
                    BuildCrefFieldValueMap(allListedTypes.FirstOrDefault(), crefFieldMap, typeFetcher);
                }
                else
                {
                    BuildCrefSchemaMap(allListedTypes, crefSchemaMap, typeFetcher, schemaReferenceRegistry);
                }
            }

            if (!currentXElement.Elements().Any())
            {
                return;
            }

            var seeNodes = currentXElement.Elements().Where(i => i.Name == KnownXmlStrings.See);

            if (seeNodes.Any())
            {
                var allListedTypes = seeNodes
                    .Select(node => node.Attribute(KnownXmlStrings.Cref)?.Value)
                    .Where(crefValue => crefValue != null).ToList();

                if (allListedTypes.Where(i => i.StartsWith("F:")).Any())
                {
                    BuildCrefFieldValueMap(allListedTypes.FirstOrDefault(), crefFieldMap, typeFetcher);
                }
                else
                {
                    BuildCrefSchemaMap(allListedTypes, crefSchemaMap, typeFetcher, schemaReferenceRegistry);
                }
            }

            var remainingNodes = currentXElement.Elements().Where(i => i.Name != KnownXmlStrings.See);

            foreach (var remainingNode in remainingNodes)
            {
                BuildMap(
                    remainingNode,
                    crefSchemaMap,
                    crefFieldMap,
                    typeFetcher,
                    schemaReferenceRegistry);
            }
        }

        private void BuildCrefSchemaMap(
            IList<string> allListedTypes,
            Dictionary<string, SchemaInfo> crefSchemaMap,
            TypeFetcher typeFetcher,
            SchemaReferenceRegistry schemaReferenceRegistry)
        {
            var key = allListedTypes.GetCrefKey();
            var schemaInfo = new SchemaInfo();
            try
            {
                var type = typeFetcher.LoadTypeFromCrefValues(allListedTypes);
                var schema = schemaReferenceRegistry.FindOrAddReference(type);
                schemaInfo.schema = schema.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);             
            }
            catch (Exception e)
            {
                var error = new GenerationError
                {
                    ExceptionType = e.GetType().Name,
                    Message = e.Message
                };

                schemaInfo.error = error;
            }

            if (!crefSchemaMap.ContainsKey(key))
            {
                crefSchemaMap.Add(key, schemaInfo);
            }
        }

        private void BuildCrefFieldValueMap(
            string crefValue,
            Dictionary<string, string> crefFieldMap,
            TypeFetcher typeFetcher)
        {
            if (string.IsNullOrWhiteSpace(crefValue))
            {
                return;
            }

            var typeName = crefValue.ExtractTypeNameFromFieldCref();
            var type = typeFetcher.LoadTypeFromCrefValues(new List<string> { typeName });
            var fieldName = crefValue.ExtractFieldNameFromCref();

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            var field = fields.FirstOrDefault(f => f.Name == fieldName);

            if (field == null)
            {
                var errorMessage = string.Format(
                    SpecificationGenerationMessages.FieldNotFound,
                    fieldName,
                    typeName);

                throw new TypeLoadException(errorMessage);
            }

            crefFieldMap.Add(crefValue, field.GetValue(null).ToString());
        }
    }
}