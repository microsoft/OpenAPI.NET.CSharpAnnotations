// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.Contracts;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.SampleApis.Controllers
{
    /// <summary>
    /// Defines V4 operations.
    /// </summary>
    public class SampleControllerV4 : ApiController
    {
        /// <summary>
        /// Sample get 1
        /// </summary>
        /// <group>Sample V4</group>
        /// <verb>GET</verb>
        /// <url>http://localhost:9000/V4/samples/</url>
        /// <param name="sampleHeaderParam1" cref="float" in="header">Header param 1</param>
        /// <param name="sampleHeaderParam2" cref="string" in="header">Header param 2</param>
        /// <param name="sampleHeaderParam3" cref="string" in="header">Header param 3</param>
        /// <response code="200">
        /// <see cref="List{T}"/>
        /// where T is <see cref="ISampleObject4{T1,T2}"/>
        /// where T1 is <see cref="SampleObject1"/>
        /// where T2 is <see cref="SampleObject4"/>
        /// List of sample objects
        /// </response>
        /// <response code="400"><see cref="string"/>Bad request</response>
        [HttpGet]
        [Route("/V4/samples")]
        public Task<List<ISampleObject4<SampleObject1, SampleObject4>>> SampleGet1()
        {
            throw new NotSupportedException();
        }
    }
}