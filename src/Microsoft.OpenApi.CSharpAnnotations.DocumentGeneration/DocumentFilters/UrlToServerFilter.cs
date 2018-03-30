// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.DocumentFilters
{
    /// <summary>
    /// Parses the value of url tag in xml documentation and apply that as server in Open Api V3 specification document.
    /// </summary>
    public class UrlToServerFilter : IDocumentFilter
    {
        /// <summary>
        /// Fetches the value of "url" tag from xml documentation and use it to populate
        /// Server object of Open Api V3 specification document.
        /// </summary>
        /// <param name="specificationDocument">The Open Api V3 specification document to be updated.</param>
        /// <param name="xmlDocuments">The documents representing the annotation xmls.</param>
        /// <param name="settings">Settings for document filters.</param>
        public void Apply(
            OpenApiDocument specificationDocument,
            IList<XDocument> xmlDocuments,
            DocumentFilterSettings settings)
        {
            var basePaths = new List<string>();
            var urlElements = new List<XElement>();

            foreach (var xmlDocument in xmlDocuments)
            {
                urlElements.AddRange(xmlDocument.XPathSelectElements("//doc/members/member/url"));
            }

            foreach (var urlElement in urlElements)
            {
                var url = urlElement.Value;
                var sanitizedUrl = SanitizeUrl(url);

                if (sanitizedUrl != null)
                {
                    basePaths.Add(sanitizedUrl.GetLeftPart(UriPartial.Authority));
                }
            }

            foreach (var basePath in basePaths.Distinct())
            {
                specificationDocument.Servers.Add(new OpenApiServer { Url = basePath});
            }
        }

        private static Uri SanitizeUrl(string url)
        {
            try
            {
                url = Regex.Replace(url, @"\s+", string.Empty);
                return new Uri(url);
            }
            catch (UriFormatException)
            {
                // A uri can be malformed in many ways by the user when they are documenting e.g.
                // https://{host}/path and we do not want to work around those issue and try to find the absolute path.
                // So ignore such uri paths and return null.
                return null;
            }
        }
    }
}