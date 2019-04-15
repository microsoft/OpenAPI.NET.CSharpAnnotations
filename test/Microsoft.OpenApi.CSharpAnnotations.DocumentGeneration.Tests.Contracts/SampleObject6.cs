// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.Contracts
{
    /// <summary>
    /// The sample object 6.
    /// </summary>
    /// 
    [DataContract]
    public class SampleObject6
    {
        /// <summary>
        /// The sample string property.
        /// </summary>
        [DataMember(Name = "samplePropertyObject")]
        public string SampleProperty { get; set; }
    }
}