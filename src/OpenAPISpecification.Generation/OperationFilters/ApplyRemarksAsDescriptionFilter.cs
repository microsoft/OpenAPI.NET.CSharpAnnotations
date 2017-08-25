// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Linq;
using System.Xml.Linq;
using Microsoft.OpenApiSpecification.Core.Models;
using Microsoft.OpenApiSpecification.Generation.Extensions;

namespace Microsoft.OpenApiSpecification.Generation.OperationFilters
{
    /// <summary>
    /// Parses the value of remarks tag in xml documentation and apply that as description of the operation.
    /// </summary>
    public class ApplyRemarksAsDescriptionFilter : IOperationFilter
    {
        /// <summary>
        /// Fetches the value of "remarks" tag from xml documentation and populates operation's description.
        /// </summary>
        /// <param name="operation">The operation to be updated.</param>
        /// <param name="element">The xml element representing an operation in the annotation xml.</param>
        /// <param name="settings">The operation filter settings.</param>
        public void Apply(Operation operation, XElement element, OperationFilterSettings settings)
        {
            string description = null;
            var descriptionElement = element.Descendants().FirstOrDefault(i => i.Name == "remarks");

            if (descriptionElement != null)
            {
                description = descriptionElement.Value.Trim().RemoveBlankLines();
            }

            operation.Description = description;
        }
    }
}