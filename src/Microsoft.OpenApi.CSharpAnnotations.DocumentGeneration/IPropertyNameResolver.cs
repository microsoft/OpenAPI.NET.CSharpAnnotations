// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Reflection;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.ReferenceRegistries;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration
{
    /// <summary>
    /// Used by <see cref="SchemaReferenceRegistry"/> to resolve property name for a given <see cref="PropertyInfo"/>.
    /// </summary>
    public interface IPropertyNameResolver
    {
        /// <summary>
        /// Resolves the property name for a given property info.
        /// </summary>
        /// <param name="propertyInfo">The property info to resolve property name for.</param>
        /// <returns>The property name.</returns>
        string ResolvePropertyName(PropertyInfo propertyInfo);
    }
}