// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.AssemblyLoader
{

#if NETFRAMEWORK
    /// <summary>
    /// Creates a separate app domain.
    /// </summary>
    /// <typeparam name="T">The type for which instance needs to be created in new app domain.</typeparam>
    internal sealed class AppDomainCreator<T> : MarshalByRefObject, IDisposable
    {
        /// <summary>
        /// Initializes new instance of <see cref="AppDomainCreator{T}"/>.
        /// </summary>
        public AppDomainCreator()
        {
            var setupInformation = AppDomain.CurrentDomain.SetupInformation;

            // Specify that the new domain should make a copy to the shadow directory, preventing
            // the locking of the assemblies.
            setupInformation.ShadowCopyFiles = "true";

            // The privateBinPath is a ; separated list of paths located in the base path of the 
            // application where the CLR will attempt to locate assemblies during the load process.
            // Here we add the location where we will copy dlls.
            setupInformation.PrivateBinPath += ";DefaultGenerationBin";

            // Setup the domain.
            Domain = AppDomain.CreateDomain(
                "AppDomain" + Guid.NewGuid(),
                AppDomain.CurrentDomain.Evidence,
                setupInformation );

            var type = typeof( T );

            Object = (T) Domain.CreateInstanceAndUnwrap( type.Assembly.FullName, type.FullName );
        }

        /// <summary>
        /// Gets the <see cref="AppDomain"/>.
        /// </summary>
        public AppDomain Domain { get; private set; }

        /// <summary>
        /// Gets the instance of the object created in the new app domain.
        /// </summary>
        public T Object { get; }

        /// <summary>
        /// Unloads the app domain.
        /// </summary>
        public void Dispose()
        {
            if ( Domain != null )
            {
                AppDomain.Unload( Domain );
                Domain = null;
            }
        }
    }
#endif
}