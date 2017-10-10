// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections;
using System.Linq;
using Microsoft.OpenApiSpecification.Core.Models;
using Newtonsoft.Json;

namespace Microsoft.OpenApiSpecification.Core.Serialization
{
    /// <summary>
    /// Custom json converter for <see cref="IExtensible"/>
    /// </summary>
    internal class ExtensibleJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IExtensible);
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var extensible = (IExtensible)Activator.CreateInstance(objectType);

            while (reader.Read() && reader.TokenType == JsonToken.PropertyName)
            {
                var readPropertyName = reader.Value.ToString();
                reader.Read();

                var allProperties = objectType.GetProperties().ToList();

                var matchedProperty = allProperties.FirstOrDefault(
                    propertyInfo =>
                    {
                        var jsonPropertyAttributes = (JsonPropertyAttribute[])propertyInfo.GetCustomAttributes(typeof(JsonPropertyAttribute), inherit: false);

                        string propertyName;
                        if (jsonPropertyAttributes.Any() && jsonPropertyAttributes[0].PropertyName != null)
                        {
                            propertyName = jsonPropertyAttributes[0].PropertyName;
                        }
                        else
                        {
                            propertyName = propertyInfo.Name;
                        }

                        return propertyName == readPropertyName;
                    });

                if (matchedProperty != null)
                {
                    matchedProperty.SetValue(extensible, serializer.Deserialize(reader, matchedProperty.PropertyType));
                }
                else
                {
                    extensible.Extensions.Add(readPropertyName, serializer.Deserialize(reader, typeof(object)));
                }
            }

            return extensible;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var extensible = (IExtensible)value;
            writer.WriteStartObject();

            foreach (var propertyInfo in extensible.GetType().GetProperties())
            {
                if (propertyInfo.Name == "Extensions")
                {
                    continue;
                }

                var propertyValue = propertyInfo.GetValue(extensible);

                if (propertyInfo.PropertyType.IsValueType &&
                    (propertyValue == null ||
                        propertyValue.Equals(Activator.CreateInstance(propertyInfo.PropertyType))))
                {
                    continue;
                }

                if (!propertyInfo.PropertyType.IsValueType && propertyValue == null)
                {
                    continue;
                }

                if (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType) &&
                    !((IEnumerable)propertyValue).GetEnumerator().MoveNext())
                {
                    continue;
                }

                var jsonPropertyAttributes = (JsonPropertyAttribute[])propertyInfo.GetCustomAttributes(typeof(JsonPropertyAttribute), inherit: false);

                string propertyName;
                if (jsonPropertyAttributes.Any() && jsonPropertyAttributes[0].PropertyName != null)
                {
                    propertyName = jsonPropertyAttributes[0].PropertyName;
                }
                else
                {
                    propertyName = propertyInfo.Name;
                }

                writer.WritePropertyName(propertyName);
                serializer.Serialize(writer, propertyInfo.GetValue(extensible));
            }

            foreach (var extension in extensible.Extensions)
            {
                writer.WritePropertyName(extension.Key);
                serializer.Serialize(writer, extension.Value);
            }

            writer.WriteEndObject();
        }
    }
}