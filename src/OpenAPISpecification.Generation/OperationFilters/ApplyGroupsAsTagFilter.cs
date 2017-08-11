// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Linq;
using System.Xml.Linq;
using Microsoft.OpenApiSpecification.Core.Models;

namespace Microsoft.OpenApiSpecification.Generation.OperationFilters
{
    /// <summary>
    /// Parses the value of group tag in xml documentation and apply that as tag in operation.
    /// </summary>
    public class ApplyGroupsAsTagFilter : IOperationFilter
    {
        /// <summary>
        /// Fetches the value of "group" tag from xml documentation and populates operation's tag.
        /// </summary>
        /// <param name="operation">The operation to be updated.</param>
        /// <param name="element">The xml element representing an operation in the annotation xml.</param>
        public void Apply(Operation operation, XElement element)
        {
            var groupElement = element.Descendants().FirstOrDefault(i => i.Name == "group");

            if (groupElement == null)
            {
                return;
            }

            operation.Tags.Add(groupElement.Value.Trim());
        }
    }
}