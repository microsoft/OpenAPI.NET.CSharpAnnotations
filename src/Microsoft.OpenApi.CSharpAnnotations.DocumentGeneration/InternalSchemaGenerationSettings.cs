// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration
{
#if NETFRAMEWORK
    internal class InternalSchemaGenerationSettings : MarshalByRefObject
    {
#else
    internal class InternalSchemaGenerationSettings
    {
#endif
        public string PropertyNameResolverName { get; set; }
    }
}