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

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Experimental;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Cecil;
using OpenSilver.Compiler;

namespace Compiler.Tests;

[TestClass]
public class XamlEventOrderingTest
{
    [TestMethod]
    public void Convert_Should_Connect_Event_Handlers_Before_Setting_Properties()
    {
        using var inspector = new AssembliesInspector("Compiler.Tests", SupportedLanguage.CSharp);
        inspector.LoadAssembly(typeof(IList).Assembly.Location);
        LoadAssemblyAndDependencies(inspector, typeof(ClassWithMembers).Assembly.GetName().Name + ".dll");

        var settings = new ConversionSettings(
            "Compiler.Tests",
            inspector,
            new CoreTypesConverterCS(inspector, "Compiler.Tests"),
            SystemTypesHelper.CSharp,
            TypeReferenceHelper.CSharp,
            XamlPreprocessorOptions.Auto);

        string xaml = """
<controls:UserControl x:Class="Compiler.Tests.EventOrderingPage"
                      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                      xmlns:controls="clr-namespace:System.Windows.Controls;assembly=OpenSilver">
    <controls:RadioButton x:Name="InitialRadioButton"
                          IsChecked="True"
                          Checked="InitialRadioButton_Checked" />
</controls:UserControl>
""";

        string generatedCode = ConvertingXamlToCSharp.Convert(
            xaml,
            "EventOrderingPage.xaml",
            "EventOrderingPage.xaml",
            settings,
            isFirstPass: false);

        int nameConnectionIndex = generatedCode.IndexOf("XamlContext_SetConnectionId", StringComparison.Ordinal);
        int checkedConnectionIndex = generatedCode.IndexOf("XamlContext_SetConnectionId", nameConnectionIndex + 1, StringComparison.Ordinal);
        int checkedHandlerIndex = generatedCode.IndexOf("Checked += this.InitialRadioButton_Checked", StringComparison.Ordinal);
        int isCheckedAssignmentIndex = generatedCode.IndexOf(".IsChecked = true;", StringComparison.Ordinal);

        Assert.IsTrue(nameConnectionIndex >= 0, generatedCode);
        Assert.IsTrue(checkedConnectionIndex >= 0, generatedCode);
        Assert.IsTrue(checkedHandlerIndex >= 0, generatedCode);
        Assert.IsTrue(isCheckedAssignmentIndex >= 0, generatedCode);
        Assert.IsTrue(nameConnectionIndex < checkedConnectionIndex, generatedCode);
        Assert.IsTrue(checkedConnectionIndex < isCheckedAssignmentIndex, generatedCode);
    }

    private static void LoadAssemblyAndDependencies(AssembliesInspector inspector, string assemblyPath)
    {
        var loadedAssemblyNames = new HashSet<string>();
        var resolver = new DefaultAssemblyResolver();
        var queue = new Queue<AssemblyDefinition>();

        queue.Enqueue(inspector.LoadAssembly(assemblyPath));
        while (queue.Count > 0)
        {
            var assembly = queue.Dequeue();
            loadedAssemblyNames.Add(assembly.Name.Name);

            foreach (var referencedAssembly in assembly.MainModule.AssemblyReferences)
            {
                if (loadedAssemblyNames.Contains(referencedAssembly.Name))
                {
                    continue;
                }

                string assemblyFullPath = Path.Combine(Path.GetDirectoryName(assemblyPath) ?? "", referencedAssembly.Name + ".dll");
                if (File.Exists(assemblyFullPath))
                {
                    queue.Enqueue(inspector.LoadAssembly(assemblyFullPath));
                    continue;
                }

                try
                {
                    queue.Enqueue(resolver.Resolve(referencedAssembly));
                }
                catch
                {
                    // Some framework assemblies are not needed for this XAML generation test.
                }
            }
        }
    }
}
