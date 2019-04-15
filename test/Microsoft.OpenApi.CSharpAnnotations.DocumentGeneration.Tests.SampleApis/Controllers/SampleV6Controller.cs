// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.Contracts;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.SampleApis.Controllers
{
    /// <summary>
    /// Sample v6 controller.
    /// </summary>
    public class SampleV6Controller : ApiController
    {
        /// <summary>
        /// Sample get 1
        /// </summary>
        /// <group>Sample V6</group>
        /// <verb>GET</verb>
        /// <url>https://myapi.sample.com/V6/samples/</url>
        /// <param name="sampleHeaderParam1" cref="float" in="header">Header param 1</param>
        /// <param name="sampleHeaderParam2" cref="string" in="header">Header param 2</param>
        /// <param name="sampleHeaderParam3" cref="string" in="header">Header param 3</param>
        /// <response code="200">
        /// <see cref="SampleObject6"/>
        /// sample object
        /// </response>
        /// <response code="400"><see cref="string"/>Bad request</response>
        [HttpGet]
        [Route("V6/samples")]
        public async Task<SampleObject6> SampleGet1()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sample get 1
        /// </summary>
        /// <group>Sample V6</group>
        /// <verb>POST</verb>
        /// <url>https://myapi.sample.com/V6/samples/{id}</url>
        /// <param name="id" cref="string" in="path">The object id
        ///     <example name="Example one"><value>id1</value></example>
        ///     <example name="Example two"><value>id2</value></example>
        /// </param>
        /// <param name="queryBool" required="true" cref="bool" in="query">Sample query boolean
        /// <example><value>true</value></example>
        /// <example><value>false</value></example>
        /// </param>
        /// <param name="sampleHeaderParam1" cref="float" in="header">
        ///     <example><value>Sample header 1</value></example>
        /// Header param 1
        /// </param>
        /// <param name="sampleHeaderParam2" cref="string" in="header">Header param 2</param>
        /// <param name="sampleHeaderParam3" cref="string" in="header">Header param 3</param>
        /// <param name="sampleObject6" in="body"><see cref="SampleObject6"/>Request Body
        /// </param>
        /// <response code="200"><see cref="SampleObject6"/>
        /// Sample object
        /// </response>
        /// <response code="400"><see cref="string"/>Bad request</response>
        [HttpPost]
        [Route("V6/samples/{id}")]
        public async Task<SampleObject6> SamplePost1([FromBody] SampleObject6 sampleObject6, string id, bool queryBool)
        {
            throw new NotImplementedException();
        }
    }
}