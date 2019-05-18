// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Exceptions;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Extensions;

#if !NETFRAMEWORK
using System.Runtime.Loader;
#endif

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration
{
    /// <summary>
    /// Proxy class for fetching type information by loading assemblies.
    /// </summary>
    public class TypeFetcher
    {
        private readonly IDictionary<Type, IList<Type>> _baseTypeMap = new Dictionary<Type, IList<Type>>();
        private readonly IList<string> _contractAssemblyPaths = new List<string>();
        private readonly IDictionary<string, Type> _typeMap = new Dictionary<string, Type>();

#if !NETFRAMEWORK
        private readonly AssemblyLoadContext _context;

        /// <summary>
        /// Creates new instance of <see cref="TypeFetcher"/>.
        /// </summary>
        /// <param name="contractAssemblyPaths">The list of contract assembly paths.</param>
        /// <param name="context">The assembly load context.</param>
        public TypeFetcher(IList<string> contractAssemblyPaths, AssemblyLoadContext context)
        {
            _context = context;
            _contractAssemblyPaths = contractAssemblyPaths;
        }
#else
        /// <summary>
        /// Creates new instance of <see cref="TypeFetcher"/>.
        /// </summary>
        /// <param name="contractAssemblyPaths">The list of contract assembly paths.</param>
        public TypeFetcher(IList<string> contractAssemblyPaths)
        {
            _contractAssemblyPaths = contractAssemblyPaths;
        }
#endif

        private Type CreateListType(string typeName)
        {
            var listType = typeof(IList<>);

            // Creates a list type, setting the provided type as the generic type.
            return listType.MakeGenericType(LoadType(typeName));
        }

        /// <summary>
        /// Handle generic types. These appear in xml annotations as follows:
        /// <![CDATA[
        ///    <see cref="T:Contracts.Generic`1"/>
        ///    <see cref="T:Contracts.CustomClass"/>
        ///    Equivalent to Generic<CustomClass>
        /// ]]>
        /// </summary>
        /// <param name="allTypeNames">The list of type names specified in the documentation.</param>
        /// <returns>The generic type.</returns>
        private Type ExtractGenericType(IList<string> allTypeNames)
        {
            var start = 0;
            return ExtractGenericTypeRecurse(allTypeNames, ref start);
        }

        /// <summary>
        /// This method should only be called by ExtractGenericType.
        /// </summary>
        /// <param name="allTypeNames">The list of type names specified in the documentation.</param>
        /// <param name="index">
        /// Reference to current index in allTypes. It is incremented in different layers of the recursive procedure.
        /// </param>
        /// <returns>The generic type.</returns>
        private Type ExtractGenericTypeRecurse(IList<string> allTypeNames, ref int index)
        {
            if (index >= allTypeNames.Count)
            {
                throw new UndocumentedGenericTypeException();
            }

            var currentTypeName = allTypeNames[index];
            var numberOfGenerics = ExtractNumberOfGenerics(currentTypeName);

            // A generic type was expected, but the documented type is a non-generic.
            // Generic types must be documented in order.
            if (numberOfGenerics == 0)
            {
                throw new UnorderedGenericTypeException();
            }

            var genericTypeArray = new Type[numberOfGenerics];

            // Iterate over documented generics, retrieving their respective types.
            for (var j = 0; j < numberOfGenerics; j++)
            {
                if (++index >= allTypeNames.Count)
                {
                    throw new UndocumentedGenericTypeException();
                }

                var currentGenericTypeName = allTypeNames[index];

                // If another generic is encountered, recurse. Otherwise, load the type.
                genericTypeArray[j] = IsGenericType(currentGenericTypeName)
                    ? ExtractGenericTypeRecurse(allTypeNames, ref index)
                    : currentGenericTypeName.Contains("[]")
                        ? CreateListType(currentGenericTypeName.Split('[')[0])
                        : LoadType(currentGenericTypeName);
            }

            var type = LoadType(currentTypeName);

            // Load type and set generic types
            return type.MakeGenericType(genericTypeArray);
        }

        /// <summary>
        /// Extracts the number of generics specified in the type name.
        /// </summary>
        /// <param name="typeName">The type name.</param>
        /// <returns>The number of generics.</returns>
        private static int ExtractNumberOfGenerics(string typeName)
        {
            return IsGenericType(typeName) ? int.Parse(typeName.Split('`')[1], CultureInfo.CurrentCulture) : 0;
        }

        /// <summary>
        /// Get base types for provided type.
        /// </summary>
        /// <param name="type">The type to fetch base types for.</param>
        /// <returns>The list of base types.</returns>
        public IList<Type> GetBaseTypes(Type type)
        {
            if (!_baseTypeMap.ContainsKey(type))
            {
                _baseTypeMap.Add(type, type.GetBaseTypes());
            }

            return _baseTypeMap[type];
        }

        private static bool IsGenericType(string typeName)
        {
            return typeName.Contains("`");
        }

        /// <summary>
        /// Loads the given type name from assemblies located in the given assembly path.
        /// </summary>
        /// <param name="typeName">The type name.</param>
        /// <exception cref="TypeLoadException">Thrown when type was not found.</exception>
        /// <returns>The type.</returns>
        public Type LoadType(string typeName)
        {
            if (_typeMap.ContainsKey(typeName))
            {
                return _typeMap[typeName];
            }

            // Try to fetch the type from the already loaded assemblies, so we do not load duplicate assemblies in
            // app domain.
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic);

            foreach (var loadedAssembly in loadedAssemblies)
            {
                var contractType = loadedAssembly.GetType(typeName);

                if (contractType == null)
                {
                    continue;
                }

                _typeMap.Add(typeName, contractType);

                return contractType;
            }

#if !NETFRAMEWORK
            foreach (var contractAssemblyPath in this._contractAssemblyPaths)
            {
                var assembly = _context.LoadFromAssemblyPath(contractAssemblyPath);
                var contractType = assembly.GetType(typeName);

                if (contractType == null)
                {
                    continue;
                }

                _typeMap.Add(typeName, contractType);
                return contractType;
            }
#else
            // Load custom type from the given list of assemblies.
            foreach (var contractAssemblyPath in _contractAssemblyPaths)
            {
                var assembly = Assembly.LoadFrom(contractAssemblyPath);
                var contractType = assembly.GetType(typeName);

                if (contractType == null)
                {
                    continue;
                }

                _typeMap.Add(typeName, contractType);
                return contractType;
            }
#endif
            var errorMessage = string.Format(
                SpecificationGenerationMessages.TypeNotFound,
                typeName,
                string.Join(" ", _contractAssemblyPaths.Select(Path.GetFileName)));

            throw new TypeLoadException(errorMessage);
        }

        /// <summary>
        /// Gets the type from the cref value.
        /// </summary>
        /// <param name="crefValues">The list of cref values.</param>
        /// <returns>The type.</returns>
        public Type LoadTypeFromCrefValues(IList<string> crefValues)
        {
            if (!crefValues.Any())
            {
                return null;
            }

            string typeName;

            if (crefValues.First().Contains("[]"))
            {
                var crefValue = crefValues.First().Split('[')[0];

                typeName = crefValue.ExtractTypeNameFromCref();
                return CreateListType(typeName);
            }

            if (crefValues.Any(IsGenericType))
            {
                return ExtractGenericType(crefValues.Select(i => i.ExtractTypeNameFromCref()).ToList());
            }

            typeName = crefValues.First().ExtractTypeNameFromCref();
            return LoadType(typeName);
        }
    }
}