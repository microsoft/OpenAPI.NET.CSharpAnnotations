// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

#if NETCore

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.AssemblyLoader
{
    internal class CustomAssemblyLoadContext : AssemblyLoadContext
    {
        private readonly HashSet<string> _assembliesLoadedByAssemblyName = new HashSet<string>();
        private bool _isResolving = false;

        internal Dictionary<string, Assembly> Assemblies { get; } = new Dictionary<string, Assembly>();

        internal IList<string> AssemblyPaths { get; set; }

        public Assembly Resolve(AssemblyName args)
        {
            if (!_isResolving)
                return Load(args);
            else
                return null;
        }

        protected override Assembly Load(AssemblyName args)
        {
            if (Assemblies.ContainsKey(args.Name))
            {
                return Assemblies[args.Name];
            }

            var separatorIndex = args.Name.IndexOf(",", StringComparison.Ordinal);
            var assemblyName = separatorIndex > 0 ? args.Name.Substring(0, separatorIndex) : args.Name;

            var assembly = TryLoadExistingAssemblyName(args.FullName);

            if (assembly != null)
            {
                Assemblies[args.Name] = assembly;
                return assembly;
            }

            // Try to load assembly using version.
            var version = args.Version;
            if (version != null)
            {
                var assemblyByVersion = TryLoadByVersion(AssemblyPaths, assemblyName, version.Major + "." + version.Minor + "." + version.Build + ".");
                if (assemblyByVersion != null)
                {
                    Assemblies[args.Name] = assemblyByVersion;
                    return assemblyByVersion;
                }

                assemblyByVersion = TryLoadByVersion(AssemblyPaths, assemblyName, version.Major + "." + version.Minor + ".");
                if (assemblyByVersion != null)
                {
                    Assemblies[args.Name] = assemblyByVersion;
                    return assemblyByVersion;
                }

                assemblyByVersion = TryLoadByVersion(AssemblyPaths, assemblyName, version.Major + ".");
                if (assemblyByVersion != null)
                {
                    Assemblies[args.Name] = assemblyByVersion;
                    return assemblyByVersion;
                }
            }

            // Try to load assembly using full name, which includes version too.
            assembly = TryLoadByAssemblyName(args.FullName);
            if (assembly != null)
            {
                Assemblies[args.Name] = assembly;
                return assembly;
            }

            // Try to load assembly using short assembly name.
            assembly = AssemblyLoadUtility.TryLoadByName(AssemblyPaths, assemblyName);
            if (assembly != null)
            {
                Assemblies[args.Name] = assembly;
                return assembly;
            }

            Assemblies[args.Name] = TryLoadByAssemblyName(assemblyName);
            return Assemblies[args.Name];
        }

        private Assembly TryLoadExistingAssemblyName(string assemblyName)
        {
            try
            {
                _isResolving = true;
                return Default.LoadFromAssemblyName(new AssemblyName(assemblyName));
            }
            catch (Exception)
            {
                // Do nothing.
            }
            finally
            {
                _isResolving = false;
            }

            return null;
        }

        private Assembly TryLoadByAssemblyName(string assemblyName)
        {
            if (!_assembliesLoadedByAssemblyName.Contains(assemblyName))
            {
                try
                {
                    _assembliesLoadedByAssemblyName.Add(assemblyName);
                    return LoadFromAssemblyName(new AssemblyName(assemblyName));
                }
                catch (Exception)
                {
                    // Do nothing.
                }
            }

            return null;
        }

        private Assembly TryLoadByVersion(
            IList<string> assemblyPaths,
            string assemblyName,
            string assemblyVersion )
        {
            var assemblyPath = assemblyPaths.FirstOrDefault( path => path.EndsWith( assemblyName + ".dll" ) );

            if ( !string.IsNullOrWhiteSpace( assemblyPath ) )
            {
                try
                {
                    var info = FileVersionInfo.GetVersionInfo( assemblyPath );
                    if ( info.FileVersion.StartsWith( assemblyVersion ) )
                    {
                        return LoadFromAssemblyPath( assemblyPath );
                    }
                }
                catch ( Exception )
                {
                    // Do nothing.
                }
            }

            return null;
        }
    }
}

#endif