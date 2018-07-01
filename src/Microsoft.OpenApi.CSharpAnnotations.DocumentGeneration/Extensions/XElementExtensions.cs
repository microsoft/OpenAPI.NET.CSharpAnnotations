// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models.KnownStrings;
using System.Text;
using System.Xml;
using System.Xml.Linq;

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
                        var childElement = (XElement)child;

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
    }
}