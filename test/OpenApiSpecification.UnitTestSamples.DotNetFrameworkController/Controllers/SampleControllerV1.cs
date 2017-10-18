// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Threading.Tasks;
using System.Web.Http;
using OpenApiSpecification.UnitTestSamples.DotNetFrameworkController.Contracts;

namespace OpenApiSpecification.UnitTestSamples.DotNetFrameworkController.Controllers
{
    /// <summary>
    /// Define V1 operations.
    /// </summary>
    public class SampleControllerV1 : ApiController
    {
        /// <summary>
        /// Sample Get 1
        /// </summary>
        /// <group>Sample V1</group>
        /// <verb>GET</verb>
        /// <url>http://localhost:9000/V1/samples/{id}?queryBool={queryBool}</url>
        /// <param name="sampleHeaderParam1" cref="float" in="header">Header param 1</param>
        /// <param name="sampleHeaderParam2" cref="string" in="header">Header param 2</param>
        /// <param name="sampleHeaderParam3" cref="string" in="header">Header param 3</param>
        /// <param name="id" cref="string" in="path">The object id</param>
        /// <param name="queryBool" required="true" cref="bool" in="query">Sample query boolean</param>
        /// <response code="200"><see cref="SampleObject1"/>Sample object retrieved</response>
        /// <response code="400"><see cref="string"/>Bad request</response>
        /// <swagger>Group1</swagger>
        /// <swagger>Group2</swagger>
        /// <returns>The sample object 1</returns>
        [HttpPost]
        [Route("/V1/samples/{id}?queryBool={queryBool}")]
        public Task<SampleObject1> SampleGet1(string id, bool queryBool)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Sample Get 2
        /// </summary>
        /// <group>Sample V1</group>
        /// <verb>GET</verb>
        /// <url>http://localhost:9000/V1/samples</url>
        /// <param name="sampleHeaderParam1" cref="float" in="header">Header param 1</param>
        /// <param name="sampleHeaderParam2" cref="string" in="header">Header param 2</param>
        /// <param name="sampleHeaderParam3" cref="string" in="header">Header param 3</param>
        /// <response code="200"><see cref="SampleObject3"/>Paged Entity contract</response>
        /// <response code="400"><see cref="string"/>Bad request</response>
        /// <returns>The sample object 3</returns>
        [HttpPost]
        [Route("/V1/samples")]
        public Task<SampleObject3> SampleGet2()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Sample Post
        /// </summary>
        /// <group>Sample V1</group>
        /// <verb>POST</verb>
        /// <url>http://localhost:9000/V1/samples</url>
        /// <param name="sampleHeaderParam1" cref="float" in="header">Header param 1</param>
        /// <param name="sampleHeaderParam2" cref="string" in="header">Header param 2</param>
        /// <param name="sampleHeaderParam3" cref="string" in="header">Header param 3</param>
        /// <param name="sampleObject" in="body"><see cref="SampleObject3"/>Sample object</param>
        /// <response code="200"><see cref="SampleObject3"/>Sample object posted</response>
        /// <response code="400"><see cref="string"/>Bad request</response>
        [HttpPost]
        [Route("/V1/samples")]
        public Task<SampleObject1> SamplePost([FromBody] SampleObject3 sampleObject)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Sample put
        /// </summary>
        /// <group>Sample V1</group>
        /// <verb>PUT</verb>
        /// <url>http://localhost:9000/V1/samples/{id}</url>
        /// <param name="sampleHeaderParam1" cref="float" in="header">Header param 1</param>
        /// <param name="sampleHeaderParam2" cref="string" in="header">Header param 2</param>
        /// <param name="sampleHeaderParam3" cref="string" in="header">Header param 3</param>
        /// <param name="id" cref="string" in="path">The object id</param>
        /// <param name="sampleObject" in="body"><see cref="SampleObject1"/>Sample object</param>
        /// <response code="200"><see cref="SampleObject1"/>The sample object updated</response>
        /// <response code="400"><see cref="string"/>Bad request</response>
        /// <returns>The sample object 1</returns>
        [HttpPut]
        [Route("/V1/samples/{id}")]
        public Task<SampleObject1> SamplePut(string id, [FromBody] SampleObject1 sampleObject)
        {
            throw new NotSupportedException();
        }
    }
}