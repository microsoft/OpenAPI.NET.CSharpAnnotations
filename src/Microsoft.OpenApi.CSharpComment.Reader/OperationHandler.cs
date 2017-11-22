// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using Microsoft.OpenApi.CSharpComment.Reader.Exceptions;
using Microsoft.OpenApi.CSharpComment.Reader.Extensions;
using Microsoft.OpenApi.CSharpComment.Reader.Models.KnownStrings;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpComment.Reader
{
    internal static class OperationHandler
    {
        /// <summary>
        /// Gets the operation id by parsing segments out of the absolute path.
        /// </summary>
        public static string GetOperationId(string absolutePath, OperationType operationMethod)
        {
            var operationId = new StringBuilder(operationMethod.ToString().ToLowerInvariant());

            foreach (var segment in absolutePath.Split('/'))
            {
                if (string.IsNullOrWhiteSpace(segment))
                {
                    continue;
                }

                var current = string.Empty;

                // In order to build an operation id, extract the path parameters
                // and prepend By to these before adding them to the operationId.
                // e.g. for GET /v6/products/{productId} -> getV6ProductsByProductId 
                var matches = new Regex(@"\{(.*?)\}").Matches(segment);

                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        current += "By" + match.Groups[1].Value.ToTitleCase();
                    }
                }
                else
                {
                    current = segment.ToTitleCase();
                }

                // OpenAPI spec recommends following "programming naming conventions" 
                // for operationId to allow tools to utilize this property.
                // To be safe, we only allow alphanumeric characters.
                operationId.Append(Regex.Replace(current, "[^a-zA-Z0-9]", string.Empty));
            }

            return operationId.ToString();
        }

        /// <summary>
        /// Extracts the operation method from the operation element
        /// </summary>
        /// <exception cref="InvalidVerbException">Thrown if the verb is missing or has invalid format.</exception>
        public static OperationType GetOperationMethod(
            string url,
            XElement operationElement)
        {
            var verbElement = operationElement.Descendants().FirstOrDefault(i => i.Name == KnownXmlStrings.Verb);

            var verb = verbElement?.Value.Trim();

            OperationType operationMethod;

            if (Enum.TryParse(verb, true, out operationMethod))
            {
                return operationMethod;
            }

            throw new InvalidVerbException(verb);
        }

        /// <summary>
        /// Extracts the URL from the operation element
        /// </summary>
        /// <exception cref="InvalidUrlException">Thrown if the URL is missing or has invalid format.</exception>
        public static string GetUrl(
            XElement operationElement)
        {
            var urls = operationElement.Elements(KnownXmlStrings.Url).Select(i => i.Value);

            // Can't process further if no url is documented, so skip the operation.
            var url = urls.FirstOrDefault();

            if (url == null)
            {
                throw new InvalidUrlException(
                    url,
                    SpecificationGenerationMessages.NullUrl);
            }

            try
            {
                url = HttpUtility.UrlDecode(new Uri(url).AbsolutePath);
            }
            catch (UriFormatException)
            {
                throw new InvalidUrlException(
                    url,
                    SpecificationGenerationMessages.MalformattedUrl);
            }
            catch (Exception e)
            {
                throw new InvalidUrlException(
                    url,
                    e.Message);
            }

            return url;
        }
    }
}