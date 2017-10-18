// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using OpenApiSpecification.UnitTestSamples.DotNetFrameworkController.Contracts;

namespace OpenApiSpecification.UnitTestSamples.DotNetFrameworkController.Controllers
{
    /// <summary>
    /// Defines V2 operations.
    /// </summary>
    public class SampleControllerV2 : ApiController
    {
        /// <summary>
        /// Sample delete
        /// </summary>
        /// <group>Sample V2</group>
        /// <verb>DELETE</verb>
        /// <url>http://localhost:9000/V2/samples/{id}</url>
        /// <param name="sampleHeaderParam1" cref="float" in="header">Header param 1</param>
        /// <param name="sampleHeaderParam2" cref="string" in="header">Header param 2</param>
        /// <param name="sampleHeaderParam3" cref="string" in="header">Header param 3</param>
        /// <param name="id" cref="string" in="path">The object id</param>
        /// <response code="200"><see cref="SampleObject1"/>Sample object deleted</response>
        /// <response code="400"><see cref="string"/>Bad request</response>
        [HttpDelete]
        [Route("/V2/samples/{id}")]
        public Task<SampleObject1> DeleteEntity(string id)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Sample get 1
        /// </summary>
        /// <group>Sample V2</group>
        /// <verb>GET</verb>
        /// <url>http://localhost:9000/V2/samples/</url>
        /// <param name="sampleHeaderParam1" cref="float" in="header">Header param 1</param>
        /// <param name="sampleHeaderParam2" cref="string" in="header">Header param 2</param>
        /// <param name="sampleHeaderParam3" cref="string" in="header">Header param 3</param>
        /// <response code="200"><see cref="List{T}"/>where T is <see cref="SampleObject2"/>List of sample objects</response>
        /// <response code="400"><see cref="string"/>Bad request</response>
        [HttpGet]
        [Route("/V2/samples")]
        public Task<List<SampleObject2>> SampleGet1()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Sample get 2
        /// </summary>
        /// <group>Sample V2</group>
        /// <verb>GET</verb>
        /// <url>http://localhost:9000/V2/samples/{id}?queryString={queryString}</url>
        /// <param name="sampleHeaderParam1" cref="float" in="header">Header param 1</param>
        /// <param name="sampleHeaderParam2" cref="string" in="header">Header param 2</param>
        /// <param name="sampleHeaderParam3" cref="string" in="header">Header param 3</param>
        /// <param name="id" cref="string" in="path">The object id</param>
        /// <param name="queryString" cref="string" in="query">The sample query string</param>
        /// <response code="200"><see cref="SampleObject2"/>Sample object retrieved</response>
        /// <response code="400"><see cref="string"/>Bad request</response>
        [HttpGet]
        [Route("/V2/samples/{id}?queryString={queryString}")]
        public Task<SampleObject2> SampleGet2(string id, string queryString)
        {
            throw new NotSupportedException();
        }
    }
}