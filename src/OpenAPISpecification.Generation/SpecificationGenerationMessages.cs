// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.OpenApiSpecification.Generation
{
    /// <summary>
    /// Holds the specification generation messages.
    /// </summary>
    public static class SpecificationGenerationMessages
    {
        public const string InvalidHttpMethod = "The documented http method: {0} is not a valid Http Method.";

        public const string InvalidUrl = "The documented url: {0} is not a valid URI.";

        public const string SuccessfulPathGeneration = "The specification for path has been generated successfully.";

        public const string NoOperationElementFoundToParse =
            "No valid operation elements have been found in the annotation xml.";

        public const string UnexpectedError = "Unexpected error occurred during generation: {0}";

        public const string UndocumentedPathParameter = "Path paramater: {0} is undocumented in {1}";

        public const string UnorderedGenericType =
            "The request or response type contains a generic type that is unordered. " +
            "The correct way to document a generic type is as follows: <![CDATA[ <responseType>" +
            "<see cref=\"Response{T}\"/> where T is <see cref=\"GenericType\"/></responseType> ]]>";

        public const string UndocumentedGenericType =
            "The request or response type contains a generic type that is undocumented. " +
            "The correct way to document a generic type is as follows: <![CDATA[ <responseType>" +
            "<see cref=\"Response{T}\"/> where T is <see cref=\"GenericType\"/></responseType> ]]>";
    }
}