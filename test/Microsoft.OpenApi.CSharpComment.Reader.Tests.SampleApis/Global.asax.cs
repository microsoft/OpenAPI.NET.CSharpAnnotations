// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.OpenApi.CSharpComment.Reader.Tests.SampleApis
{
    /// <summary>
    /// Web API Application.
    /// </summary>
    public class WebApiApplication : HttpApplication
    {
        /// <summary>
        /// Start application.
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}