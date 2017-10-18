// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Web.Mvc;
using System.Web.Routing;

namespace OpenApiSpecification.UnitTestSamples.DotNetFrameworkController
{
    /// <summary>
    /// Responsible for route configuration
    /// </summary>
    public sealed class RouteConfig
    {
        private RouteConfig()
        {
        }

        /// <summary>
        /// Registers routes
        /// </summary>
        /// <param name="routes">Route collection</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
        }
    }
}