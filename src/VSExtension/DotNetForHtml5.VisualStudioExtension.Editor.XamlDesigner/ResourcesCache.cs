using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DotNetForHtml5.VisualStudioExtension.Editor.XamlDesigner
{
    internal class ResourcesCache
    {
        public ResourcesCache()
        {
            ResourceDictionaryFileNameToContent = new Dictionary<string, XElement>();
        }

        public IEnumerable<XElement> AppDotXamlResources { get; set; }
        public Dictionary<string, XElement> ResourceDictionaryFileNameToContent { get; set; } // The key is the full file name of the ResourceDictionary (including its absolute path), while the value is the content of the ResourceDictionary.
        public string AppDotXamlFullPath { get; set; }
    }
}
