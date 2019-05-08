using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration
{
    /// <summary>
    /// Custom json converter for a dictionary with non-primitive key type.
    /// </summary>
    internal class SchemaTypeInfoJsonConverter : JsonConverter
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

            var schemaTypeInfo = new SchemaTypeInfo();

            while (reader.Read() && reader.TokenType == JsonToken.PropertyName)
            {
                var propertyName = reader.Value.ToString();
                reader.Read();

                switch (propertyName)
                {
                    case nameof(SchemaTypeInfo.CrefToFieldValueMap):
                        schemaTypeInfo.CrefToFieldValueMap = (Dictionary<string,string>)
                            serializer.Deserialize(reader, typeof(Dictionary<string, string>));
                        break;

                    case nameof(SchemaTypeInfo.CrefToSchemaMap):
                        schemaTypeInfo.CrefToSchemaMap = (Dictionary<string, SchemaInfo>)serializer.Deserialize(
                            reader,
                            typeof(Dictionary<string, SchemaInfo>));
                        break;

                    case nameof(SchemaTypeInfo.VariantSchemaReferenceMap):
                        reader.Read();
                        propertyName = reader.Value.ToString();
                        reader.Read();
                        var key = JsonConvert.DeserializeObject<DocumentVariantInfo>(propertyName);
                        var value = (Dictionary<string,string>)serializer.Deserialize(reader, typeof(Dictionary<string, string>));

                        schemaTypeInfo.VariantSchemaReferenceMap.Add(key,value);
                        break;

                    default:
                        // do nothing
                        break;
                }
            }

            return schemaTypeInfo;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var schemaTypeInfo = (SchemaTypeInfo)value;
            writer.WriteStartObject();

            if (schemaTypeInfo.CrefToFieldValueMap != null)
            {
                writer.WritePropertyName(nameof(schemaTypeInfo.CrefToFieldValueMap));
                serializer.Serialize(writer, schemaTypeInfo.CrefToFieldValueMap);
            }

            if (schemaTypeInfo.CrefToSchemaMap != null)
            {
                writer.WritePropertyName(nameof(schemaTypeInfo.CrefToSchemaMap));
                serializer.Serialize(writer, schemaTypeInfo.CrefToSchemaMap);
            }

            if (schemaTypeInfo.VariantSchemaReferenceMap != null)
            {
                writer.WritePropertyName(nameof(schemaTypeInfo.VariantSchemaReferenceMap));
                serializer.Serialize(writer, schemaTypeInfo.VariantSchemaReferenceMap);
            }

            writer.WriteEndObject();
        }
    }
}
