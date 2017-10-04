// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.OpenApiSpecification.Core.Models;
using Microsoft.OpenApiSpecification.Generation.Extensions;
using Microsoft.OpenApiSpecification.Generation.Models;

namespace Microsoft.OpenApiSpecification.Generation.OperationFilters
{
    /// <summary>
    /// Parses the value of param tag in xml documentation and apply that as parameter in operation.
    /// </summary>
    public class ApplyParamAsParameterFilter : IOperationFilter
    {
        private static OpenApiDataTypeFormatPair GetDataTypeValue(string cref)
        {
            if (cref != null && cref.Split(':')[0].Trim() == KnownStrings.T)
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
                case KnownStrings.Header:
                    return ParameterKind.Header;

                case KnownStrings.Query:
                    return ParameterKind.Query;

                default:
                    return ParameterKind.Path;
            }
        }

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
                    p => p.Name == KnownStrings.Param &&
                        (p.Attribute(KnownStrings.In)?.Value == KnownStrings.Path||
                            p.Attribute(KnownStrings.In)?.Value == KnownStrings.Query ||
                            p.Attribute(KnownStrings.In)?.Value == KnownStrings.Header ));

            var isRequired = false;
            var parameters = new List<Parameter>();

            foreach (var paramElement in paramElements)
            {
                if (paramElement.Attribute(KnownStrings.IsRequired)?.Value.Trim() != null)
                {
                    isRequired = Convert.ToBoolean(paramElement.Attribute(KnownStrings.IsRequired).Value);
                }

                var inValue = paramElement.Attribute(KnownStrings.In)?.Value.Trim();
                var name = paramElement.Attribute(KnownStrings.Name)?.Value.Trim();
                var cref = paramElement.Attribute(KnownStrings.Cref)?.Value.Trim();
                var description = paramElement.Attribute(KnownStrings.Description)?.Value.RemoveBlankLines();

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
    }
}