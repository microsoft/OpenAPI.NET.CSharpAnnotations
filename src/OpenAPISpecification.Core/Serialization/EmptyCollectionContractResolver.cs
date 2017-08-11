// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Microsoft.OpenApiSpecification.Core.Serialization
{
    /// <summary>
    /// Custom JsonConvert that allows you to ignore empty collection properties during serialization.
    /// </summary>
    public class EmptyCollectionContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            var shouldSerialize = property.ShouldSerialize;
            property.ShouldSerialize = obj => (shouldSerialize == null || shouldSerialize(obj))
                && !IsEmptyCollection(property, obj);

            return property;
        }

        private static bool IsEmptyCollection(JsonProperty property, object target)
        {
            var value = property.ValueProvider.GetValue(target);
            var collection = value as ICollection;

            if (collection != null && collection.Count == 0)
            {
                return true;
            }

            if (!typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
            {
                return false;
            }

            var countProp = property.PropertyType.GetProperty("Count");

            if (countProp == null)
            {
                return false;
            }

            var count = (int) countProp.GetValue(value, null);

            return count == 0;
        }
    }
}