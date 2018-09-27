// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
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

            var securityScheme = new OpenApiSecurityScheme();

            switch (type)
            {
                case KnownXmlStrings.ApiKey:
                    securityScheme = securityElement.ToApiKeySecurityScheme();
                    break;

                case KnownXmlStrings.Http:
                    securityScheme = securityElement.ToHttpSecurityScheme();
                    break;

                case KnownXmlStrings.OAuth2:
                    IList<string> oAuthScopes;
                    securityScheme = securityElement.ToOAuth2SecurityScheme(out oAuthScopes);
                    Scopes.AddRange(oAuthScopes);
                    break;

                case KnownXmlStrings.OpenIdConnect:
                    IList<string> scopes;
                    securityScheme = securityElement.ToOpenIdConnectSecurityScheme(out scopes);
                    Scopes.AddRange(scopes);
                    break;

                default:
                    throw new InvalidSecurityTagException(string.Format(
                        SpecificationGenerationMessages.NotSupportedTypeAttributeValue,
                        type,
                        KnownXmlStrings.Security,
                        string.Join(", ", KnownXmlStrings.AllowedSecurityTypeValues)));
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
    }
}