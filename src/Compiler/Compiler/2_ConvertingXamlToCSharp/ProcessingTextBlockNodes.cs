using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace DotNetForHtml5.Compiler
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

        public static void Process(XDocument doc, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain)
        {
            TraverseNextNode(doc.Root, 1, null, reflectionOnSeparateAppDomain);
        }

        static void TraverseNextNode(XNode currentNode, int siblingNodesCount, XElement parentElement, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain)
        {
            if (currentNode is XText)
            {
                if ((parentElement.Name == GeneratingCSharpCode.DefaultXamlNamespace + "TextBlock" && siblingNodesCount > 1) // Note: if there is only one XNode inside a TextBlock, we do not surround with a <Run> for runtime performance optimization.
                    || parentElement.Name == GeneratingCSharpCode.DefaultXamlNamespace + "Span"
                    || parentElement.Name == GeneratingCSharpCode.DefaultXamlNamespace + "Italic"
                    || parentElement.Name == GeneratingCSharpCode.DefaultXamlNamespace + "Underline"
                    || parentElement.Name == GeneratingCSharpCode.DefaultXamlNamespace + "Bold"
                    || parentElement.Name == GeneratingCSharpCode.DefaultXamlNamespace + "Hyperlink")
                {
                    // Surround with a <Run>:
                    XElement contentWrapper = new XElement(GeneratingCSharpCode.DefaultXamlNamespace + "Run"); //todo: read the "ContentWrapperAttribute" of the collection (cf. InlineCollection.cs) instead of hard-coding this.
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
