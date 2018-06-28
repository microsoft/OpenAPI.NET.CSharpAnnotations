// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Exceptions;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models.KnownStrings;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="XElement"/>.
    /// </summary>
    public static class XElementExtensions
    {
        /// <summary>
        /// Get the text inside the element with the following modifications:
        /// 1. The see/seealso cref value and the paramref/typeparamref name value extracted out of the inner XML elements.
        /// 2. The para tag is ignored.
        /// 3. Any blank lines are removed.
        /// 4. Beginning and trailing whitespaces are trimmed.
        /// </summary>
        public static string GetDescriptionText(this XElement element)
        {
            var description = new StringBuilder();

            var children = element.Nodes();

            foreach (var child in children)
            {
                switch (child.NodeType)
                {
                    case XmlNodeType.Text:
                        description.Append(child);
                        break;

                    case XmlNodeType.CDATA:
                        description.Append(child.ToString()
                            .Replace("<![CDATA[", string.Empty).Replace("]]>", string.Empty));
                        break;

                    case XmlNodeType.Element:
                        var childElement = (XElement) child;

                        switch (childElement.Name.ToString())
                        {
                            case KnownXmlStrings.Para:
                                description.Append(GetDescriptionText(childElement));
                                break;

                            case KnownXmlStrings.See:
                                description.Append(childElement.Attribute(KnownXmlStrings.Cref)?.Value);
                                break;

                            case KnownXmlStrings.Seealso:
                                description.Append(childElement.Attribute(KnownXmlStrings.Cref)?.Value);
                                break;

                            case KnownXmlStrings.Paramref:
                                description.Append(childElement.Attribute(KnownXmlStrings.Name)?.Value);
                                break;

                            case KnownXmlStrings.Typeparamref:
                                description.Append(childElement.Attribute(KnownXmlStrings.Name)?.Value);
                                break;
                        }
                        break;
                }
            }

            return description.ToString().Trim().RemoveBlankLines();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="typeFetcher"></param>
        /// <returns></returns>
        public static OpenApiExample ToOpenApiExample(this XElement element, TypeFetcher typeFetcher)
        {
            if(typeFetcher == null)
            {
                throw new ArgumentNullException( nameof( typeFetcher ) );
            }

            var openApiExample = new OpenApiExample();

            var summaryElement = element
                .Elements()
                .Where( p => p.Name == KnownXmlStrings.Summary )
                .FirstOrDefault();

            if ( summaryElement != null )
            {
                openApiExample.Summary = summaryElement.Value;
            }

            var valueElement = element.Elements().Where( p => p.Name == KnownXmlStrings.Value ).FirstOrDefault();
            var urlElement = element.Elements().Where( p => p.Name == KnownXmlStrings.Url ).FirstOrDefault();

            if ( valueElement != null && urlElement != null )
            {
                throw new InvalidExampleException( SpecificationGenerationMessages.ProvideEitherValueOrUrlTag );
            }

            IOpenApiAny exampleValue;

            if ( valueElement != null )
            {
                var seeNodes = element.Descendants( KnownXmlStrings.See );

                var crefValue = seeNodes
                    .Select( node => node.Attribute( KnownXmlStrings.Cref )?.Value )
                    .Where( crefVal => crefVal != null ).FirstOrDefault();

                if ( crefValue != null )
                {
                    var typeName = crefValue.ExtractTypeNameFromFieldCref();
                    var type = typeFetcher.LoadTypeFromCrefValues( new List<string>() { typeName } );
                    var fieldName = crefValue.ExtractFieldNameFromCref();

                    FieldInfo[] fields = type.GetFields( BindingFlags.Public
                        | BindingFlags.Static );
                    var field = fields.Where( f => f.Name == fieldName ).FirstOrDefault();

                    if(field==null)
                    {

                    }

                    var value = field.GetValue( null );

                    exampleValue = new OpenApiStringReader().ReadFragment<IOpenApiAny>(
                        value.ToString(),
                        OpenApiSpecVersion.OpenApi3_0,
                        out OpenApiDiagnostic openApiDiagnostic );

                    openApiExample.Value = exampleValue;
                }

                if ( string.IsNullOrWhiteSpace( valueElement.Value ) )
                {
                    throw new InvalidExampleException( SpecificationGenerationMessages.ProvideValueForExample );
                }

                exampleValue = new OpenApiStringReader()
                    .ReadFragment<IOpenApiAny>( valueElement.Value, OpenApiSpecVersion.OpenApi3_0, out OpenApiDiagnostic _ );

                openApiExample.Value = exampleValue;
            }

            if ( urlElement != null )
            {
                openApiExample.ExternalValue = urlElement.Value;
            }

            return openApiExample;
        }
    }
}