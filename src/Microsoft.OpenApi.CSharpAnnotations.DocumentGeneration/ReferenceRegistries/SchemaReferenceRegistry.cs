// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Exceptions;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Extensions;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.ReferenceRegistries
{
    /// <summary>
    /// Reference Registry for <see cref="OpenApiSchema"/>
    /// </summary>
    public class SchemaReferenceRegistry : ReferenceRegistry<Type, OpenApiSchema>
    {
        /// <summary>
        /// The dictionary containing all references of the given type.
        /// </summary>
        public override IDictionary<string, OpenApiSchema> References { get; } = new Dictionary<string, OpenApiSchema>();

        /// <summary>
        /// Finds the existing reference object based on the key from the input or creates a new one.
        /// </summary>
        /// <returns>The existing or created reference object.</returns>
        internal override OpenApiSchema FindOrAddReference(Type input)
        {
            // Return empty schema when the type does not have a name. 
            // This can occur, for example, when a generic type without the generic argument specified
            // is passed in.
            if (input == null || input.FullName == null)
            {
                return new OpenApiSchema();
            }

            var key = GetKey(input);

            // If the schema already exists in the References, simply return.
            if (References.ContainsKey(key))
            {
                return new OpenApiSchema
                {
                    Reference = new OpenApiReference()
                    {
                        Id = key,
                        Type = ReferenceType.Schema
                    }
                };
            }
            
            try
            {
                // There are multiple cases for input types that should be handled differently to match the OpenAPI spec.
                //
                // 1. Simple Type
                // 2. Enum Type
                // 3. Dictionary Type
                // 4. Enumerable Type
                // 5. Object Type
                var schema = new OpenApiSchema();

                if (input.IsSimple())
                {
                    schema = input.MapToOpenApiSchema();

                    // Certain simple types yield more specific information.
                    if (input == typeof(char))
                    {
                        schema.MinLength = 1;
                        schema.MaxLength = 1;
                    }
                    else if (input == typeof(Guid))
                    {
                        schema.Example = new OpenApiString(Guid.Empty.ToString());
                    }

                    return schema;
                }

                if (input.IsEnum)
                {
                    schema.Type = "string";
                    foreach (var name in Enum.GetNames(input))
                    {
                        schema.Enum.Add(new OpenApiString(name));
                    }

                    return schema;
                }

                if (input.IsDictionary())
                {
                    schema.Type = "object";
                    schema.AdditionalProperties = FindOrAddReference(input.GetGenericArguments()[1]);

                    return schema;
                }

                if (input.IsEnumerable())
                {
                    schema.Type = "array";

                    schema.Items = FindOrAddReference(input.GetEnumerableItemType());

                    return schema;
                }

                schema.Type = "object";
                
                // Note this assignment is necessary to allow self-referencing type to finish
                // without causing stack overflow.
                // We can also assume that the schema is an object type at this point.
                References[key] = schema;

                foreach ( var propertyInfo in input.GetProperties() )
                {
                    var propertyName = propertyInfo.Name;
                    var innerSchema = FindOrAddReference( propertyInfo.PropertyType );

                    // Check if the property is read-only.
                    innerSchema.ReadOnly = !propertyInfo.CanWrite;

                    var attributes = propertyInfo.GetCustomAttributes( false );

                    foreach (var attribute in attributes)
                    {
                        if (attribute.GetType().FullName == "Newtonsoft.Json.JsonPropertyAttribute")
                        {
                            Type type = attribute.GetType();
                            PropertyInfo propertyNameInfo = type.GetProperty("PropertyName");

                            if (propertyNameInfo != null)
                            {
                                var jsonPropertyName = (string)propertyNameInfo.GetValue(attribute, null);

                                if(!string.IsNullOrWhiteSpace(jsonPropertyName))
                                {
                                    propertyName = jsonPropertyName;
                                }
                            }

                            PropertyInfo requiredPropertInfo = type.GetProperty("Required");

                            if (requiredPropertInfo != null)
                            {
                                var requiredValue = Enum.GetName(
                                    requiredPropertInfo.PropertyType,
                                    requiredPropertInfo.GetValue(attribute, null));

                                if (requiredValue == "Always" )
                                {
                                    schema.Required.Add(propertyName);
                                }
                            }
                        }
                    }

                    schema.Properties[propertyName] = innerSchema;
                }

                References[key] = schema;

                return new OpenApiSchema
                {
                    Reference = new OpenApiReference()
                    {
                        Id = key,
                        Type = ReferenceType.Schema
                    }
                };
            }
            catch (Exception e)
            {
                throw new AddingSchemaReferenceFailedException(key, e.Message);
            }
        }

        /// <summary>
        /// Gets the key from the input object to use as reference string.
        /// </summary>
        /// <remarks>
        /// This must match the regular expression ^[a-zA-Z0-9\.\-_]+$ due to OpenAPI V3 spec.
        /// </remarks>
        protected override string GetKey(Type input)
        {
            // Type.ToString() returns full name for non-generic types and
            // returns a full name without unnecessary assembly information for generic types.
            var typeName = input.ToString();

            return typeName.SanitizeClassName();
        }
    }
}