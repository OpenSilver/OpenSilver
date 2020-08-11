

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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DotNetForHtml5.Compiler
{
    internal static class FixingVisualStatesName
    {
        const string visualStateManagerGroupsAsString = "VisualStateManager.VisualStateGroups";
        const string visualStateGroupAsString = "VisualStateGroup";
        const string visualStateGroupDotStatesAsString = "VisualStateGroup.States";
        const string visualStateAsString = "VisualState";
        static bool IsInitialized { get; set; }
        static Dictionary<string, string> _UWPToWPFVisualStatesName;

        internal static void Fix(XDocument doc, bool isSLMigration)
        {
            if (!isSLMigration)
            {
                return;
            }
            else
            {
                if (!IsInitialized)
                {
                    Initialize();
                }
                TraverseNextElement(doc.Root);
            }
        }

        static void Initialize()
        {
            if (_UWPToWPFVisualStatesName == null)
            {
                _UWPToWPFVisualStatesName = new Dictionary<string, string>();
            }
            else
            {
                _UWPToWPFVisualStatesName.Clear();
            }
            //We only have this Name for now but we might add more in the future.
            _UWPToWPFVisualStatesName.Add("PointerOver", "MouseOver");
            IsInitialized = true;
        }

        static void TraverseNextElement(XElement currentElement)
        {
            if (IsVisualStateGroupManager(currentElement))
            {
                ProcessVisualStateManagerChildren(currentElement);
            }
            else
            {
                // Recursion
                foreach (XElement child in currentElement.Elements())
                {
                    TraverseNextElement(child);
                }
            }
        }

        static void ProcessVisualStateManagerChildren(XElement currentElement)
        {
            foreach (XElement child in currentElement.Elements())
            {
                if (child.Name.LocalName == visualStateGroupAsString)
                {
                    XElement states = FindVisualStateGroupChilds(child);
                    if(states != null)
                    {
                        ProcessVisualStateGroup(states);
                    }
                }
            }
        }

        static void ProcessVisualStateGroup(XElement currentVisualStateGroupAsXElement)
        {
            foreach (XElement child in currentVisualStateGroupAsXElement.Elements())
            {
                if (child.Name.LocalName == visualStateAsString)
                {
                    ProcessVisualState(child);
                }
            }
        }

        static void ProcessVisualState(XElement visualStateAsXElement)
        {
            XAttribute nameAttribute = GetXNameAttribute(visualStateAsXElement);
            if (nameAttribute != null)
            {
                if (_UWPToWPFVisualStatesName.ContainsKey(nameAttribute.Value))
                {
                    nameAttribute.Value = _UWPToWPFVisualStatesName[nameAttribute.Value];
                }
            }
        }

        static XElement FindVisualStateGroupChilds(XElement visualStateGroup)
        {
            foreach (XElement child in visualStateGroup.Elements())
            {
                if (child.Name.LocalName == visualStateGroupDotStatesAsString)
                {
                    return child;
                }
            }
            return null;
        }

        static bool IsVisualStateGroupManager(XElement element)
        {
            return (element.Name.LocalName == visualStateManagerGroupsAsString &&
                (element.Name.NamespaceName == GeneratingCSharpCode.DefaultXamlNamespace || element.Name.NamespaceName == "System.Windows"));
        }

        static bool IsAttributeTheXNameAttribute(XAttribute attribute)
        {
            bool isXName = (attribute.Name.LocalName == "Name" && attribute.Name.NamespaceName == GeneratingCSharpCode.xNamespace);
            bool isName = (attribute.Name.LocalName == "Name" && string.IsNullOrEmpty(attribute.Name.NamespaceName));
            return isXName || isName;
        }

        static XAttribute GetXNameAttribute(XElement element)
        {
            if (element != null && element.HasAttributes)
            {
                foreach (XAttribute attribute in element.Attributes())
                {
                    if (IsAttributeTheXNameAttribute(attribute))
                    {
                        return attribute;
                    }
                }
            }
            return null;
        }
    }
}
