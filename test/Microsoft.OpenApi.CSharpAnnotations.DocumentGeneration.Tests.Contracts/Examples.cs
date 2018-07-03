// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.Contracts
{
    /// <summary>
    /// Holds examples for contracts.
    /// </summary>
    public static class Examples
    {
        public const string SampleObject1Example = @"{
  ""samplePropertyBool"": true,
  ""samplePropertyStringInt"": 100,
  ""samplePropertyString1"": ""SampleObject SamplePropertyString1"",
  ""samplePropertyString2"": ""SampleObject SamplePropertyString2"",
  ""samplePropertyString3"": ""SampleObject SamplePropertyString3"",
  ""samplePropertyString4"": ""SampleObject SamplePropertyString4"",
  ""samplePropertyEnum"": 100
}";

        public const string SampleObject2Example = @"{
  ""samplePropertyEnum"": 100,
  ""samplePropertyInt"": ""SampleObject2 SamplePropertyInt"",
  ""samplePropertyObjectDictionary"": {
    ""SampleKey1"": {
      ""samplePropertyBool"": true,
      ""samplePropertyStringInt"": 100,
      ""samplePropertyString1"": ""SampleObject SamplePropertyString1"",
      ""samplePropertyString2"": ""SampleObject SamplePropertyString2"",
      ""samplePropertyString3"": ""SampleObject SamplePropertyString3"",
      ""samplePropertyString4"": ""SampleObject SamplePropertyString4"",
      ""samplePropertyEnum"": 100
    }
  },
  ""samplePropertyObjectList"": [
    {
      ""samplePropertyBool"": true,
      ""samplePropertyStringInt"": 100,
      ""samplePropertyString1"": ""SampleObject SamplePropertyString1"",
      ""samplePropertyString2"": ""SampleObject SamplePropertyString2"",
      ""samplePropertyString3"": ""SampleObject SamplePropertyString3"",
      ""samplePropertyString4"": ""SampleObject SamplePropertyString4"",
      ""samplePropertyEnum"": 100
    }
  ],
  ""samplePropertyString1"": ""SampleObject2 SamplePropertyString1""
}"";

        public const string SampleObject3Example = @""{
  ""samplePropertyObject"": {
    ""samplePropertyEnum"": 100,
    ""samplePropertyInt"": ""SampleObject2 SamplePropertyInt"",
    ""samplePropertyObjectDictionary"": {
      ""SampleKey1"": {
        ""samplePropertyBool"": true,
        ""samplePropertyStringInt"": 100,
        ""samplePropertyString1"": ""SampleObject SamplePropertyString1"",
        ""samplePropertyString2"": ""SampleObject SamplePropertyString2"",
        ""samplePropertyString3"": ""SampleObject SamplePropertyString3"",
        ""samplePropertyString4"": ""SampleObject SamplePropertyString4"",
        ""samplePropertyEnum"": 100
      }
    },
    ""samplePropertyObjectList"": [
      {
        ""samplePropertyBool"": true,
        ""samplePropertyStringInt"": 100,
        ""samplePropertyString1"": ""SampleObject SamplePropertyString1"",
        ""samplePropertyString2"": ""SampleObject SamplePropertyString2"",
        ""samplePropertyString3"": ""SampleObject SamplePropertyString3"",
        ""samplePropertyString4"": ""SampleObject SamplePropertyString4"",
        ""samplePropertyEnum"": 100
      }
    ],
    ""samplePropertyString1"": ""SampleObject2 SamplePropertyString1""
  },
  ""samplePropertyObjectList"": [
    {
      ""samplePropertyBool"": true,
      ""samplePropertyStringInt"": 100,
      ""samplePropertyString1"": ""SampleObject SamplePropertyString1"",
      ""samplePropertyString2"": ""SampleObject SamplePropertyString2"",
      ""samplePropertyString3"": ""SampleObject SamplePropertyString3"",
      ""samplePropertyString4"": ""SampleObject SamplePropertyString4"",
      ""samplePropertyEnum"": 100
    }
  ]
}";

        public const string SampleObject4Example = @"{
  ""samplePropertyObject"": {
    ""samplePropertyEnum"": 100,
    ""samplePropertyInt"": ""SampleObject2 SamplePropertyInt"",
    ""samplePropertyObjectDictionary"": {
      ""SampleKey1"": {
        ""samplePropertyBool"": true,
        ""samplePropertyStringInt"": 100,
        ""samplePropertyString1"": ""SampleObject SamplePropertyString1"",
        ""samplePropertyString2"": ""SampleObject SamplePropertyString2"",
        ""samplePropertyString3"": ""SampleObject SamplePropertyString3"",
        ""samplePropertyString4"": ""SampleObject SamplePropertyString4"",
        ""samplePropertyEnum"": 100
      }
    },
    ""samplePropertyObjectList"": [
      {
        ""samplePropertyBool"": true,
        ""samplePropertyStringInt"": 100,
        ""samplePropertyString1"": ""SampleObject SamplePropertyString1"",
        ""samplePropertyString2"": ""SampleObject SamplePropertyString2"",
        ""samplePropertyString3"": ""SampleObject SamplePropertyString3"",
        ""samplePropertyString4"": ""SampleObject SamplePropertyString4"",
        ""samplePropertyEnum"": 100
      }
    ],
    ""samplePropertyString1"": ""SampleObject2 SamplePropertyString1""
  },
  ""Sample3PropertyObject"": {
    ""samplePropertyObject"": {
      ""samplePropertyEnum"": 100,
      ""samplePropertyInt"": ""SampleObject2 SamplePropertyInt"",
      ""samplePropertyObjectDictionary"": {
        ""SampleKey1"": {
          ""samplePropertyBool"": true,
          ""samplePropertyStringInt"": 100,
          ""samplePropertyString1"": ""SampleObject SamplePropertyString1"",
          ""samplePropertyString2"": ""SampleObject SamplePropertyString2"",
          ""samplePropertyString3"": ""SampleObject SamplePropertyString3"",
          ""samplePropertyString4"": ""SampleObject SamplePropertyString4"",
          ""samplePropertyEnum"": 100
        }
      },
      ""samplePropertyObjectList"": [
        {
          ""samplePropertyBool"": true,
          ""samplePropertyStringInt"": 100,
          ""samplePropertyString1"": ""SampleObject SamplePropertyString1"",
          ""samplePropertyString2"": ""SampleObject SamplePropertyString2"",
          ""samplePropertyString3"": ""SampleObject SamplePropertyString3"",
          ""samplePropertyString4"": ""SampleObject SamplePropertyString4"",
          ""samplePropertyEnum"": 100
        }
      ],
      ""samplePropertyString1"": ""SampleObject2 SamplePropertyString1""
    },
    ""samplePropertyObjectList"": [
      {
        ""samplePropertyBool"": true,
        ""samplePropertyStringInt"": 100,
        ""samplePropertyString1"": ""SampleObject SamplePropertyString1"",
        ""samplePropertyString2"": ""SampleObject SamplePropertyString2"",
        ""samplePropertyString3"": ""SampleObject SamplePropertyString3"",
        ""samplePropertyString4"": ""SampleObject SamplePropertyString4"",
        ""samplePropertyEnum"": 100
      }
    ]
  },
  ""SamplePropertyObjectList"": [
    {
      ""samplePropertyBool"": true,
      ""samplePropertyStringInt"": 100,
      ""samplePropertyString1"": ""SampleObject SamplePropertyString1"",
      ""samplePropertyString2"": ""SampleObject SamplePropertyString2"",
      ""samplePropertyString3"": ""SampleObject SamplePropertyString3"",
      ""samplePropertyString4"": ""SampleObject SamplePropertyString4"",
      ""samplePropertyEnum"": 100
    }
  ]
}";
    }
}