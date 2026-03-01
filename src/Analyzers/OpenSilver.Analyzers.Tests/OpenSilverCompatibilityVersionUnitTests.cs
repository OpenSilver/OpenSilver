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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenSilver.CodeAnalysis.Tests;

[TestClass]
public class OpenSilverCompatibilityVersionUnitTests : CSharpAnalyzerVerifier<OpenSilverCompatibilityVersionAnalyzer, DefaultVerifier>
{
    [TestMethod]
    public async Task When_ReferencedAssemblyVersionIsLower_ShouldWarn()
    {
        var test = CreateTest(
            legacyAssemblySource:
            """
            [assembly: OpenSilver.Runtime.CompilerServices.OpenSilverAssemblyAttribute]
            [assembly: OpenSilver.Runtime.CompilerServices.OpenSilverCompatibilityVersionAttribute(1u)]
            """,
            openSilverCompatibilityVersion: 2);

        test.ExpectedDiagnostics.Add(
            new DiagnosticResult(OpenSilverCompatibilityVersionAnalyzer.OS0003)
                .WithNoLocation()
                .WithArguments("LegacyAssembly"));

        await test.RunAsync();
    }

    [TestMethod]
    public async Task When_CompatibilityAttributeIsMissing_ShouldWarn()
    {
        var test = CreateTest(
            legacyAssemblySource:
            """
            [assembly: OpenSilver.Runtime.CompilerServices.OpenSilverAssemblyAttribute]
            """,
            openSilverCompatibilityVersion: 2);

        test.ExpectedDiagnostics.Add(
            new DiagnosticResult(OpenSilverCompatibilityVersionAnalyzer.OS0003)
                .WithNoLocation()
                .WithArguments("LegacyAssembly"));

        await test.RunAsync();
    }

    [TestMethod]
    public async Task When_ReferencedAssemblyVersionMatches_ShouldNotWarn()
    {
        var test = CreateTest(
            legacyAssemblySource:
            """
            [assembly: OpenSilver.Runtime.CompilerServices.OpenSilverAssemblyAttribute]
            [assembly: OpenSilver.Runtime.CompilerServices.OpenSilverCompatibilityVersionAttribute(2u)]
            """,
            openSilverCompatibilityVersion: 2);

        await test.RunAsync();
    }

    [TestMethod]
    public async Task When_SuppressionPropertyIsTrue_ShouldNotWarn()
    {
        var test = CreateTest(
            legacyAssemblySource:
            """
            [assembly: OpenSilver.Runtime.CompilerServices.OpenSilverAssemblyAttribute]
            [assembly: OpenSilver.Runtime.CompilerServices.OpenSilverCompatibilityVersionAttribute(1u)]
            """,
            openSilverCompatibilityVersion: 2);

        test.TestState.AnalyzerConfigFiles.Add((
            "/.globalconfig",
            """
            is_global = true
            build_property.OpenSilverSuppressCompatibilityWarnings = true
            """));

        await test.RunAsync();
    }

    private static CSharpAnalyzerTest<OpenSilverCompatibilityVersionAnalyzer, DefaultVerifier> CreateTest(
        string legacyAssemblySource,
        uint openSilverCompatibilityVersion)
    {
        var openSilverReference = CreateOpenSilverReference(openSilverCompatibilityVersion);
        var legacyReference = CreateLegacyAssemblyReference(legacyAssemblySource, openSilverReference);

        var test = new CSharpAnalyzerTest<OpenSilverCompatibilityVersionAnalyzer, DefaultVerifier>
        {
            TestCode =
                """
                namespace MyApp
                {
                    public class App { }
                }
                """,
        };

        test.TestState.AdditionalReferences.Add(openSilverReference);
        test.TestState.AdditionalReferences.Add(legacyReference);
        return test;
    }

    private static MetadataReference CreateOpenSilverReference(uint compatibilityVersion)
    {
        string source =
            $$"""
            using System;

            [assembly: OpenSilver.Runtime.CompilerServices.OpenSilverAssemblyAttribute]
            [assembly: OpenSilver.Runtime.CompilerServices.OpenSilverCompatibilityVersionAttribute({{compatibilityVersion}}u)]

            namespace OpenSilver.Runtime.CompilerServices
            {
                [AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
                public sealed class OpenSilverAssemblyAttribute : Attribute
                {
                    public OpenSilverAssemblyAttribute() { }
                }

                [AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
                public sealed class OpenSilverCompatibilityVersionAttribute : Attribute
                {
                    public OpenSilverCompatibilityVersionAttribute(uint version)
                    {
                        Version = version;
                    }

                    public uint Version { get; }
                }
            }
            """;

        return CreateAssemblyReference("OpenSilver", source);
    }

    private static MetadataReference CreateLegacyAssemblyReference(
        string legacyAssemblySource,
        MetadataReference openSilverReference)
    {
        return CreateAssemblyReference(
            "LegacyAssembly",
            legacyAssemblySource + Environment.NewLine + "namespace Legacy { public class Type { } }",
            openSilverReference);
    }

    private static MetadataReference CreateAssemblyReference(
        string assemblyName,
        string source,
        params MetadataReference[] extraReferences)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        var references = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .Concat(extraReferences)
            .Distinct(MetadataReferencePathComparer.Instance);

        var compilation = CSharpCompilation.Create(
            assemblyName,
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var stream = new MemoryStream();
        var emitResult = compilation.Emit(stream);

        if (!emitResult.Success)
        {
            string diagnostics = string.Join(Environment.NewLine, emitResult.Diagnostics.Select(d => d.ToString()));
            Assert.Fail(diagnostics);
        }

        stream.Position = 0;
        return MetadataReference.CreateFromStream(stream);
    }

    private sealed class MetadataReferencePathComparer : IEqualityComparer<MetadataReference>
    {
        public static MetadataReferencePathComparer Instance { get; } = new();

        public bool Equals(MetadataReference x, MetadataReference y)
        {
            string left = (x as PortableExecutableReference)?.FilePath;
            string right = (y as PortableExecutableReference)?.FilePath;
            return StringComparer.OrdinalIgnoreCase.Equals(left, right);
        }

        public int GetHashCode(MetadataReference obj)
        {
            string path = (obj as PortableExecutableReference)?.FilePath ?? string.Empty;
            return StringComparer.OrdinalIgnoreCase.GetHashCode(path);
        }
    }
}
