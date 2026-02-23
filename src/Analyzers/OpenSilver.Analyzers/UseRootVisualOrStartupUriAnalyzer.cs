
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

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace OpenSilver.CodeAnalysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class UseRootVisualOrStartupUriAnalyzer : DiagnosticAnalyzer
{
    private const string Title = "Use RootVisual or StartupUri to initialize the Application";
    private const string MessageFormat = "Initializing an application with 'Window.Current.Content' is not supported. Use 'Application.RootVisual' or 'Application.StartupUri' instead.";
    private const string Description = MessageFormat;
    private const string Category = "Usage";

    public static readonly DiagnosticDescriptor OS0002 =
        new DiagnosticDescriptor(
            nameof(OS0002),
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [OS0002];

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterCompilationStartAction(csa =>
        {
            if (Helpers.FindOpenSilverAssembly(csa.Compilation) is not IAssemblySymbol openSilverAssembly)
            {
                return;
            }

            var applicationSymbol = openSilverAssembly.GetTypeByMetadataName("System.Windows.Application");
            var windowSymbol = openSilverAssembly.GetTypeByMetadataName("System.Windows.Window");

            if (applicationSymbol is null || windowSymbol is null)
            {
                return;
            }

            csa.RegisterSyntaxNodeAction(
                c => OnClassDeclaration(c, applicationSymbol, windowSymbol),
                SyntaxKind.ClassDeclaration);
        });
    }

    private static void OnClassDeclaration(
        SyntaxNodeAnalysisContext context,
        INamedTypeSymbol applicationSymbol,
        INamedTypeSymbol windowSymbol)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;

        // Check if this class derives from System.Windows.Application
        if (context.SemanticModel.GetDeclaredSymbol(classDeclaration) is not INamedTypeSymbol classSymbol)
        {
            return;
        }

        if (!DerivesFrom(classSymbol, applicationSymbol))
        {
            return;
        }

        // Find all assignment expressions in methods and constructors declared directly in this class
        var assignments = classDeclaration.Members
            .Where(m => m is MethodDeclarationSyntax || m is ConstructorDeclarationSyntax)
            .SelectMany(m => m.DescendantNodes().OfType<AssignmentExpressionSyntax>())
            .Where(a => a.IsKind(SyntaxKind.SimpleAssignmentExpression));

        foreach (var assignment in assignments)
        {
            if (IsWindowCurrentContentAssignment(context, assignment, windowSymbol))
            {
                context.ReportDiagnostic(Diagnostic.Create(OS0002, assignment.Left.GetLocation()));
            }
        }
    }

    private static bool IsWindowCurrentContentAssignment(
        SyntaxNodeAnalysisContext context,
        AssignmentExpressionSyntax assignment,
        INamedTypeSymbol windowSymbol)
    {
        // The left side must be a member access expression (e.g. Window.Current.Content)
        if (assignment.Left is not MemberAccessExpressionSyntax contentAccess)
        {
            return false;
        }

        // Check if the property being assigned is the Content property of System.Windows.Window
        if (context.SemanticModel.GetSymbolInfo(contentAccess).Symbol is not IPropertySymbol propertySymbol
            || propertySymbol.Name != "Content")
        {
            return false;
        }

        if (!SymbolEqualityComparer.Default.Equals(propertySymbol.ContainingType, windowSymbol))
        {
            return false;
        }

        // The expression before .Content must be a member access to "Current" (i.e. Window.Current)
        if (contentAccess.Expression is not MemberAccessExpressionSyntax currentAccess)
        {
            return false;
        }

        if (context.SemanticModel.GetSymbolInfo(currentAccess).Symbol is not IPropertySymbol currentSymbol
            || currentSymbol.Name != "Current")
        {
            return false;
        }

        // Current must belong to System.Windows.Window
        if (!SymbolEqualityComparer.Default.Equals(currentSymbol.ContainingType, windowSymbol))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Checks whether <paramref name="type"/> derives from <paramref name="baseType"/>
    /// (or is the same type).
    /// </summary>
    private static bool DerivesFrom(INamedTypeSymbol type, INamedTypeSymbol baseType)
    {
        for (var current = type; current != null; current = current.BaseType)
        {
            if (SymbolEqualityComparer.Default.Equals(current, baseType))
            {
                return true;
            }
        }

        return false;
    }
}
