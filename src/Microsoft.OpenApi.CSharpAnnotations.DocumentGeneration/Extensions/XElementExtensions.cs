// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Exceptions;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models.KnownStrings;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="XElement"/>.
    /// </summary>
    public static class XElementExtensions
    {
        /// <summary>
        /// Get the text inside the element with the following modifications:
        /// 1. The see/seealso cref value and the paramref/typeparamref name value extracted out of the inner XML elements.
        /// 2. The para tag is ignored.
        /// 3. Any blank lines are removed.
        /// 4. Beginning and trailing whitespaces are trimmed.
        /// </summary>
        public static string GetDescriptionText(this XElement element)
        {
            var description = new StringBuilder();

            var children = element.Nodes();

            foreach (var child in children)
            {
                switch (child.NodeType)
                {
                    case XmlNodeType.Text:
                        description.Append(child);
                        break;

                    case XmlNodeType.CDATA:
                        description.Append(child.ToString()
                            .Replace("<![CDATA[", string.Empty).Replace("]]>", string.Empty));
                        break;

                    case XmlNodeType.Element:
                        var childElement = (XElement) child;

                        switch (childElement.Name.ToString())
                        {
                            case KnownXmlStrings.Para:
                                description.Append(GetDescriptionText(childElement));
                                break;

                            case KnownXmlStrings.See:
                                description.Append(childElement.Attribute(KnownXmlStrings.Cref)?.Value);
                                break;

                            case KnownXmlStrings.Seealso:
                                description.Append(childElement.Attribute(KnownXmlStrings.Cref)?.Value);
                                break;

                            case KnownXmlStrings.Paramref:
                                description.Append(childElement.Attribute(KnownXmlStrings.Name)?.Value);
                                break;

                            case KnownXmlStrings.Typeparamref:
                                description.Append(childElement.Attribute(KnownXmlStrings.Name)?.Value);
                                break;
                        }
                        break;
                }
            }

            return description.ToString().Trim().RemoveBlankLines();
        }

        /// <summary>
        /// Gets the text from the last text node of the provided element's child nodes with the following modifications:
        /// 1. Any blank lines are removed.
        /// 2. Beginning and trailing whitespaces are trimmed.
        /// </summary>
        public static string GetDescriptionTextFromLastTextNode(this XElement element)
        {
            var lastTextNode = element
                .Nodes()
                .LastOrDefault(i => i.NodeType == XmlNodeType.Text);

            if (lastTextNode != null)
            {
                return lastTextNode.ToString().Trim().RemoveBlankLines();
            }

            return string.Empty;
        }

        /// <summary>
        /// Fetches list of fully qualified type names from the "cref" attribute or "see" tag of the provided XElement.
        /// </summary>
        /// <param name="xElement">The XElement to process.</param>
        /// <returns><see cref="Type"/></returns>
        internal static IList<string> GetListedTypes(
            this XElement xElement)
        {
            var cref = xElement.Attribute(KnownXmlStrings.Cref)?.Value.Trim();
            var seeNodes = xElement.Elements().Where(i => i.Name == KnownXmlStrings.See);

            var allListedTypes = seeNodes
                .Select(node => node.Attribute(KnownXmlStrings.Cref)?.Value)
                .Where(crefValue => crefValue != null).ToList();

            // If no see tags are present, add the value from cref tag.
            if (!allListedTypes.Any() && !string.IsNullOrWhiteSpace(cref))
            {
                allListedTypes.Add(cref);
            }

            return allListedTypes;
        }

        private static ParameterLocation GetParameterKind(string parameterKind)
        {
            switch (parameterKind)
            {
                case KnownXmlStrings.Header:
                    return ParameterLocation.Header;

                case KnownXmlStrings.Query:
                    return ParameterLocation.Query;

                case KnownXmlStrings.Cookie:
                    return ParameterLocation.Cookie;

                default:
                    return ParameterLocation.Header;
            }
        }

        /// <summary>
        /// Processed the "security" tag child elements of the provided XElement
        /// and generate <see cref="OpenApiSecurityScheme"/> of type ApiKey
        /// </summary>
        /// <param name="xElement">The XElement to process.</param>
        /// <returns><see cref="OpenApiSecurityScheme"/></returns>
        internal static OpenApiSecurityScheme ToApiKeySecurityScheme(this XElement xElement)
        {
            var inValue = xElement.Elements().FirstOrDefault(p => p.Name == KnownXmlStrings.In)?.Value;
            var name = xElement.Attribute(KnownXmlStrings.Name)?.Value.Trim();
            var description = xElement.Elements().FirstOrDefault(
                p => p.Name == KnownXmlStrings.Description)?.Value.Trim().RemoveBlankLines(); ;

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidSecurityTagException(
                    string.Format(SpecificationGenerationMessages.UndocumentedName, KnownXmlStrings.Security));
            }

            return new OpenApiSecurityScheme
            {
                Description = description,
                Type = SecuritySchemeType.ApiKey,
                In = GetParameterKind(inValue),
                Name = name
            };
        }

        /// <summary>
        /// Processed the "security" tag child elements of the provided XElement
        /// and generate <see cref="OpenApiSecurityScheme"/> of type Http
        /// </summary>
        /// <param name="xElement">The XElement to process.</param>
        /// <returns><see cref="OpenApiSecurityScheme"/></returns>
        internal static OpenApiSecurityScheme ToHttpSecurityScheme(this XElement xElement)
        {
            var scheme = xElement.Elements().FirstOrDefault(p => p.Name == KnownXmlStrings.Scheme)?.Value;
            var bearerFormat = xElement.Elements()
                .FirstOrDefault(p => p.Name == KnownXmlStrings.BearerFormat)?.Value;
            var description = xElement.Elements().FirstOrDefault(
                p => p.Name == KnownXmlStrings.Description)?.Value.Trim().RemoveBlankLines(); ;

            if (string.IsNullOrWhiteSpace(scheme))
            {
                throw new InvalidSecurityTagException(
                    string.Format(SpecificationGenerationMessages.UndocumentedScheme, SecuritySchemeType.Http));
            }

            return new OpenApiSecurityScheme
            {
                Description = description,
                Type = SecuritySchemeType.Http,
                Scheme = scheme,
                BearerFormat = bearerFormat
            };
        }

        /// <summary>
        /// Processed the "security" tag child elements of the provided XElement
        /// and generate <see cref="OpenApiSecurityScheme"/> of type OAuth2
        /// </summary>
        /// <param name="xElement">The XElement to process.</param>
        /// <param name="scopes">The scopes.</param>
        /// <returns><see cref="OpenApiSecurityScheme"/></returns>
        internal static OpenApiSecurityScheme ToOAuth2SecurityScheme(this XElement xElement, out IList<string> scopes)
        {
            var flowElements = xElement.Elements().Where(p => p.Name == KnownXmlStrings.Flow);
            var description = xElement.Elements()
                .FirstOrDefault(p => p.Name == KnownXmlStrings.Description)?
                .Value.Trim().RemoveBlankLines(); ;
            scopes = new List<string>();

            var securityScheme = new OpenApiSecurityScheme
            {
                Flows = new OpenApiOAuthFlows(),
                Description = description,
                Type = SecuritySchemeType.OAuth2
            };

            if (!flowElements.Any())
            {
                throw new InvalidSecurityTagException(
                    string.Format(SpecificationGenerationMessages.UndocumentedFlow, SecuritySchemeType.OAuth2));
            }

            foreach (var flowElement in flowElements)
            {
                var flowType = flowElement.Attribute(KnownXmlStrings.Type)?.Value;

                if (string.IsNullOrWhiteSpace(flowType))
                {
                    throw new InvalidSecurityTagException(string.Format(
                        SpecificationGenerationMessages.UndocumentedType,
                        KnownXmlStrings.Flow));
                }

                IList<string> oAuthScopes;

                switch (flowType)
                {
                    case KnownXmlStrings.ImplicitFlow:

                        securityScheme.Flows.Implicit =
                            flowElement.ToOAuthFlow(flowType, out oAuthScopes);
                        foreach (var oAuthScope in oAuthScopes)
                        {
                            if (!scopes.Contains(oAuthScope))
                            {
                                scopes.Add(oAuthScope);
                            }
                        }
                        break;

                    case KnownXmlStrings.Password:

                        securityScheme.Flows.Password =
                            flowElement.ToOAuthFlow(flowType, out oAuthScopes);
                        foreach (var oAuthScope in oAuthScopes)
                        {
                            if (!scopes.Contains(oAuthScope))
                            {
                                scopes.Add(oAuthScope);
                            }
                        }
                        break;

                    case KnownXmlStrings.ClientCredentials:

                        securityScheme.Flows.ClientCredentials =
                            flowElement.ToOAuthFlow(flowType, out oAuthScopes);
                        foreach (var oAuthScope in oAuthScopes)
                        {
                            if (!scopes.Contains(oAuthScope))
                            {
                                scopes.Add(oAuthScope);
                            }
                        }
                        break;

                    case KnownXmlStrings.AuthorizationCode:
                        securityScheme.Flows.AuthorizationCode =
                            flowElement.ToOAuthFlow(flowType, out oAuthScopes);
                        foreach (var oAuthScope in oAuthScopes)
                        {
                            if (!scopes.Contains(oAuthScope))
                            {
                                scopes.Add(oAuthScope);
                            }
                        }

                        break;

                    default:
                        throw new InvalidSecurityTagException(string.Format(
                            SpecificationGenerationMessages.NotSupportedTypeAttributeValue,
                            flowType,
                            KnownXmlStrings.Flow,
                            string.Join(", ", KnownXmlStrings.AllowedFlowTypeValues)));
                }
            }

            return securityScheme;
        }

        private static OpenApiOAuthFlow ToOAuthFlow(this XElement element, string flowType,
            out IList<string> scopeNames)
        {
            var oAuthFlow = new OpenApiOAuthFlow();
            scopeNames = new List<string>();

            var authorizationUrl = element.Elements()
                .FirstOrDefault(p => p.Name == KnownXmlStrings.AuthorizationUrl)?.Value;

            var refreshUrl = element.Elements()
                .FirstOrDefault(p => p.Name == KnownXmlStrings.RefreshUrl)?.Value;

            var tokenUrl = element.Elements()
                .FirstOrDefault(p => p.Name == KnownXmlStrings.TokenUrl)?.Value;

            if (flowType == KnownXmlStrings.ImplicitFlow || flowType == KnownXmlStrings.AuthorizationCode)
            {
                if (authorizationUrl == null)
                {
                    throw new InvalidSecurityTagException(string.Format(
                        SpecificationGenerationMessages.UndocumentedAuthorizationUrl,
                        flowType));
                }
                oAuthFlow.AuthorizationUrl = new Uri(authorizationUrl);
            }

            if (flowType == KnownXmlStrings.Password
                || flowType == KnownXmlStrings.AuthorizationCode
                || flowType == KnownXmlStrings.ClientCredentials)
            {
                if (tokenUrl == null)
                {
                    throw new InvalidSecurityTagException(string.Format(
                        SpecificationGenerationMessages.UndocumentedTokenUrl,
                        flowType));
                }

                oAuthFlow.TokenUrl = new Uri(tokenUrl);
            }

            if (refreshUrl != null)
            {
                oAuthFlow.RefreshUrl = new Uri(refreshUrl);
            }

            var scopeElements = element.Elements()
                .Where(p => p.Name == KnownXmlStrings.Scope);

            if (!scopeElements.Any())
            {
                throw new InvalidSecurityTagException(string.Format(
                    SpecificationGenerationMessages.UndocumentedScopeForFlow,
                    flowType));
            }

            foreach (var scopeElement in scopeElements)
            {
                var name = scopeElement.Attribute(KnownXmlStrings.Name)?.Value;

                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new InvalidSecurityTagException(string.Format(
                        SpecificationGenerationMessages.UndocumentedName,
                        KnownXmlStrings.Scope));
                }

                var description = scopeElement.Elements().FirstOrDefault(p => p.Name == KnownXmlStrings.Description)
                    ?.Value;

                if (string.IsNullOrWhiteSpace(description))
                {
                    throw new InvalidSecurityTagException(string.Format(
                        SpecificationGenerationMessages.UndocumentedDescription,
                        KnownXmlStrings.Scope));
                }

                scopeNames.Add(name);
                oAuthFlow.Scopes.Add(name, description);
            }

            return oAuthFlow;
        }

        private static OpenApiExample ToOpenApiExample(
            this XElement element,
            Dictionary<string, FieldValueInfo> crefFieldValueMap,
            List<GenerationError> generationErrors)
        {
            var exampleChildElements = element.Elements();

            if (!exampleChildElements.Any())
            {
                return null;
            }

            var summaryElement = exampleChildElements.FirstOrDefault(p => p.Name == KnownXmlStrings.Summary);

            var openApiExample = new OpenApiExample();

            if (summaryElement != null)
            {
                openApiExample.Summary = summaryElement.Value;
            }

            var valueElement = exampleChildElements.FirstOrDefault(p => p.Name == KnownXmlStrings.Value);
            var urlElement = exampleChildElements.FirstOrDefault(p => p.Name == KnownXmlStrings.Url);

            if (valueElement != null && urlElement != null)
            {
                generationErrors.Add(
                    new GenerationError
                    {
                        ExceptionType = nameof(InvalidExampleException),
                        Message = SpecificationGenerationMessages.ProvideEitherValueOrUrlTag
                    });

                return null;
            }

            IOpenApiAny exampleValue = null;

            if (valueElement != null)
            {
                var seeNodes = element.Descendants(KnownXmlStrings.See);
                var crefValue = seeNodes
                    .Select(node => node.Attribute(KnownXmlStrings.Cref)?.Value)
                    .FirstOrDefault(crefVal => crefVal != null);

                if (string.IsNullOrWhiteSpace(valueElement.Value) && string.IsNullOrWhiteSpace(crefValue))
                {
                    generationErrors.Add(
                    new GenerationError
                    {
                        ExceptionType = nameof(InvalidExampleException),
                        Message = SpecificationGenerationMessages.ProvideValueForExample
                    });

                    return null;
                }

                if (!string.IsNullOrWhiteSpace(valueElement.Value))
                {
                    exampleValue = new OpenApiStringReader()
                        .ReadFragment<IOpenApiAny>(
                            valueElement.Value,
                            OpenApiSpecVersion.OpenApi3_0,
                            out OpenApiDiagnostic _);
                }

                if (!string.IsNullOrWhiteSpace(crefValue) && crefFieldValueMap.ContainsKey(crefValue))
                {
                    var fieldValueInfo = crefFieldValueMap[crefValue];

                    if (fieldValueInfo.Error != null)
                    {
                        generationErrors.Add(fieldValueInfo.Error);

                        return null;
                    }

                    exampleValue = new OpenApiStringReader().ReadFragment<IOpenApiAny>(
                        fieldValueInfo.Value,
                        OpenApiSpecVersion.OpenApi3_0,
                        out OpenApiDiagnostic _);
                }

                openApiExample.Value = exampleValue;
            }

            if (urlElement != null)
            {
                openApiExample.ExternalValue = urlElement.Value;
            }

            return openApiExample;
        }

        /// <summary>
        /// Processes the "example" tag child elements of the provide XElement
        /// and generates a map of string to OpenApiExample.
        /// </summary>
        /// <param name="xElement">The XElement to process.</param>
        /// <param name="crefFieldValueMap">The cref to field value map.</param>
        /// <param name="generationErrors">The generation errors produced while processing header.</param>
        /// <returns>The map of string to OpenApiExample.</returns>
        internal static Dictionary<string, OpenApiExample> ToOpenApiExamples(
            this XElement xElement,
            Dictionary<string, FieldValueInfo> crefFieldValueMap,
            List<GenerationError> generationErrors)
        {
            var exampleElements = xElement.Elements().Where(p => p.Name == KnownXmlStrings.Example);
            var examples = new Dictionary<string, OpenApiExample>();
            var exampleCounter = 1;

            foreach (var exampleElement in exampleElements)
            {
                var exampleName = exampleElement.Attribute(KnownXmlStrings.Name)?.Value.Trim();
                var example = exampleElement.ToOpenApiExample(crefFieldValueMap, generationErrors);

                if (example != null)
                {
                    examples.Add(
                        string.IsNullOrWhiteSpace(exampleName) ? $"example{exampleCounter++}" : exampleName,
                        example);
                }
            }

            return examples;
        }

        /// <summary>
        /// Processes the "header" tag child elements of the provide XElement
        /// and generates a map of string to OpenApiHeader.
        /// </summary>
        /// <param name="xElement">The XElement to process.</param>
        /// <param name="crefSchemaMap">The cref to schema map.</param>
        /// <param name="generationErrors">The generation errors produced while processing header.</param>
        /// <returns>The map of string to OpenApiHeader.</returns>
        internal static Dictionary<string, OpenApiHeader> ToOpenApiHeaders(
            this XElement xElement,
            Dictionary<string, SchemaGenerationInfo> crefSchemaMap,
            IList<GenerationError> generationErrors)
        {
            var headerElements = xElement.Elements()
                .Where(
                    p => p.Name == KnownXmlStrings.Header)
                .ToList();

            var openApiHeaders = new Dictionary<string, OpenApiHeader>();

            foreach (var headerElement in headerElements)
            {
                var name = headerElement.Attribute(KnownXmlStrings.Name)?.Value.Trim();
                if (string.IsNullOrWhiteSpace(name))
                {
                    generationErrors.Add(new GenerationError
                    {
                        ExceptionType = nameof(InvalidHeaderException),
                        Message = string.Format(SpecificationGenerationMessages.UndocumentedName, "header")
                    });

                    return null;
                }

                var description = headerElement
                    .Elements()
                    .FirstOrDefault(p => p.Name == KnownXmlStrings.Description)?.Value.Trim().RemoveBlankLines();

                var listedTypes = headerElement.GetListedTypes();
                var crefKey = listedTypes.ToCrefKey();

                var schema = new OpenApiSchema
                {
                    Type = "string"
                };

                if (crefSchemaMap.ContainsKey(crefKey))
                {
                    var schemaInfo = crefSchemaMap[crefKey];

                    if (schemaInfo.Error != null)
                    {
                        generationErrors.Add(schemaInfo.Error);

                        return null;
                    }

                    schema = schemaInfo.Schema;
                }

                openApiHeaders.Add(
                    name,
                    new OpenApiHeader
                    {
                        Description = description,
                        Schema = schema
                    });
            }

            return openApiHeaders;
        }

        /// <summary>
        /// Processed the "security" tag child elements of the provided XElement
        /// and generate <see cref="OpenApiSecurityScheme"/> of type OpenIdConnect
        /// </summary>
        /// <param name="xElement">The XElement to process.</param>
        /// <param name="scopes">The scopes.</param>
        /// <returns><see cref="OpenApiSecurityScheme"/></returns>
        internal static OpenApiSecurityScheme ToOpenIdConnectSecurityScheme(this XElement xElement,
            out IList<string> scopes)
        {
            var url = xElement.Elements().FirstOrDefault(p => p.Name == KnownXmlStrings.OpenIdConnectUrl)?.Value;
            var scopeElements = xElement.Elements().Where(p => p.Name == KnownXmlStrings.Scope);
            var description = xElement.Elements()
                .FirstOrDefault(p => p.Name == KnownXmlStrings.Description)?
                .Value.Trim().RemoveBlankLines(); ;
            scopes = new List<string>();

            if (!scopeElements.Any())
            {
                throw new InvalidSecurityTagException(string.Format(
                    SpecificationGenerationMessages.UndocumentedScopeForSecurity,
                    KnownXmlStrings.OpenIdConnect));
            }

            foreach (var scopeElement in scopeElements)
            {
                var name = scopeElement.Attribute(KnownXmlStrings.Name)?.Value;

                if (name == null)
                {
                    throw new InvalidSecurityTagException(string.Format(
                        SpecificationGenerationMessages.UndocumentedName,
                        KnownXmlStrings.Scope));
                }

                scopes.Add(name);
            }

            if (string.IsNullOrWhiteSpace(url))
            {
                throw new InvalidSecurityTagException(
                    string.Format(SpecificationGenerationMessages.UndocumentedOpenIdConnectUrl,
                        SecuritySchemeType.OpenIdConnect));
            }

            return new OpenApiSecurityScheme
            {
                Description = description,
                Type = SecuritySchemeType.OpenIdConnect,
                OpenIdConnectUrl = new Uri(url)
            };
        }
    }
}