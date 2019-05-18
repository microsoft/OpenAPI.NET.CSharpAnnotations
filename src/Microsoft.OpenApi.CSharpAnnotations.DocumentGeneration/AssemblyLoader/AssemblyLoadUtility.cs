// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.AssemblyLoader
{
    /// <summary>
    /// Contains reusable methods for loading assembly.
    /// </summary>
    internal static class AssemblyLoadUtility
    {
        /// <summary>
        /// Try load the assembly by using provided assembly by name and version if provided.
        /// </summary>
        /// <param name="assemblyPaths">The assembly paths.</param>
        /// <param name="assemblyName">The assembly name to load.It's not assembly full name.</param>
        /// <param name="assemblyVersion">The assembly version to load.</param>
        /// <returns>The assembly if able to load, otherwise null.</returns>
        internal static Assembly TryLoadByNameOrVersion(
            IList<string> assemblyPaths,
            string assemblyName,
            string assemblyVersion = null)
        {
            var assemblyPath = assemblyPaths.FirstOrDefault(path => path.EndsWith(assemblyName + ".dll"));

            if (!string.IsNullOrWhiteSpace(assemblyPath))
            {
                if(!string.IsNullOrWhiteSpace(assemblyVersion))
                {
                    try
                    {
                        var info = FileVersionInfo.GetVersionInfo(assemblyPath);
                        if (info.FileVersion.StartsWith(assemblyVersion))
                        {
                            return Assembly.LoadFrom(assemblyPath);
                        }
                    }
                    catch (Exception)
                    {
                        // Do nothing.
                    }
                }
                else
                {
                    return Assembly.LoadFrom(assemblyPath);
                }
                                
            }

            return null;
        }
    }
}