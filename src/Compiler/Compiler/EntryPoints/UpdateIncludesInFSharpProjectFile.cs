
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
using System.Xml;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace OpenSilver.Compiler
{
    public sealed class UpdateIncludesInFSharpProjectFile : Task
    {
        private const string _xpathExpression = "//ItemGroup/*";

        [Required]
        public string ProjectPath { get; set; }

        [Required]
        public ITaskItem[] AllItems { get; set; }

        [Output]
        public ITaskItem[] UpdatedItems { get; set; }

        public override bool Execute()
        {
            var results = new List<ITaskItem>();

            foreach (ITaskItem item in GetIncludes())
            {
                if (string.Equals(item.GetMetadata("Extension"), ".xaml", StringComparison.OrdinalIgnoreCase))
                {
                    string compiledXamlFilePath = item.GetMetadata(XamlPreprocessor.CompiledXamlFilePathMetadata);
                    results.Add(new TaskItem(compiledXamlFilePath));
                }
                else
                {
                    results.Add(new TaskItem(item));
                }
            }

            UpdatedItems = results.ToArray();
            return true;
        }

        private IEnumerable<ITaskItem> GetIncludes()
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(ProjectPath);

            var topLevelItemGroupChildren = xmlDoc.SelectNodes(_xpathExpression);

            // When project file uses "<Choose>, <When>" tags, it needs to check include files that passed conditions.
            var itemsMap = CreateItemsMap(AllItems);

            if (topLevelItemGroupChildren == null)
            {
                yield break;
            }

            // Extract the "Include" attribute value for each selected XmlElement
            foreach (var element in topLevelItemGroupChildren.OfType<XmlElement>())
            {
                if (ProcessElement(element, out string filePath))
                {
                    if (itemsMap.TryGetValue(filePath, out ITaskItem item))
                    {
                        // Add the value of the "Include" attribute to the result list
                        yield return item;
                    }
                }
            }
        }

        private static Dictionary<string, ITaskItem> CreateItemsMap(ITaskItem[] items)
        {
            var d = new Dictionary<string, ITaskItem>();
            foreach (ITaskItem item in items)
            {
                string path = item.GetMetadata("Identity");
                d[path] = item;
            }
            return d;
        }

        private static bool ProcessElement(XmlElement element, out string filePath)
        {
            return IsFsharpFile(element, out filePath) || IsXamlFile(element, out filePath);
        }

        private static bool IsFsharpFile(XmlElement element, out string filePath)
        {
            if (element.Name == "Compile")
            {
                var include = element.Attributes["Include"];
                if (include is not null && !string.IsNullOrWhiteSpace(include.Value))
                {
                    filePath = include.Value;
                    return true;
                }
            }

            filePath = null;
            return false;
        }

        private static bool IsXamlFile(XmlElement element, out string filePath)
        {
            if (element.Name == "Page" || element.Name == "Content" || element.Name == "ApplicationDefinition")
            {
                var include = element.Attributes["Include"];
                if (include?.Value?.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase) ?? false)
                {
                    filePath = include.Value;
                    return true;
                }
            }

            filePath = null;
            return false;
        }
    }
}
