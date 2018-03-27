// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests
{
    /// <summary>
    /// Custom json converter for a dictionary with non-primitive key type.
    /// </summary>
    internal class DictionaryJsonConverter<TKey, TValue> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(IDictionary).IsAssignableFrom(objectType);
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

            var deserializedObject = new Dictionary<TKey, TValue>();

            while (reader.Read() && reader.TokenType == JsonToken.PropertyName)
            {
                var propertyName = reader.Value.ToString();
                reader.Read();

                var key = JsonConvert.DeserializeObject<TKey>(propertyName);
                var value = (TValue)serializer.Deserialize(reader, typeof(TValue));
                deserializedObject.Add(key, value);
            }

            return deserializedObject;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var deserializedObject = (Dictionary<TKey, TValue>)value;
            writer.WriteStartObject();
            foreach (var pair in deserializedObject)
            {
                writer.WritePropertyName(pair.Key.ToString());
                serializer.Serialize(writer, pair.Value);
            }

            writer.WriteEndObject();
        }
    }
}