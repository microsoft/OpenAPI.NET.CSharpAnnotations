// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.OpenApiSpecification.Core.Models;
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

            // Query paramElements again to get all the parameter elements that have "in" attribute.
            // This will include those whose "in" attribute were just populated above, but exclude
            // those that have "in" attribute being "body" since they will be handled as a request body.
            var paramElementsWithIn = paramElements.Where(
                    p =>
                        KnownXmlStrings.InValuesTranslatableToParameter.Contains(
                            p.Attribute(KnownXmlStrings.In)?.Value))
                .ToList();

            foreach (var paramElement in paramElementsWithIn)
            {
                var inValue = paramElement.Attribute(KnownXmlStrings.In)?.Value.Trim();
                var name = paramElement.Attribute(KnownXmlStrings.Name)?.Value.Trim();

                if (inValue == KnownXmlStrings.Path &&
                    !settings.Path.Contains($"{{{name}}}", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                var isRequired = paramElement.Attribute(KnownXmlStrings.Required)?.Value.Trim();
                var cref = paramElement.Attribute(KnownXmlStrings.Cref)?.Value.Trim();
                var description = paramElement.Attribute(KnownXmlStrings.Description)?.Value.RemoveBlankLines();

                // TODO: Handle System.Array param type.
                //
                var dataType = GetDataTypeValue(cref);

                operation.Parameters.Add(
                    new Parameter
                    {
                        Name = name,
                        In = GetParameterKind(inValue),
                        Description = description,
                        IsRequired = Convert.ToBoolean(isRequired),

                        Schema = new Schema
                        {
                            Type = dataType.DataType,
                            Format = dataType.Format
                        }
                    });
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