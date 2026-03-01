
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
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace OpenSilver.CodeAnalysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class OpenSilverCompatibilityVersionAnalyzer : DiagnosticAnalyzer
{
    private const string Title = "OpenSilver assembly is outdated";
    private const string MessageFormat = "Assembly '{0}' is outdated and was not tested with the selected OpenSilver version. This can cause build or runtime errors. Rebuild or upgrade this assembly with a more recent OpenSilver package. You may also set <OpenSilverSuppressCompatibilityWarnings>true</OpenSilverSuppressCompatibilityWarnings> in the project file to ignore this warning and continue at your own risk.";
    private const string Description = "The assembly was built with an older version of OpenSilver and may not behave correctly with the selected version.";
    private const string Category = "Compatibility";

    internal static readonly DiagnosticDescriptor OS0003 =
        new DiagnosticDescriptor(
            nameof(OS0003),
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Description,
            customTags: [WellKnownDiagnosticTags.CompilationEnd]);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [OS0003];

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterCompilationAction(ca =>
        {
            if (IsSuppressed(ca.Options.AnalyzerConfigOptionsProvider))
            {
                return;
            }

            if (Helpers.FindOpenSilverAssembly(ca.Compilation) is not IAssemblySymbol openSilverAssembly)
            {
                return;
            }

            var openSilverAssemblyAttributeSymbol = openSilverAssembly.GetTypeByMetadataName(
                "OpenSilver.Runtime.CompilerServices.OpenSilverAssemblyAttribute");
            var openSilverCompatibilityVersionAttributeSymbol = openSilverAssembly.GetTypeByMetadataName(
                "OpenSilver.Runtime.CompilerServices.OpenSilverCompatibilityVersionAttribute");

            if (openSilverAssemblyAttributeSymbol is null ||
                openSilverCompatibilityVersionAttributeSymbol is null ||
                !TryGetCompatibilityVersion(
                    openSilverAssembly,
                    openSilverCompatibilityVersionAttributeSymbol,
                    out uint openSilverVersion))
            {
                return;
            }

            foreach (IAssemblySymbol assembly in EnumerateAssemblies(ca.Compilation))
            {
                if (!HasAttribute(assembly, openSilverAssemblyAttributeSymbol))
                {
                    continue;
                }

                if (!TryGetCompatibilityVersion(
                    assembly,
                    openSilverCompatibilityVersionAttributeSymbol,
                    out uint assemblyVersion))
                {
                    ReportOutdatedAssembly(ca, assembly);
                    continue;
                }

                if (assemblyVersion < openSilverVersion)
                {
                    ReportOutdatedAssembly(ca, assembly);
                }
            }
        });
    }

    private static bool IsSuppressed(AnalyzerConfigOptionsProvider optionsProvider)
    {
        const string SuppressWarningPropertyName = "build_property.OpenSilverSuppressCompatibilityWarnings";

        if (!optionsProvider.GlobalOptions.TryGetValue(SuppressWarningPropertyName, out string value))
        {
            return false;
        }

        return bool.TryParse(value, out bool suppressed) && suppressed;
    }

    private static IEnumerable<IAssemblySymbol> EnumerateAssemblies(Compilation compilation)
    {
        yield return compilation.Assembly;

        foreach (IAssemblySymbol assembly in compilation.SourceModule.ReferencedAssemblySymbols)
        {
            yield return assembly;
        }
    }

    private static void ReportOutdatedAssembly(CompilationAnalysisContext context, IAssemblySymbol assembly)
        => context.ReportDiagnostic(Diagnostic.Create(OS0003, Location.None, assembly.Name));

    private static bool HasAttribute(IAssemblySymbol assembly, INamedTypeSymbol attributeSymbol)
        => GetAttribute(assembly, attributeSymbol) is not null;

    private static AttributeData GetAttribute(IAssemblySymbol assembly, INamedTypeSymbol attributeSymbol)
        => assembly
            .GetAttributes()
            .FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeSymbol));

    private static bool TryGetCompatibilityVersion(
        IAssemblySymbol assembly,
        INamedTypeSymbol openSilverCompatibilityVersionAttributeSymbol,
        out uint version)
    {
        if (GetAttribute(assembly, openSilverCompatibilityVersionAttributeSymbol) is not AttributeData attribute)
        {
            version = 0;
            return false;
        }

        if (attribute.ConstructorArguments.Length > 0 && TryReadVersion(attribute.ConstructorArguments[0], out version))
        {
            return true;
        }

        foreach (var namedArgument in attribute.NamedArguments)
        {
            if (namedArgument.Key.Equals("Version", StringComparison.Ordinal) && TryReadVersion(namedArgument.Value, out version))
            {
                return true;
            }
        }

        version = default;
        return false;
    }

    private static bool TryReadVersion(TypedConstant constant, out uint version)
    {
        switch (constant.Value)
        {
            case uint u:
                version = u;
                return true;
            case int i when i >= 0:
                version = (uint)i;
                return true;
            case string s when uint.TryParse(s, NumberStyles.None, CultureInfo.InvariantCulture, out uint parsed):
                version = parsed;
                return true;
            default:
                version = 0;
                return false;
        }
    }
}
