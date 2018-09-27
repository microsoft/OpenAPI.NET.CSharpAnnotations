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
    /// <security type="http" name="http-bearer">
    ///     <description>Test security</description>
    ///     <scheme>bearer</scheme>
    ///     <bearerFormat>JWT</bearerFormat>
    /// </security>
    /// <security type="oauth2" name="oauth">
    ///     <description>Test security</description>
    ///     <flow type="implicit">
    ///         <authorizationUrl>https://example.com/api/oauth/dialog</authorizationUrl>
    ///         <refreshUrl>https://example.com/api/oauth/dialog</refreshUrl>
    ///         <scope name="scope1">
    ///             <description></description>
    ///         </scope>
    ///     </flow>
    /// </security>
    /// <security type="openIdConnect" name="openIdConnect">
    ///     <description>Test security</description>
    ///     <openIdConnectUrl>https://example.com/api/oauth/dialog</openIdConnectUrl>
    ///     <scope name="scope1"></scope>
    ///     <scope name="scope2"></scope>
    /// </security>
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