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
    }
}