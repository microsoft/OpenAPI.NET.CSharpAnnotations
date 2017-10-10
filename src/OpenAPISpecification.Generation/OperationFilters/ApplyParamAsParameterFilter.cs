// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.OpenApiSpecification.Core.Models;
using Microsoft.OpenApiSpecification.Generation.Exceptions;
using Microsoft.OpenApiSpecification.Generation.Extensions;
using Microsoft.OpenApiSpecification.Generation.Models;
using Microsoft.OpenApiSpecification.Generation.Models.KnownStrings;

namespace Microsoft.OpenApiSpecification.Generation.OperationFilters
{
    /// <summary>
    /// Parses the value of param tag in xml documentation and apply that as parameter in operation.
    /// </summary>
    public class ApplyParamAsParameterFilter : IOperationFilter
    {
        /// <summary>
        /// Fetches the value of "param" tags from xml documentation and populates operation's parameters values.
        /// </summary>
        /// <param name="operation">The operation to be updated.</param>
        /// <param name="element">The xml element representing an operation in the annotation xml.</param>
        /// <param name="settings">The operation filter settings.</param>
        /// <remarks>
        /// Care should be taken to not overwrite the existing value in Operation if already present.
        /// This guarantees the predictable behavior that the first tag in the XML will be respected.
        /// It also guarantees that common annotations in the config file do not overwrite the
        /// annotations in the main documentation.
        /// </remarks>
        public void Apply(Operation operation, XElement element, OperationFilterSettings settings)
        {
            var paramElements = element.Elements()
                .Where(
                    p => p.Name == KnownXmlStrings.Param)
                .ToList();

            var paramElementsWithoutIn = paramElements.Where(
                    p =>
                        !KnownXmlStrings.AllowedInValues.Contains(p.Attribute(KnownXmlStrings.In)?.Value))
                .ToList();

            var url = element.Elements().FirstOrDefault(p => p.Name == KnownXmlStrings.Url)?.Value;

            if (!string.IsNullOrWhiteSpace(url))
            {
                foreach (var paramElement in paramElementsWithoutIn)
                {
                    var paramName = paramElement.Attribute(KnownXmlStrings.Name)?.Value;

                    if (url.Contains(
                            $"/{{{paramName}}}",
                            StringComparison.InvariantCultureIgnoreCase) &&
                        url.Contains(
                            $"={{{paramName}}}",
                            StringComparison.InvariantCultureIgnoreCase))
                    {
                        // The parameter is in both path and query. We cannot determine what to put for "in" attribute.
                        throw new ConflictingPathAndQueryParametersException(paramName, url);
                    }

                    if (url.Contains(
                        $"/{{{paramName}}}",
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        paramElement.Add(new XAttribute(KnownXmlStrings.In, KnownXmlStrings.Path));
                    }
                    else if (url.Contains(
                        $"={{{paramName}}}",
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        paramElement.Add(new XAttribute(KnownXmlStrings.In, KnownXmlStrings.Query));
                    }
                }

                var pathParamElements = paramElements.Where(p => p.Attribute(KnownXmlStrings.In)?.Value == KnownXmlStrings.Path).ToList();

                var matches = new Regex(@"\{(.*?)\}").Matches(url.Split('?')[0]);

                foreach (Match match in matches)
                {
                    var pathParamNameFromUrl = match.Groups[1].Value;

                    // All path params in the URL must be documented.
                    if (!pathParamElements.Any(p => p.Attribute(KnownXmlStrings.Name)?.Value == pathParamNameFromUrl))
                    {
                        throw new UndocumentedPathParameterException(pathParamNameFromUrl, url);
                    }
                }
            }

            paramElementsWithoutIn = paramElements.Where(
                    p =>
                        !KnownXmlStrings.AllowedInValues.Contains(p.Attribute(KnownXmlStrings.In)?.Value))
                .ToList();

            if (paramElementsWithoutIn.Any())
            {
                throw new MissingInAttributeException(
                    paramElementsWithoutIn.Select(
                            p => p.Attribute(KnownXmlStrings.Name)?.Value)
                        .ToList());
            }

            // Query paramElements again to get all the parameter elements that have "in" attribute.
            // This will include those whose "in" attribute were just populated above, but exclude
            // those that have "in" attribute being "body" since they will be handled as a request body.
            var paramElementsWithIn = paramElements.Where(
                p =>
                    KnownXmlStrings.InValuesTranslatableToParameter.Contains(
                        p.Attribute(KnownXmlStrings.In)?.Value));

            var isRequired = false;
            var parameters = new List<Parameter>();

            foreach (var paramElement in paramElementsWithIn)
            {
                if (paramElement.Attribute(KnownXmlStrings.IsRequired)?.Value.Trim() != null)
                {
                    isRequired = Convert.ToBoolean(paramElement.Attribute(KnownXmlStrings.IsRequired).Value);
                }

                var inValue = paramElement.Attribute(KnownXmlStrings.In)?.Value.Trim();
                var name = paramElement.Attribute(KnownXmlStrings.Name)?.Value.Trim();
                var cref = paramElement.Attribute(KnownXmlStrings.Cref)?.Value.Trim();
                var description = paramElement.Attribute(KnownXmlStrings.Description)?.Value.RemoveBlankLines();

                // TODO: Handle System.Array param type.
                //
                var dataType = GetDataTypeValue(cref);

                parameters.Add(
                    new Parameter
                    {
                        Name = name,
                        In = GetParameterKind(inValue),
                        Description = description,
                        IsRequired = isRequired,

                        Schema = new Schema
                        {
                            Type = dataType.DataType,
                            Format = dataType.Format
                        }
                    });
            }

            foreach (var parameter in parameters)
            {
                if (operation.Parameters.All(p => parameter.Name != p.Name))
                {
                    operation.Parameters.Add(parameter);
                }
            }
        }

        private static OpenApiDataTypeFormatPair GetDataTypeValue(string cref)
        {
            if (cref != null && cref.Split(':')[0].Trim() == KnownXmlStrings.T)
            {
                var typeName = cref.Split(':')[1].Trim();

                return Type.GetType(typeName).MapToOpenApiDataTypeFormatPair();
            }

            return typeof(string).MapToOpenApiDataTypeFormatPair();
        }

        private static ParameterKind GetParameterKind(string kind)
        {
            switch (kind)
            {
                case KnownXmlStrings.Header:
                    return ParameterKind.Header;

                case KnownXmlStrings.Query:
                    return ParameterKind.Query;

                default:
                    return ParameterKind.Path;
            }
        }
    }
}