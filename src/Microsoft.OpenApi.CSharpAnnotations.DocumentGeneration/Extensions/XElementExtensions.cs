// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models.KnownStrings;

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
                        description.Append(child.ToString().Replace("<![CDATA[","").Replace("]]>",""));
                        break;

                    case XmlNodeType.Element:
                        var childElement = (XElement)child;
                        if (childElement.Name == KnownXmlStrings.Para)
                        {
                            description.Append(GetDescriptionText(childElement));
                        }
                        else if (childElement.Name == KnownXmlStrings.See)
                        {
                            description.Append(childElement.Attribute(KnownXmlStrings.Cref)?.Value);
                        }
                        else if (childElement.Name == KnownXmlStrings.Seealso)
                        {
                            description.Append(childElement.Attribute(KnownXmlStrings.Cref)?.Value);
                        }
                        else if (childElement.Name == KnownXmlStrings.Paramref)
                        {
                            description.Append(childElement.Attribute(KnownXmlStrings.Name)?.Value);
                        }
                        else if (childElement.Name == KnownXmlStrings.Typeparamref)
                        {
                            description.Append(childElement.Attribute(KnownXmlStrings.Name)?.Value);
                        }
                        break;
                }
            }

            return description.ToString().Trim().RemoveBlankLines();
        }
    }
}