// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.OpenApi.CSharpComment.Reader.Models.KnownStrings;

namespace Microsoft.OpenApi.CSharpComment.Reader.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="XElement"/>.
    /// </summary>
    public static class XElementExtensions
    {
        /// <summary>
        /// Get the text inside the element with the following modifications:
        /// 1. The see/seealso cref value and th paramref/typeparamref name value extracted out of the inner XML elements.
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
                if (child.NodeType == XmlNodeType.Text)
                {
                    description.Append(child);
                }
                else if (child.NodeType == XmlNodeType.Element)
                {
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
                }
            }

            return description.ToString().Trim().RemoveBlankLines();
        }
    }
}