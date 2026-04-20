
/*===================================================================================
*
*   Copyright (c) Userware/OpenSilver.net
*
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*
\*====================================================================================*/


using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Cecil;
using OpenSilver.Compiler;

namespace Compiler.Tests
{
    [TestClass]
    public class InsertingMarkupNodesInXamlTest
    {
        private const string ExperimentalSubjectName = "Experimental";
        private const string ExperimentalSubjectDll = ExperimentalSubjectName + ".dll";
        private const string XamlPresentationNs = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        private const string XamlXNs = "http://schemas.microsoft.com/winfx/2006/xaml";

        private static AssembliesInspector Inspector;
        private static ConversionSettings Settings;
        private static readonly DefaultAssemblyResolver DefaultResolver = new();

        [ClassInitialize]
        public static void ClassInitialize(TestContext _)
        {
            Inspector = new AssembliesInspector(ExperimentalSubjectName, SupportedLanguage.CSharp);
            LoadAssemblyAndDependencies(ExperimentalSubjectDll);

            Settings = new ConversionSettings(
                ExperimentalSubjectName,
                Inspector,
                new CoreTypesConverterCS(Inspector, ExperimentalSubjectName),
                SystemTypesHelper.CSharp,
                TypeReferenceHelper.CSharp,
                XamlPreprocessorOptions.Auto);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Inspector?.Dispose();
        }

        [TestMethod]
        public void InsertMarkupNodes_Should_Resolve_Positional_Parameter_For_Nested_DynamicResource()
        {
            var doc = XDocument.Parse($@"
<Border xmlns=""{XamlPresentationNs}""
        xmlns:x=""{XamlXNs}""
        Background=""{{DynamicResource {{x:Static SystemColors.HighlightBrushKey}}}}"" />");

            InsertingMarkupNodesInXaml.InsertMarkupNodes(doc, Settings);

            string serialized = doc.ToString();
            Assert.IsFalse(
                serialized.Contains("_placeHolderForDefaultValue"),
                $"Generated document should not contain the placeholder key.\n{serialized}");

            XElement background = FindFirstByLocalName(doc.Root, "Border.Background");
            Assert.IsNotNull(background, "Expected a Border.Background property element.");

            XElement dynamicRes = background.Elements()
                .FirstOrDefault(e => e.Name.LocalName == "DynamicResourceExtension");
            Assert.IsNotNull(dynamicRes, "Expected a DynamicResourceExtension element.");

            XElement resourceKey = dynamicRes.Elements()
                .FirstOrDefault(e => e.Name.LocalName == "DynamicResourceExtension.ResourceKey");
            Assert.IsNotNull(
                resourceKey,
                "Expected the positional parameter to be resolved to DynamicResourceExtension.ResourceKey.");

            XElement staticExt = resourceKey.Elements()
                .FirstOrDefault(e => e.Name.LocalName == "StaticExtension");
            Assert.IsNotNull(staticExt, "Expected a StaticExtension inside the resolved ResourceKey.");

            string member = staticExt.Attributes()
                .FirstOrDefault(a => a.Name.LocalName == "Member")?.Value;
            Assert.AreEqual("SystemColors.HighlightBrushKey", member);
        }

        [TestMethod]
        public void InsertMarkupNodes_Should_Resolve_Positional_Parameter_For_Nested_StaticResource()
        {
            var doc = XDocument.Parse($@"
<Border xmlns=""{XamlPresentationNs}""
        xmlns:x=""{XamlXNs}""
        Background=""{{StaticResource {{x:Static SystemColors.HighlightBrushKey}}}}"" />");

            InsertingMarkupNodesInXaml.InsertMarkupNodes(doc, Settings);

            string serialized = doc.ToString();
            Assert.IsFalse(
                serialized.Contains("_placeHolderForDefaultValue"),
                $"Generated document should not contain the placeholder key.\n{serialized}");

            XElement background = FindFirstByLocalName(doc.Root, "Border.Background");
            Assert.IsNotNull(background, serialized);

            // StaticResource is normalized to map onto StaticResourceExtension by the inspector, so the
            // generated markup node keeps the "StaticResource" local name (see special-casing in
            // MonoCecilAssembliesInspectorImpl.FindType). What matters for this regression test is that
            // the positional parameter is resolved to the ResourceKey content property, not the
            // "_placeHolderForDefaultValue" placeholder.
            XElement staticRes = background.Elements()
                .FirstOrDefault(e => e.Name.LocalName is "StaticResource" or "StaticResourceExtension");
            Assert.IsNotNull(staticRes, serialized);

            XElement resourceKey = staticRes.Elements()
                .FirstOrDefault(e => e.Name.LocalName == staticRes.Name.LocalName + ".ResourceKey");
            Assert.IsNotNull(
                resourceKey,
                $"Expected the positional parameter to be resolved to {staticRes.Name.LocalName}.ResourceKey.\n{serialized}");
        }

        [TestMethod]
        public void InsertMarkupNodes_Should_Still_Handle_Explicit_Property_Name()
        {
            var doc = XDocument.Parse($@"
<Border xmlns=""{XamlPresentationNs}""
        xmlns:x=""{XamlXNs}""
        Background=""{{DynamicResource ResourceKey={{x:Static SystemColors.HighlightBrushKey}}}}"" />");

            InsertingMarkupNodesInXaml.InsertMarkupNodes(doc, Settings);

            string serialized = doc.ToString();
            Assert.IsFalse(serialized.Contains("_placeHolderForDefaultValue"), serialized);

            XElement resourceKey = doc.Descendants()
                .FirstOrDefault(e => e.Name.LocalName == "DynamicResourceExtension.ResourceKey");
            Assert.IsNotNull(resourceKey);
        }

        private static XElement FindFirstByLocalName(XElement root, string localName)
        {
            if (root.Name.LocalName == localName)
            {
                return root;
            }

            return root.Descendants().FirstOrDefault(e => e.Name.LocalName == localName);
        }

        private static void LoadAssemblyAndDependencies(string assemblyPath)
        {
            var loadedAssemblyNames = new HashSet<string>();
            var queue = new Queue<AssemblyDefinition>();
            queue.Enqueue(Inspector.LoadAssembly(assemblyPath));
            while (queue.Count > 0)
            {
                var assembly = queue.Dequeue();
                loadedAssemblyNames.Add(assembly.Name.Name);

                var referencedAssemblies = assembly.MainModule.AssemblyReferences;

                foreach (var referencedAssembly in referencedAssemblies)
                {
                    if (loadedAssemblyNames.Contains(referencedAssembly.Name))
                    {
                        continue;
                    }

                    var assemblyFullPath = Path.Combine(Path.GetDirectoryName(assemblyPath) ?? "", referencedAssembly.Name + ".dll");
                    if (File.Exists(assemblyFullPath))
                    {
                        queue.Enqueue(Inspector.LoadAssembly(assemblyFullPath));
                    }
                    else
                    {
                        try
                        {
                            var ns = DefaultResolver.Resolve(referencedAssembly);
                            if (ns != null)
                            {
                                ns = Inspector.LoadAssembly(ns.MainModule.FileName);
                                if (ns != null)
                                {
                                    queue.Enqueue(ns);
                                }
                            }
                        }
                        catch (AssemblyResolutionException) { }
                    }
                }
            }
        }
    }
}
