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
    public class SampleV4Controller : ApiController
    {
        /// <summary>
        /// Sample get 1
        /// </summary>
        /// <group>Sample V4</group>
        /// <verb>GET</verb>
        /// <url>https://myapi.sample.com/V4/samples/</url>
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
        [Route("V4/samples")]
        public Task<List<ISampleObject4<SampleObject1, SampleObject4>>> SampleGet1()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Sample get 1
        /// </summary>
        /// <group>Sample V4</group>
        /// <verb>POST</verb>
        /// <url>https://myapi.sample.com/V4/samples/{id}</url>
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
        /// <param name="sampleObject1" in="body"><see cref="SampleObject1"/>Request Body
        ///     <example><value><see cref="Examples.SampleObject1Example"/></value></example>
        /// </param>
        /// <response code="200"><see cref="SampleObject4"/>
        ///     <example><value><see cref="Examples.SampleObject4Example"/></value></example>
        ///     <example><value><see cref="Examples.SampleObject4Example"/></value></example>
        /// Sample object
        /// </response>
        /// <response code="400"><see cref="string"/>Bad request</response>
        [HttpPost]
        [Route("V4/samples/{id}")]
        public Task<SampleObject4> SamplePost1([FromBody] SampleObject1 sampleObject1, string id, bool queryBool)
        {
            throw new NotSupportedException();
        }
    }
}