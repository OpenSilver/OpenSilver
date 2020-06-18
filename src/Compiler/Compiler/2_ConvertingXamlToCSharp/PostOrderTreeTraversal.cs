using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DotNetForHtml5.Compiler
{
    internal static class PostOrderTreeTraversal
    {
        public static IEnumerable<XElement> TraverseTreeInPostOrder(XElement currentElement)
        {
            foreach (var childNode in currentElement.Elements())
            {
                foreach (var item in TraverseTreeInPostOrder(childNode))
                {
                    yield return item;
                }
            }
            yield return currentElement;
        }
    }
}
