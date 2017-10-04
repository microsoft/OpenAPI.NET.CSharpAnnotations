// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Linq;
using System.Xml.Linq;
using Microsoft.OpenApiSpecification.Core.Models;
using Microsoft.OpenApiSpecification.Generation.Extensions;
using Microsoft.OpenApiSpecification.Generation.Models;

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
        /// <remarks>
        /// Care should be taken to not overwrite the existing value in Operation if already present.
        /// This guarantees the predictable behavior that the first tag in the XML will be respected.
        /// It also guarantees that common annotations in the config file do not overwrite the
        /// annotations in the main documentation.
        /// </remarks>
        public void Apply(Operation operation, XElement element, OperationFilterSettings settings)
        {
            string description = null;
            var descriptionElement = element.Descendants().FirstOrDefault(i => i.Name == KnownStrings.Remarks);

            if (descriptionElement != null)
            {
                description = descriptionElement.Value.Trim().RemoveBlankLines();
            }

            if (string.IsNullOrWhiteSpace(operation.Description))
            {
                operation.Description = description;
            }
        }
    }
}