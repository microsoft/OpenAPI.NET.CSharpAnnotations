// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using FluentAssertions;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Exceptions;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Extensions;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models.KnownStrings;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.ReferenceRegistries;
using Microsoft.OpenApi.Models;
using Xunit;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.ReferenceRegistryTests
{
    [Collection("DefaultSettings")]
    public class SecuritySchemeReferenceRegistryTest
    {
        private const string InputDirectory = "ReferenceRegistryTests/Input";

        [Theory]
        [MemberData(nameof(GetTestCasesForGenerateSecuritySchemeShouldSucceed))]
        public void GenerateSecuritySchemeShouldSucceed(
            XElement xElement,
            OpenApiSecurityScheme expectedSecurityScheme,
            IDictionary<string, OpenApiSecurityScheme> expectedReferences,
            IList<string> expectedScopes)
        {
            // Arrange
            var securitySchemeReferenceRegistry = new SecuritySchemeReferenceRegistry();

            // Act
            var returnedSecurityScheme =
                securitySchemeReferenceRegistry.FindOrAddReference(xElement);

            // Assert
            var actualReferences = securitySchemeReferenceRegistry.References;
            var actualScopes = securitySchemeReferenceRegistry.Scopes;

            returnedSecurityScheme.Should().BeEquivalentTo(expectedSecurityScheme);
            actualReferences.Should().BeEquivalentTo(expectedReferences);
            actualScopes.Should().BeEquivalentTo(expectedScopes);
        }

        [Theory]
        [MemberData(nameof(GetTestCasesForGenerateSecuritySchemeShouldThrowException))]
        public void GenerateSecuritySchemeShouldThrowException(
            XElement xElement,
            string expectedExceptionMessage)
        {
            // Arrange
            var securitySchemeReferenceRegistry = new SecuritySchemeReferenceRegistry();

            try
            {
                securitySchemeReferenceRegistry.FindOrAddReference(xElement);
            }
            catch (InvalidSecurityTagException e)
            {
                e.Message.Should().BeEquivalentTo(expectedExceptionMessage);
            }
        }

        public static IEnumerable<object[]> GetTestCasesForGenerateSecuritySchemeShouldSucceed()
        {
            yield return new object[]
            {
                XDocument.Load(Path.Combine(InputDirectory, "ValidApiKeySecurityTag.xml")).Root,
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "api-Key".SanitizeClassName()
                    }
                },
                new Dictionary<string, OpenApiSecurityScheme>
                {
                    ["api-Key".SanitizeClassName()]
                    = new OpenApiSecurityScheme
                    {
                        Description = "Test security",
                        Name = "api-Key",
                        In = ParameterLocation.Query,
                        Type = SecuritySchemeType.ApiKey
                    }
                },
                new List<string>()
            };

            yield return new object[]
            {
                XDocument.Load(Path.Combine(InputDirectory, "ValidHttpSecurityTag.xml")).Root,
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "http".SanitizeClassName()
                    }
                },
                new Dictionary<string, OpenApiSecurityScheme>
                {
                    ["http".SanitizeClassName()]
                    = new OpenApiSecurityScheme
                    {
                        Description = "Test security",
                        Scheme = "basic",
                        Type = SecuritySchemeType.Http
                    }
                },
                new List<string>()
            };

            yield return new object[]
            {
                XDocument.Load(Path.Combine(InputDirectory, "ValidOAuth2SecurityTag.xml")).Root,
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "oauth".SanitizeClassName()
                    }
                },
                new Dictionary<string, OpenApiSecurityScheme>
                {
                    ["oauth".SanitizeClassName()]
                    = new OpenApiSecurityScheme
                    {
                        Description = "Test security",
                        Flows = new OpenApiOAuthFlows
                        {
                            AuthorizationCode = new OpenApiOAuthFlow
                            {
                                AuthorizationUrl = new Uri("https://example.com/api/oauth/dialog"),
                                TokenUrl = new Uri("https://example.com/api/oauth/dialog"),
                                RefreshUrl = new Uri("https://example.com/api/oauth/dialog"),
                                Scopes = new Dictionary<string, string>
                                {
                                    {"scope1", "scope1"}
                                }
                            },
                            Implicit = new OpenApiOAuthFlow
                            {
                                AuthorizationUrl = new Uri("https://example.com/api/oauth/dialog"),
                                RefreshUrl = new Uri("https://example.com/api/oauth/dialog"),
                                Scopes = new Dictionary<string, string>
                                {
                                    {"scope1", "scope1"}
                                }
                            },
                            Password = new OpenApiOAuthFlow
                            {
                                TokenUrl = new Uri("https://example.com/api/oauth/dialog"),
                                RefreshUrl = new Uri("https://example.com/api/oauth/dialog"),
                                Scopes = new Dictionary<string, string>
                                {
                                    {"scope1", "scope1"}
                                }
                            },
                            ClientCredentials = new OpenApiOAuthFlow
                            {
                                TokenUrl = new Uri("https://example.com/api/oauth/dialog"),
                                RefreshUrl = new Uri("https://example.com/api/oauth/dialog"),
                                Scopes = new Dictionary<string, string>
                                {
                                    {"scope1", "scope1"}
                                }
                            }
                        },
                        Type = SecuritySchemeType.OAuth2
                    }
                },
                new List<string> {"scope1"}
            };

            yield return new object[]
            {
                XDocument.Load(Path.Combine(InputDirectory, "ValidOpenIdConnectSecurityTag.xml")).Root,
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "openidconnect".SanitizeClassName()
                    }
                },
                new Dictionary<string, OpenApiSecurityScheme>
                {
                    ["openidconnect".SanitizeClassName()]
                    = new OpenApiSecurityScheme
                    {
                        Description = "Test security",
                        OpenIdConnectUrl = new Uri("https://example.com/api/oauth/dialog"),
                        Type = SecuritySchemeType.OpenIdConnect
                    }
                },
                new List<string> {"scope1"}
            };
        }

        public static IEnumerable<object[]> GetTestCasesForGenerateSecuritySchemeShouldThrowException()
        {
            // Unsupported security type
            yield return new object[]
            {
                XDocument.Parse(
                        "<security type=\"notSupported\" name=\"api-Key\"><description>Test security</description><in>query</in></security>")
                    .Root,
                string.Format(
                    SpecificationGenerationMessages.NotSupportedTypeAttributeValue,
                    "notSupported",
                    KnownXmlStrings.Security,
                    string.Join(", ", KnownXmlStrings.AllowedSecurityTypeValues))
            };

            // Undocumented name attribute in security tag
            yield return new object[]
            {
                XDocument.Parse(
                        "<security type=\"notSupported\"><description>Test security</description><in>query</in></security>")
                    .Root,
                string.Format(
                    SpecificationGenerationMessages.UndocumentedName,
                    KnownXmlStrings.Security)
            };

            // Undocumented scheme for http security type
            yield return new object[]
            {
                XDocument.Parse(
                    "<security type=\"http\" name=\"http\"><description>Test security</description></security>").Root,
                string.Format(SpecificationGenerationMessages.UndocumentedScheme, SecuritySchemeType.Http)
            };

            // Undocumented flow for oAuth2 security type
            yield return new object[]
            {
                XDocument.Parse(
                        "<security type=\"oauth2\" name=\"oauth\"><description>Test security</description></security>")
                    .Root,
                string.Format(
                    SpecificationGenerationMessages.UndocumentedFlow,
                    KnownXmlStrings.OAuth2)
            };

            // Undocumented authorization url for implicit flow type
            yield return new object[]
            {
                XDocument.Parse(
                        "<security type=\"oauth2\" name=\"oauth\"><description>Test security</description><flow type=\"implicit\"><scope name=\"scope1\"><description>scope1</description></scope></flow></security>")
                    .Root,
                string.Format(
                    SpecificationGenerationMessages.UndocumentedAuthorizationUrl,
                    KnownXmlStrings.ImplicitFlow)
            };

            // Undocumented token url for password flow type
            yield return new object[]
            {
                XDocument.Parse(
                        "<security type=\"oauth2\" name=\"oauth\"><description>Test security</description><flow type=\"password\"><scope name=\"scope1\"><description>scope1</description></scope></flow></security>")
                    .Root,
                string.Format(
                    SpecificationGenerationMessages.UndocumentedTokenUrl,
                    KnownXmlStrings.Password)
            };

            // Undocumented open id connect url for security type openIdConnect
            yield return new object[]
            {
                XDocument.Parse(
                        "<security type=\"openIdConnect\" name=\"openidconnect\"><scope name=\"scope1\"></scope>\r\n</security>")
                    .Root,
                string.Format(
                    SpecificationGenerationMessages.UndocumentedOpenIdConnectUrl,
                    KnownXmlStrings.OpenIdConnect)
            };


            // Undocumented scope for security type openIdConnect
            yield return new object[]
            {
                XDocument.Parse(
                        "<security type=\"openIdConnect\" name=\"openidconnect\"><openIdConnectUrl>https://example.com/api/oauth/dialog</openIdConnectUrl></security>")
                    .Root,
                string.Format(
                    SpecificationGenerationMessages.UndocumentedScopeForSecurity,
                    KnownXmlStrings.OpenIdConnect)
            };

            // Undocumented scope for flow type clientCredentials
            yield return new object[]
            {
                XDocument.Parse(
                        "<security type=\"oauth2\" name=\"oauth\"><flow type=\"clientCredentials\"><tokenUrl>https://example.com/api/oauth/dialog</tokenUrl></flow></security>")
                    .Root,
                string.Format(
                    SpecificationGenerationMessages.UndocumentedScopeForFlow,
                    KnownXmlStrings.ClientCredentials)
            };

            // Undocumented name for scope
            yield return new object[]
            {
                XDocument.Parse(
                        "<security type=\"oauth2\" name=\"oauth\"><flow type=\"clientCredentials\"><tokenUrl>https://example.com/api/oauth/dialog</tokenUrl><scope><description>scope1</description></scope></flow></security>")
                    .Root,
                string.Format(
                    SpecificationGenerationMessages.UndocumentedName,
                    KnownXmlStrings.Scope)
            };

            // Undocumented description for scope
            yield return new object[]
            {
                XDocument.Parse(
                        "<security type=\"oauth2\" name=\"oauth\"><flow type=\"clientCredentials\"><tokenUrl>https://example.com/api/oauth/dialog</tokenUrl><scope name=\"scope1\"></scope></flow></security>")
                    .Root,
                string.Format(
                    SpecificationGenerationMessages.UndocumentedDescription,
                    KnownXmlStrings.Scope)
            };
        }
    }
}