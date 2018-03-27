// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Web.Http;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.SampleApis
{
    /// <summary>
    /// Web config.
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Register the configuration, services, and routes.
        /// </summary>
        /// <param name="config">HTTP Configuration</param>
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();
        }
    }
}