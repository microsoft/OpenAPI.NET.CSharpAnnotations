// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.OpenApi.CSharpComment.Reader
{
    /// <summary>
    /// Holds the specification generation messages.
    /// </summary>
    public static class SpecificationGenerationMessages
    {
        public const string ConflictingDocumentVariantAttributes =
            "The document variant (Categorizer: {0}, Title: {1}) have conflicting attributes: " +
            "{2} | {3}";

        public const string ConflictingPathAndQueryParameters = "The parameter {0} is both present in path and query of the url {1}";

        public const string DuplicateOperation = "There are duplicates for this operation: {0} {1}";

        public const string InvalidRequestBody = "The documented request body {0} is not valid.";

        public const string InvalidResponse = "The documented response {0} is not valid.";

        public const string InvalidHttpMethod = "The documented verb {0} is not a valid http Method.";

        public const string InvalidUrl = "The documented url ({0}) is not a valid uri due to this error: {1}.";

        public const string AddingSchemaReferenceFailed = "Adding schema {0} to the schema reference registry failed with the following error: {1}.";
        
        public const string NullUrl = "Url is null";

        public const string MalformattedUrl = "Url is malformatted and cannot be decoded properly.";

        public const string MissingInAttribute = "In attribute is missing from parameter(s) {0}";

        public const string MoreThanOneVariantNameNotAllowed =
            "More than one document variant is not allowed. Variant used for categorization is: {0}";

        public const string NoOperationElementFoundToParse =
            "No valid operation elements have been found in the annotation xml.";

        public const string OperationMethodNotParsedGivenUrlIsInvalid =
            "Operation method has not been parsed given that the url is invalid.";

        public const string SuccessfulPathGeneration = "The specification for path has been generated successfully.";

        public const string UndocumentedGenericType =
            "The request or response type contains a generic type that is undocumented. " +
            "The correct way to document a generic type is as follows: <responseType>" +
            "<see cref=\"Response{T}\"/> where T is <see cref=\"GenericType\"/></responseType>";

        public const string UndocumentedPathParameter = "Path paramater: {0} is undocumented in {1}";

        public const string UnexpectedError = "Unexpected error occurred during generation: {0}";
        
        public const string UnorderedGenericType =
            "The request or response type contains a generic type that is unordered. " +
            "The correct way to document a generic type is as follows: <![CDATA[ <responseType>" +
            "<see cref=\"Response{T}\"/> where T is <see cref=\"GenericType\"/></responseType> ]]>";
    }
}