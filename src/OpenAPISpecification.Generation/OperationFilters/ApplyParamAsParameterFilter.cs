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
    /// Parses the value of group tag in xml documentation and apply that as parameter in operation.
    /// </summary>
    public class ApplyParamAsParameterFilter : IOperationFilter
    {
        /// <summary>
        /// Fetches the value of "param" tags from xml documentation and populates operation's parameters values.
        /// </summary>
        /// <param name="operation">The operation to be updated.</param>
        /// <param name="element">The xml element representing an operation in the annotation xml.</param>
        public void Apply(Operation operation, XElement element)
        {
            var paramElements = element.Elements().Where(p => p.Name == "param"
                && (p.Attribute("in")?.Value == "path" ||
                p.Attribute("in")?.Value == "query" ||
                p.Attribute("in")?.Value == "header"));

            var isRequired = false;
            var parameters = new List<Parameter>();

            foreach (var paramElement in paramElements)
            {
                if (paramElement.Attribute("isRequired")?.Value.Trim() != null)
                {
                    isRequired = Convert.ToBoolean(paramElement.Attribute("isRequired").Value);
                }

                var inValue = paramElement.Attribute("in")?.Value.Trim();
                var name = paramElement.Attribute("name")?.Value.Trim();
                var cref = paramElement.Attribute("cref")?.Value.Trim();
                var description = paramElement.Attribute("description")?.Value.RemoveBlankLines();
                
                // TODO: Handle System.Array param type.
                //
                var dataType = GetDataTypeValue(cref);

                parameters.Add(new Parameter
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
                operation.Parameters.Add(parameter);
            }
        }

        private static OpenApiDataTypeFormatPair GetDataTypeValue(string cref)
        {
            if (cref != null && cref.Split(':')[0].Trim() == "T")
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
                case "header":
                    return ParameterKind.Header;

                case "query":
                    return ParameterKind.Query;

                default:
                    return ParameterKind.Path;
            }
        }
    }
}