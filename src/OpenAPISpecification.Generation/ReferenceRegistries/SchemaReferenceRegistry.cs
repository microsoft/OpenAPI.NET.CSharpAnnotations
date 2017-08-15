// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.OpenApiSpecification.Core.Models;
using Microsoft.OpenApiSpecification.Generation.Extensions;

namespace Microsoft.OpenApiSpecification.Generation.ReferenceRegistries
{
    /// <summary>
    /// Reference Registry for <see cref="Schema"/>
    /// </summary>
    public class SchemaReferenceRegistry : ReferenceRegistry<Type, Schema>
    {
        /// <summary>
        /// The dictionary containing all references of the given type.
        /// </summary>
        public override IDictionary<string, Schema> References { get; } = new Dictionary<string, Schema>();

        /// <summary>
        /// Finds the existing reference object based on the key from the input or creates a new one.
        /// </summary>
        /// <returns>The existing or created reference object.</returns>
        internal override Schema FindOrAddReference(Type input)
        {
            const string PathToSchemaReferences = "#/components/schemas/";

            var key = GetKey(input);

            // If the schema already exists in the References, simply return.
            if (References.ContainsKey(key))
            {
                return new Schema
                {
                    Reference = PathToSchemaReferences + key
                };
            }

            // There are multiple cases for input types that should be handled differently to match the OpenAPI spec.
            //
            // 1. Simple Type
            // 2. Enum Type
            // 3. Dictionary Type
            // 4. Enumerable Type
            // 5. Object Type
            var schema = new Schema();

            if (input.IsSimple())
            {
                var dataTypeAndFormatPair = input.MapToOpenApiDataTypeFormatPair();
                schema.Type = dataTypeAndFormatPair.DataType;
                schema.Format = dataTypeAndFormatPair.Format;

                return schema;
            }

            if (input.IsEnum)
            {
                schema.Type = "string";
                foreach (var name in Enum.GetNames(input))
                {
                    schema.Enum.Add(name);
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
            foreach (var propertyInfo in input.GetProperties())
            {
                var innerSchema = FindOrAddReference(propertyInfo.PropertyType);
                innerSchema.ReadOnly = !propertyInfo.CanWrite;

                schema.Properties[propertyInfo.Name] = innerSchema;
            }

            References[key] = schema;

            return new Schema
            {
                Reference = PathToSchemaReferences + key
            };
        }

        /// <summary>
        /// Gets the key from the input object to use as reference string.
        /// </summary>
        /// <remarks>This must match the regular expression ^[a-zA-Z0-9\.\-_]+$</remarks>
        protected override string GetKey(Type input)
        {
            return new Regex(@"[^a-zA-Z0-9\.\-_]").Replace(input.FullName, "_");
        }
    }
}