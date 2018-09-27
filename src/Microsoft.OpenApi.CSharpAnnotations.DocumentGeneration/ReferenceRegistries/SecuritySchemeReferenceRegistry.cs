// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Exceptions;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Extensions;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models.KnownStrings;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.ReferenceRegistries
{
    /// <summary>
    /// Reference Registry for <see cref="OpenApiSecurityScheme"/>.
    /// </summary>
    public class SecuritySchemeReferenceRegistry : ReferenceRegistry<XElement, OpenApiSecurityScheme>
    {
        /// <summary>
        /// The dictionary containing all references of the given security scheme.
        /// </summary>
        public override IDictionary<string, OpenApiSecurityScheme> References { get; } =
            new Dictionary<string, OpenApiSecurityScheme>();

        internal List<string> Scopes { get; private set; }

        /// <summary>
        /// Finds the existing reference object based on the key from the input or creates a new one.
        /// </summary>
        /// <returns>The existing or created reference object.</returns>
        internal override OpenApiSecurityScheme FindOrAddReference(XElement securityElement)
        {
            var type = securityElement.Attribute(KnownXmlStrings.Type)?.Value.Trim();
            var key = GetKey(securityElement);

            Scopes = new List<string>();

            if (References.ContainsKey(key))
            {
                return new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Id = key,
                        Type = ReferenceType.SecurityScheme
                    }
                };
            }

            var description = securityElement.Elements().FirstOrDefault(
                p => p.Name == KnownXmlStrings.Description)?.Value;

            var securityScheme = new OpenApiSecurityScheme
            {
                Description = description,
                Type = GetSecuritySchemeType(type)
            };

            if (securityScheme.Type == SecuritySchemeType.ApiKey)
            {
                var inValue = securityElement.Elements().FirstOrDefault(p => p.Name == KnownXmlStrings.In)?.Value;
                var name = securityElement.Attribute(KnownXmlStrings.Name)?.Value.Trim();

                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new InvalidSecurityTagException(
                        string.Format(SpecificationGenerationMessages.UndocumentedName, KnownXmlStrings.Security));
                }

                securityScheme.In = GetParameterKind(inValue);
                securityScheme.Name = name;
            }

            if (securityScheme.Type == SecuritySchemeType.Http)
            {
                var scheme = securityElement.Elements().FirstOrDefault(p => p.Name == KnownXmlStrings.Scheme)?.Value;
                var bearerFormat = securityElement.Elements()
                    .FirstOrDefault(p => p.Name == KnownXmlStrings.BearerFormat)?.Value;

                if (string.IsNullOrWhiteSpace(scheme))
                {
                    throw new InvalidSecurityTagException(
                        string.Format(SpecificationGenerationMessages.UndocumentedScheme, SecuritySchemeType.Http));
                }

                securityScheme.Scheme = scheme;

                if (!string.IsNullOrWhiteSpace(bearerFormat))
                {
                    securityScheme.BearerFormat = bearerFormat;
                }
            }

            if (securityScheme.Type == SecuritySchemeType.OpenIdConnect)
            {
                var url = securityElement.Elements().FirstOrDefault(p => p.Name == KnownXmlStrings.OpenIdConnectUrl)
                    ?.Value;

                var scopeElements = securityElement.Elements()
                    .Where(p => p.Name == KnownXmlStrings.scope);

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
                            KnownXmlStrings.scope));
                    }

                    Scopes.Add(name);
                }

                if (string.IsNullOrWhiteSpace(url))
                {
                    throw new InvalidSecurityTagException(
                        string.Format(SpecificationGenerationMessages.UndocumentedOpenIdConnectUrl,
                            SecuritySchemeType.OpenIdConnect));
                }
                securityScheme.OpenIdConnectUrl = new Uri(url);
            }

            if (securityScheme.Type == SecuritySchemeType.OAuth2)
            {
                var flowElements = securityElement.Elements().Where(p => p.Name == KnownXmlStrings.flow);

                securityScheme.Flows = new OpenApiOAuthFlows();

                if (!flowElements.Any())
                {
                    throw new InvalidSecurityTagException(
                        string.Format(SpecificationGenerationMessages.UndocumentedFlow, SecuritySchemeType.OAuth2));
                }

                foreach (var flowElement in flowElements)
                {
                    var flowType = flowElement.Attribute(KnownXmlStrings.Type)?.Value;

                    if (string.IsNullOrWhiteSpace(type))
                    {
                        throw new InvalidSecurityTagException(string.Format(
                            SpecificationGenerationMessages.UndocumentedType,
                            KnownXmlStrings.flow));
                    }

                    IList<string> oAuthScopes;

                    if (flowType == KnownXmlStrings.implicitFlow)
                    {
                        securityScheme.Flows.Implicit =
                            GetOAuthFlow(flowElement, KnownXmlStrings.implicitFlow, out oAuthScopes);
                        Scopes.AddRange(oAuthScopes);
                    }

                    if (flowType == KnownXmlStrings.password)
                    {
                        securityScheme.Flows.Password =
                            GetOAuthFlow(flowElement, KnownXmlStrings.password, out oAuthScopes);
                        foreach (var oAuthScope in oAuthScopes)
                        {
                            if (!Scopes.Contains(oAuthScope))
                            {
                                Scopes.Add(oAuthScope);
                            }
                        }
                    }

                    if (flowType == KnownXmlStrings.clientCredentials)
                    {
                        securityScheme.Flows.ClientCredentials =
                            GetOAuthFlow(flowElement, KnownXmlStrings.clientCredentials, out oAuthScopes);
                        foreach (var oAuthScope in oAuthScopes)
                        {
                            if (!Scopes.Contains(oAuthScope))
                            {
                                Scopes.Add(oAuthScope);
                            }
                        }
                    }

                    if (flowType == KnownXmlStrings.authorizationCode)
                    {
                        securityScheme.Flows.AuthorizationCode =
                            GetOAuthFlow(flowElement, KnownXmlStrings.authorizationCode, out oAuthScopes);
                        foreach (var oAuthScope in oAuthScopes)
                        {
                            if (!Scopes.Contains(oAuthScope))
                            {
                                Scopes.Add(oAuthScope);
                            }
                        }
                    }
                }
            }

            References.Add(key, securityScheme);

            return new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = key,
                    Type = ReferenceType.SecurityScheme
                }
            };
        }

        internal override string GetKey(XElement input)
        {
            var name = input.Attribute(KnownXmlStrings.Name)?.Value.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidSecurityTagException(string.Format(
                    SpecificationGenerationMessages.UndocumentedName,
                    KnownXmlStrings.Security));
            }
            return name.SanitizeClassName();
        }

        private static OpenApiOAuthFlow GetOAuthFlow(XElement element, string flowType, out IList<string> scopeNames)
        {
            var oAuthFlow = new OpenApiOAuthFlow();
            scopeNames = new List<string>();

            var authorizationUrl = element.Elements()
                .FirstOrDefault(p => p.Name == KnownXmlStrings.AuthorizationUrl)?.Value;

            var refreshUrl = element.Elements()
                .FirstOrDefault(p => p.Name == KnownXmlStrings.RefreshUrl)?.Value;

            var tokenUrl = element.Elements()
                .FirstOrDefault(p => p.Name == KnownXmlStrings.TokenUrl)?.Value;

            if (flowType == KnownXmlStrings.implicitFlow || flowType == KnownXmlStrings.authorizationCode)
            {
                if (authorizationUrl == null)
                {
                    throw new InvalidSecurityTagException(string.Format(
                        SpecificationGenerationMessages.UndocumentedAuthorizationUrl,
                        flowType));
                }
                oAuthFlow.AuthorizationUrl = new Uri(authorizationUrl);
            }

            if (flowType == KnownXmlStrings.password || flowType == KnownXmlStrings.authorizationCode ||
                flowType == KnownXmlStrings.clientCredentials)
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
                .Where(p => p.Name == KnownXmlStrings.scope);

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
                        KnownXmlStrings.scope));
                }

                var description = scopeElement.Elements().FirstOrDefault(p => p.Name == KnownXmlStrings.Description)
                    ?.Value;

                if (string.IsNullOrWhiteSpace(description))
                {
                    throw new InvalidSecurityTagException(string.Format(
                        SpecificationGenerationMessages.UndocumentedDescription,
                        KnownXmlStrings.scope));
                }

                scopeNames.Add(name);
                oAuthFlow.Scopes.Add(name, description);
            }

            return oAuthFlow;
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

        private static SecuritySchemeType GetSecuritySchemeType(string type)
        {
            switch (type)
            {
                case KnownXmlStrings.OpenIdConnect:
                    return SecuritySchemeType.OpenIdConnect;

                case KnownXmlStrings.ApiKey:
                    return SecuritySchemeType.ApiKey;

                case KnownXmlStrings.Http:
                    return SecuritySchemeType.Http;

                case KnownXmlStrings.OAuth2:
                    return SecuritySchemeType.OAuth2;

                default:
                    throw new InvalidSecurityTagException(string.Format(
                        SpecificationGenerationMessages.NotSupportedTypeAttributeValue,
                        type,
                        KnownXmlStrings.Security,
                        string.Join(", ", KnownXmlStrings.AllowedSecurityTypeValues)));
            }
        }
    }
}