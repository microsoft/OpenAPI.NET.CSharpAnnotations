// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.OpenApiSpecification.Generation.Models.KnownStrings
{
    internal class KnownXmlStrings
    {
        public const string Annotation = "annotation";
        public const string Body = "body";
        public const string Code = "code";
        public const string Cref = "cref";
        public const string Description = "description";
        public const string Group = "group";
        public const string Header = "header";
        public const string In = "in";
        public const string Name = "name";
        public const string Option = "option";
        public const string Options = "options";
        public const string Param = "param";
        public const string Path = "path";
        public const string PathParam = "pathParam";
        public const string Query = "query";
        public const string QueryParam = "queryParam";
        public const string Remarks = "remarks";
        public const string Required = "required";
        public const string Response = "response";
        public const string See = "see";
        public const string Summary = "summary";
        public const string T = "T";
        public const string Tag = "tag";
        public const string Type = "type";
        public const string Url = "url";
        public const string Variant = "variant";
        public const string Verb = "verb";

        public static string[] AllowedInValues => new[] {Header, Path, Query, Body};

        // "body" in attribute is translated to a requestBody instead of a parameter.
        public static string[] InValuesTranslatableToParameter => new[] {Header, Path, Query};
    }
}