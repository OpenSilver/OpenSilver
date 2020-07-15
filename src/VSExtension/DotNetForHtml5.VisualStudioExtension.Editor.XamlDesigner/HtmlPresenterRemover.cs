using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DotNetForHtml5.VisualStudioExtension.Editor.XamlDesigner
{
    internal static class HtmlPresenterRemover
    {
        /// <summary>
        ///  Remove the content of all the "HtmlPresenter" nodes, because the content may be not well formatted and may lead to a syntax error when parsing the XDocument.
        /// </summary>
        public static string RemoveHtmlPresenterNodes(string sourceXaml)
        {
            return Regex.Replace(sourceXaml, @"(<[^<:>\/]+:HtmlPresenter[\s\S]*?>)[\s\S]+?(<\/[^<:>\/]+:HtmlPresenter>)", "$1$2");
        }
    }
}
