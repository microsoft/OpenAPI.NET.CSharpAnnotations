// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models.KnownStrings;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.OperationFilters
{
    /// <summary>
    /// Parses the value of the security tag in xml documentation and apply that as security requirement in operation.
    /// </summary>
    public class SecurityToSecurityRequirementOperationFilter : IOperationFilter
    {
        /// <summary>
        /// Fetches the value of "security" tags from xml documentation and populates operation's security requirement
        /// values.
        /// </summary>
        /// <param name="operation">The operation to be updated.</param>
        /// <param name="element">The xml element representing an operation in the annotation xml.</param>
        /// <param name="settings">The operation filter settings.</param>
        public void Apply(OpenApiOperation operation, XElement element, OperationFilterSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException( nameof( settings ) );
            }

            if (element == null)
            {
                throw new ArgumentNullException( nameof( element ) );
            }

            var securityElements = element.Elements()
                .Where(
                    p => p.Name == KnownXmlStrings.Security)
                .ToList();

            if (!securityElements.Any())
            {
                return;
            }

            var openApiSecurityRequirement = new OpenApiSecurityRequirement();
            var securitySchemeRegistry = settings.ReferenceRegistryManager.SecuritySchemeReferenceRegistry;

            foreach (var securityElement in securityElements)
            {
                var securityScheme = securitySchemeRegistry.FindOrAddReference( securityElement );

                openApiSecurityRequirement.Add( securityScheme, securitySchemeRegistry.Scopes );
            }

            operation.Security.Add( openApiSecurityRequirement );
        }
    }
}