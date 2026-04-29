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
public class XamlSetterPropertyTest
{
    [TestMethod]
    public void Convert_Should_Report_Missing_Style_Setter_Property()
    {
        string xaml = """
<windows:ResourceDictionary xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                            xmlns:windows="clr-namespace:System.Windows;assembly=OpenSilver"
                            xmlns:controls="clr-namespace:System.Windows.Controls;assembly=OpenSilver">
    <windows:Style TargetType="{x:Type controls:TextBlock}">
        <windows:Setter Property="Background" Value="#00ffffff" />
    </windows:Style>
</windows:ResourceDictionary>
""";

        XamlParseException exception = ConvertAndGetXamlParseException(xaml);

        Assert.IsNotNull(exception);
        StringAssert.Contains(
            exception.Message,
            "Property or field 'Background' not found in type 'System.Windows.Controls.TextBlock'.");
    }

    [TestMethod]
    public void Convert_Should_Report_Missing_Trigger_Property_In_ControlTemplate()
    {
        string xaml = """
<windows:ResourceDictionary xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                            xmlns:windows="clr-namespace:System.Windows;assembly=OpenSilver"
                            xmlns:controls="clr-namespace:System.Windows.Controls;assembly=OpenSilver"
                            xmlns:primitives="clr-namespace:System.Windows.Controls.Primitives;assembly=OpenSilver">
    <windows:Style TargetType="{x:Type controls:MenuItem}">
        <windows:Setter Property="Template">
            <windows:Setter.Value>
                <controls:ControlTemplate TargetType="{x:Type controls:MenuItem}">
                    <controls:Grid>
                        <primitives:Popup x:Name="SubMenuPopup" />
                    </controls:Grid>
                    <controls:ControlTemplate.Triggers>
                        <windows:Trigger Property="IsSuspendingPopupAnimation" Value="true">
                            <windows:Setter TargetName="SubMenuPopup" Property="PopupAnimation" Value="None" />
                        </windows:Trigger>
                    </controls:ControlTemplate.Triggers>
                </controls:ControlTemplate>
            </windows:Setter.Value>
        </windows:Setter>
    </windows:Style>
</windows:ResourceDictionary>
""";

        XamlParseException exception = ConvertAndGetXamlParseException(xaml);

        Assert.IsNotNull(exception);
        StringAssert.Contains(
            exception.Message,
            "Property or field 'IsSuspendingPopupAnimation' not found in type 'System.Windows.Controls.MenuItem'.");
    }

    private static XamlParseException ConvertAndGetXamlParseException(string xaml)
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

        try
        {
            ConvertingXamlToCSharp.Convert(
                xaml,
                "SetterPropertyPage.xaml",
                "SetterPropertyPage.xaml",
                settings,
                isFirstPass: false);
        }
        catch (XamlParseException ex)
        {
            return ex;
        }
        catch (Exception ex)
        {
            Assert.Fail($"Expected a XamlParseException, but got {ex.GetType().FullName}: {ex.Message}");
        }

        return null;
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
