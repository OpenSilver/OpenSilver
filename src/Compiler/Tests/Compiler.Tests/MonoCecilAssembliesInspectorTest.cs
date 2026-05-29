
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Experimental;
using OpenSilver.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using Mono.Cecil;

namespace Compiler.Tests;

[TestClass]
public partial class MonoCecilAssembliesInspectorTest
{
    private const string GlobalPrefix = "global::";
    private const string ExperimentalSubjectName = "Experimental";
    private const string ExperimentalSubjectDll = ExperimentalSubjectName + ".dll";
    private const string ExperimentalNamespace = "Experimental";
    private const string Content = "Content";

    private static readonly MonoCecilAssembliesInspectorImpl MonoCecilVersion = CreateMonoCecilInspector();
    private static readonly DefaultAssemblyResolver DefaultResolver = new();

    private static MonoCecilAssembliesInspectorImpl CreateMonoCecilInspector() => new(ExperimentalSubjectName, SupportedLanguage.CSharp);

    private static TypeDefinition GetClassWithMembersType() =>
        MonoCecilVersion.FindType(
            ExperimentalNamespace,
            nameof(ClassWithMembers),
            typeof(ClassWithMembers).Assembly.GetName().Name,
            null);

    [ClassInitialize]
    public static void ClassInitialize(TestContext _)
    {
        LoadAssemblyAndDependencies(ExperimentalSubjectDll);
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        MonoCecilVersion.Dispose();
    }

    [TestMethod]
    public void GetEnumValue_Should_Handle_Nested_Enum_Type()
    {
        var enumType = MonoCecilVersion.FindType(
            typeof(ClassWithNestedEnum).FullName,
            nameof(ClassWithNestedEnum.InputBehavior),
            typeof(ClassWithNestedEnum.InputBehavior).Assembly.GetName().Name,
            null);

        var res = MonoCecilVersion.GetEnumValue(enumType, nameof(ClassWithNestedEnum.InputBehavior.SelectFromList).ToLower(), true, true);

        Assert.AreEqual(res, $"{GlobalPrefix}{typeof(ClassWithNestedEnum).FullName}.{nameof(ClassWithNestedEnum.InputBehavior)}.{nameof(ClassWithNestedEnum.InputBehavior.SelectFromList)}");
    }

    [TestMethod]
    public void GetEnumValue_Should_Handle_Integer_Input_Value()
    {
        var enumType = MonoCecilVersion.FindType(
            typeof(ClassWithNestedEnum).FullName,
            nameof(ClassWithNestedEnum.InputBehavior),
            typeof(ClassWithNestedEnum.InputBehavior).Assembly.GetName().Name,
            null);

        var res = MonoCecilVersion.GetEnumValue(enumType, "1", true, true);

        Assert.AreEqual(res, $"({GlobalPrefix}{typeof(ClassWithNestedEnum).FullName}.{nameof(ClassWithNestedEnum.InputBehavior)})1");
    }

    [TestMethod]
    public void GetEnumValue_Should_Return_Value_For_Enum_Without_Namespace()
    {
        var enumType = MonoCecilVersion.FindType(
            "",
            nameof(EnumWithoutNamespace),
            typeof(EnumWithoutNamespace).Assembly.GetName().Name,
            null);

        var res = MonoCecilVersion.GetEnumValue(enumType, nameof(EnumWithoutNamespace.Item), true, false);

        Assert.AreEqual(res, $"{GlobalPrefix}{typeof(EnumWithoutNamespace).FullName}.{nameof(EnumWithoutNamespace.Item)}");
    }

    [TestMethod]
    public void GetContentPropertyName_Should_Return_Value()
    {
        var type = MonoCecilVersion.FindType(
            typeof(ContentControl).Namespace,
            nameof(ContentControl),
            typeof(ContentControl).Assembly.GetName().Name,
            null);

        var res = MonoCecilVersion.GetContentPropertyName(type, null);

        Assert.AreEqual(Content, res);
    }

    [TestMethod]
    public void UnloadAssembly_Should_Unload_And_Remove_Associated_Types()
    {
        var compilerTests = "Compiler.Tests";
        var assembly = MonoCecilVersion.LoadAssembly(compilerTests + ".dll");
        var typeBefore = MonoCecilVersion.FindType(
            compilerTests,
            nameof(MonoCecilAssembliesInspectorTest),
            compilerTests,
            null);
        MonoCecilVersion.UnloadAssembly(assembly);
        var typeAfter = MonoCecilVersion.FindType(
            compilerTests,
            nameof(MonoCecilAssembliesInspectorTest),
            compilerTests,
            null,
            throwIfNull: false);

        Assert.AreEqual(nameof(MonoCecilAssembliesInspectorTest), typeBefore.Name);
        Assert.IsNull(typeAfter);
    }

    private static void LoadAssemblyAndDependencies(string assemblyPath)
    {
        var loadedAssemblyNames = new HashSet<string>();
        var queue = new Queue<AssemblyDefinition>();
        queue.Enqueue(MonoCecilVersion.LoadAssembly(assemblyPath));
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
                    queue.Enqueue(MonoCecilVersion.LoadAssembly(assemblyFullPath));
                }
                else
                {
                    //There is not the assembly in the output folder.
                    //Maybe it is netstandard.dll or any another core library.
                    //Let's try to load via default resolver.
                    try
                    {
                        var ns = DefaultResolver.Resolve(referencedAssembly);
                        if (ns != null)
                        {
                            ns = MonoCecilVersion.LoadAssembly(ns.MainModule.FileName);
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
