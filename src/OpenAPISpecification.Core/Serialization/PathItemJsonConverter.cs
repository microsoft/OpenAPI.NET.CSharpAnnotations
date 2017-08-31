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
    /// Custom json converter for <see cref="PathItem"/>
    /// </summary>
    internal class PathItemJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(PathItem);
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

            var pathItem = new PathItem();

            while (reader.Read() && reader.TokenType == JsonToken.PropertyName)
            {
                var propertyName = reader.Value.ToString();
                reader.Read();

                switch (propertyName)
                {
                    case "summary":
                        pathItem.Summary = (string) serializer.Deserialize(reader, typeof(string));
                        break;

                    case "description":
                        pathItem.Description = (string) serializer.Deserialize(reader, typeof(string));
                        break;

                    case "parameters":
                        pathItem.Parameters =
                            (IDictionary<string, Parameter>) serializer.Deserialize(
                                reader,
                                typeof(IDictionary<string, Parameter>));
                        break;

                    case "servers":
                        pathItem.Servers = (IList<Server>) serializer.Deserialize(reader, typeof(IList<Server>));
                        break;

                    default:
                        var key = (OperationMethod) Enum.Parse(
                            typeof(OperationMethod),
                            propertyName,
                            true);
                        var value = (Operation) serializer.Deserialize(reader, typeof(Operation));
                        pathItem.Add(key, value);
                        break;
                }
            }

            return pathItem;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var pathItem = (PathItem) value;
            writer.WriteStartObject();

            if (pathItem.Summary != null)
            {
                writer.WritePropertyName(nameof(pathItem.Summary).ToLower());
                serializer.Serialize(writer, pathItem.Summary);
            }

            if (pathItem.Description != null)
            {
                writer.WritePropertyName(nameof(pathItem.Description).ToLower());
                serializer.Serialize(writer, pathItem.Description);
            }

            if (pathItem.Parameters != null && pathItem.Parameters.Count > 0)
            {
                writer.WritePropertyName(nameof(pathItem.Parameters).ToLower());
                serializer.Serialize(writer, pathItem.Parameters);
            }

            if (pathItem.Servers != null && pathItem.Servers.Count > 0)
            {
                writer.WritePropertyName(nameof(pathItem.Servers).ToLower());
                serializer.Serialize(writer, pathItem.Servers);
            }

            // Open api spec needs the operation method to be lower cased so convert dictionary keys to lower case.
            foreach (var pair in pathItem)
            {
                writer.WritePropertyName(pair.Key.ToString().ToLowerInvariant());
                serializer.Serialize(writer, pair.Value);
            }

            writer.WriteEndObject();
        }
    }
}