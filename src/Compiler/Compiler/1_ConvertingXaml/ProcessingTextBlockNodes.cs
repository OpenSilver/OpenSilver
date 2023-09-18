

/*===================================================================================
* 
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*      
*   This file is part of both the OpenSilver Compiler (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Compiler (http://cshtml5.com), which is dual-licensed (MIT + commercial).
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace OpenSilver.Compiler
{
    internal static class ProcessingTextBlockNodes
    {
        //------------------------------------------------------------
        // This class will process the "TextBlock" and "Span" nodes
        // in order to add the implicit "<Run>" elements. For example,
        // <TextBlock>This <Run>is</Run> a test</TextBlock> becomes:
        // <TextBlock><Run>This </Run><Run>is</Run><Run> a test</Run></TextBlock>
        // The goal is that any direct text becomes wrapped inside a <Run>.
        // SPECIAL CASE: if there is only one text inside the TextBlock,
        // we do not surround it with a <Run> for performance optimization.
        //------------------------------------------------------------

        public static void Process(XDocument doc, AssembliesInspector reflectionOnSeparateAppDomain)
        {
            TraverseNextNode(doc.Root, 1, null, reflectionOnSeparateAppDomain);
        }

        static void TraverseNextNode(XNode currentNode, int siblingNodesCount, XElement parentElement, AssembliesInspector reflectionOnSeparateAppDomain)
        {
            if (currentNode is XText)
            {
                if ((GeneratingCode.IsTextBlock(parentElement) && siblingNodesCount > 1) // Note: if there is only one XNode inside a TextBlock, we do not surround with a <Run> for runtime performance optimization.
                    || GeneratingCode.IsSpan(parentElement)
                    || GeneratingCode.IsItalic(parentElement)
                    || GeneratingCode.IsUnderline(parentElement)
                    || GeneratingCode.IsBold(parentElement)
                    || GeneratingCode.IsHyperlink(parentElement)
                    || GeneratingCode.IsParagraph(parentElement))
                {
                    // Surround with a <Run>:
                    XElement contentWrapper = new XElement(XName.Get("Run", GeneratingCode.DefaultXamlNamespace)); //todo: read the "ContentWrapperAttribute" of the collection (cf. InlineCollection.cs) instead of hard-coding this.
                    XNode content = currentNode;
                    currentNode.ReplaceWith(contentWrapper);
                    contentWrapper.Add(content);
                }
            }

            // Recursion:
            if (currentNode is XElement)
            {
                List<XNode> childNodes = ((XElement)currentNode).Nodes().ToList(); // Node: we convert to list because the code inside the loop below is going to modify the collection, so a "foreach" is not appropriate.
                int childNodesCount = childNodes.Count;
                for (int i = 0; i < childNodesCount; i++)
                {
                    TraverseNextNode(childNodes[i], childNodesCount, ((XElement)currentNode), reflectionOnSeparateAppDomain);
                }
            }
        }
    }
}
