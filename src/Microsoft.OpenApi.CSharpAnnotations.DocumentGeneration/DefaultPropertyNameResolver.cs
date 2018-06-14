// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Linq;
using System.Reflection;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.ReferenceRegistries;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration
{
    /// <summary>
    /// Used by <see cref="SchemaReferenceRegistry"/> to resolve property name for a given <see cref="PropertyInfo"/>.
    /// </summary>
    public class DefaultPropertyNameResolver : IPropertyNameResolver
    {
        /// <summary>
        /// Resolves the property name for the given property info.
        /// </summary>
        /// <param name="propertyInfo">The property info to resolve property name for.</param>
        /// <returns>The property info.</returns>
        public string ResolvePropertyName(PropertyInfo propertyInfo)
        {
            var propertyName = propertyInfo.Name;
            var attributes = propertyInfo.GetCustomAttributes(false);

            var jsonPropertyAttribute =
                attributes?.Where(i => i.GetType().FullName == "Newtonsoft.Json.JsonPropertyAttribute")
                    .FirstOrDefault();

            if (jsonPropertyAttribute == null)
            {
                return propertyName;
            }

            var type = jsonPropertyAttribute.GetType();
            var propertyNameInfo = type.GetProperty("PropertyName");

            if (propertyNameInfo == null)
            {
                return propertyName;
            }

            var jsonPropertyName = (string) propertyNameInfo.GetValue(jsonPropertyAttribute, null);

            if (!string.IsNullOrWhiteSpace(jsonPropertyName))
            {
                propertyName = jsonPropertyName;
            }

            return propertyName;
        }
    }
}