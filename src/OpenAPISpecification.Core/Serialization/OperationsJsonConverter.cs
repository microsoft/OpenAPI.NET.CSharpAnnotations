// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OpenApiSpecification.Core.Models;
using Newtonsoft.Json;

namespace Microsoft.OpenApiSpecification.Core.Serialization
{
    /// <summary>
    /// Custom json converter for <see cref="Operations"/>
    /// </summary>
    internal class OperationsJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Operations);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var operations = new Operations();

            while (reader.Read() && reader.TokenType == JsonToken.PropertyName)
            {
                var propertyName = reader.Value.ToString();
                reader.Read();

                switch (propertyName)
                {
                    case "summary":
                        operations.Summary = (string) serializer.Deserialize(reader, typeof(string));
                        break;

                    case "description":
                        operations.Description = (string) serializer.Deserialize(reader, typeof(string));
                        break;

                    case "parameters":
                        operations.Parameters =
                            (IDictionary<string, Parameter>) serializer.Deserialize(reader,
                                typeof(IDictionary<string, Parameter>));
                        break;

                    case "servers":
                        operations.Servers = (IList<Server>) serializer.Deserialize(reader, typeof(IList<Server>));
                        break;

                    default:
                        var key = (OperationMethod) Enum.Parse(enumType: typeof(OperationMethod), value: propertyName,
                            ignoreCase: true);
                        var value = (Operation) serializer.Deserialize(reader, typeof(Operation));
                        operations.Add(key, value);
                        break;
                }
            }

            return operations;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var operations = (Operations) value;
            writer.WriteStartObject();

            if (operations.Summary != null)
            {
                writer.WritePropertyName(nameof(operations.Summary).ToLower());
                serializer.Serialize(writer, operations.Summary);
            }

            if (operations.Description != null)
            {
                writer.WritePropertyName(nameof(operations.Description).ToLower());
                serializer.Serialize(writer, operations.Description);
            }

            if (operations.Parameters != null && operations.Parameters.Count > 0)
            {
                writer.WritePropertyName(nameof(operations.Parameters).ToLower());
                serializer.Serialize(writer, operations.Parameters);
            }

            if (operations.Servers != null && operations.Servers.Count > 0)
            {
                writer.WritePropertyName(nameof(operations.Servers).ToLower());
                serializer.Serialize(writer, operations.Servers);
            }

            // Open api spec needs the operation method to be lower cased so convert dictionary keys to lower case.
            foreach (var pair in operations)
            {
                writer.WritePropertyName(pair.Key.ToString().ToLowerInvariant());
                serializer.Serialize(writer, pair.Value);
            }

            writer.WriteEndObject();
        }
    }
}