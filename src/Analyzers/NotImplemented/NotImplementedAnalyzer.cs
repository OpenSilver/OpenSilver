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
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace OpenSilver.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NotImplementedAnalyzer : DiagnosticAnalyzer
    {
        private const string Title = "OpenSilver type or member is not implemented";

        private const string MessageFormat = "{0} is not implemented.";

        private const string Description = "This member or type is not implemented and might not behave as expected.";

        private const string Category = "Compatibility";

        internal static readonly DiagnosticDescriptor OS0001 =
            new DiagnosticDescriptor(
                nameof(OS0001),
                Title,
                MessageFormat,
                Category,
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(OS0001);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterCompilationStartAction(csa =>
            {
                var notImplSymbol = csa.Compilation.GetTypeByMetadataName("OpenSilver.NotImplementedAttribute");

                csa.RegisterSyntaxNodeAction(c => OnMemberAccessExpression(c, notImplSymbol), SyntaxKind.SimpleMemberAccessExpression);
                csa.RegisterSyntaxNodeAction(c => OnObjectCreationExpression(c, notImplSymbol), SyntaxKind.ObjectCreationExpression);
                csa.RegisterSyntaxNodeAction(c => OnMethodDeclaration(c, notImplSymbol), SyntaxKind.MethodDeclaration);
                csa.RegisterSyntaxNodeAction(c => OnPropertyDeclaration(c, notImplSymbol), SyntaxKind.PropertyDeclaration);
                csa.RegisterSyntaxNodeAction(c => OnEventDeclaration(c, notImplSymbol), SyntaxKind.EventDeclaration);
                csa.RegisterSyntaxNodeAction(c => OnBaseTypeDeclaration(c, notImplSymbol), SyntaxKind.SimpleBaseType);
            });
        }

        private void OnMemberAccessExpression(SyntaxNodeAnalysisContext contextAnalysis, INamedTypeSymbol notImplSymbol)
        {
            var memberAccess = contextAnalysis.Node as MemberAccessExpressionSyntax;

            var member = contextAnalysis.SemanticModel.GetSymbolInfo(memberAccess);

            if (member.Symbol is null)
            {
                return;
            }

            if (HasNotImplementedAttribute(notImplSymbol, member.Symbol.ContainingAssembly))
            {
                ReportDiagnostic(contextAnalysis, memberAccess.Name.GetLocation(), member.Symbol.ContainingAssembly.ToDisplayString());
                return;
            }

            ISymbol symbol;
            bool isMemberNotImplemented;
            switch (member.Symbol)
            {
                case IMethodSymbol methodSymbol:
                    isMemberNotImplemented = HasNotImplementedAttribute(notImplSymbol, methodSymbol, out symbol)
                        || HasNotImplementedAttribute(notImplSymbol, methodSymbol.ContainingType, out symbol);
                    break;
                case IPropertySymbol propertySymbol:
                    isMemberNotImplemented = HasNotImplementedAttribute(notImplSymbol, propertySymbol, out symbol)
                        || HasNotImplementedAttribute(notImplSymbol, propertySymbol.ContainingType, out symbol);
                    break;
                case IEventSymbol eventSymbol:
                    isMemberNotImplemented = HasNotImplementedAttribute(notImplSymbol, eventSymbol, out symbol)
                        || HasNotImplementedAttribute(notImplSymbol, eventSymbol.ContainingType, out symbol);
                    break;
                default:
                    if (isMemberNotImplemented = HasNotImplementedAttribute(notImplSymbol, member.Symbol))
                    {
                        symbol = member.Symbol;
                    }
                    else if (isMemberNotImplemented = HasNotImplementedAttribute(notImplSymbol, member.Symbol.ContainingSymbol))
                    {
                        symbol = member.Symbol.ContainingSymbol;
                    }
                    else
                    {
                        symbol = null;
                    }
                    break;
            }

            if (isMemberNotImplemented)
            {
                ReportDiagnostic(contextAnalysis, memberAccess.Name.GetLocation(), symbol.ToDisplayString());
            }
        }

        private void OnObjectCreationExpression(SyntaxNodeAnalysisContext contextAnalysis, INamedTypeSymbol notImplSymbol)
        {
            var objectCreation = contextAnalysis.Node as ObjectCreationExpressionSyntax;

            if (objectCreation != null)
            {
                // First, check if the constructor is implemented, then check
                // if the type is implemented.

                ISymbol symbol;

                IMethodSymbol ctorSymbol = contextAnalysis.SemanticModel.GetSymbolInfo(objectCreation).Symbol as IMethodSymbol;

                if (HasNotImplementedAttribute(notImplSymbol, ctorSymbol, out symbol))
                {
                    ReportDiagnostic(contextAnalysis, objectCreation.Type.GetLocation(), symbol.ToDisplayString());
                }
                else if (contextAnalysis.SemanticModel.GetSymbolInfo(objectCreation.Type).Symbol is INamedTypeSymbol typeSymbol)
                {
                    if (HasNotImplementedAttribute(notImplSymbol, typeSymbol.ContainingAssembly))
                    {
                        ReportDiagnostic(contextAnalysis, objectCreation.Type.GetLocation(), typeSymbol.ContainingAssembly.ToDisplayString());
                    }
                    else if (HasNotImplementedAttribute(notImplSymbol, typeSymbol, out symbol))
                    {
                        ReportDiagnostic(contextAnalysis, objectCreation.Type.GetLocation(), symbol.ToDisplayString());
                    }
                }
            }
        }

        private void OnMethodDeclaration(SyntaxNodeAnalysisContext contextAnalysis, INamedTypeSymbol notImplSymbol)
        {
            var methodDeclaration = contextAnalysis.Node as MethodDeclarationSyntax;

            if (methodDeclaration != null)
            {
                var symbol = contextAnalysis.SemanticModel.GetDeclaredSymbol(methodDeclaration);

                // We can skip to the base implementation directly, as a method declaration
                // will never be an opensilver symbol.
                if (HasNotImplementedAttribute(notImplSymbol, symbol?.OverriddenMethod, out ISymbol notImplMethod))
                {
                    ReportDiagnostic(contextAnalysis, methodDeclaration.Identifier.GetLocation(), notImplMethod.ToDisplayString());
                }
            }
        }

        private void OnPropertyDeclaration(SyntaxNodeAnalysisContext contextAnalysis, INamedTypeSymbol notImplSymbol)
        {
            var propertyDeclaration = contextAnalysis.Node as PropertyDeclarationSyntax;

            if (propertyDeclaration != null)
            {
                var symbol = contextAnalysis.SemanticModel.GetDeclaredSymbol(propertyDeclaration);

                // We can skip to the base implementation directly, as a property declaration
                // will never be an opensilver symbol.
                if (HasNotImplementedAttribute(notImplSymbol, symbol?.OverriddenProperty, out ISymbol notImplProperty))
                {
                    ReportDiagnostic(contextAnalysis, propertyDeclaration.Identifier.GetLocation(), notImplProperty.ToDisplayString());
                }
            }
        }

        private void OnEventDeclaration(SyntaxNodeAnalysisContext contextAnalysis, INamedTypeSymbol notImplSymbol)
        {
            var eventDeclaration = contextAnalysis.Node as EventDeclarationSyntax;

            if (eventDeclaration != null)
            {
                var symbol = contextAnalysis.SemanticModel.GetDeclaredSymbol(eventDeclaration);

                // We can skip to the base implementation directly, as a event declaration
                // will never be an opensilver symbol.
                if (HasNotImplementedAttribute(notImplSymbol, symbol?.OverriddenEvent, out ISymbol notImplEvent))
                {
                    ReportDiagnostic(contextAnalysis, eventDeclaration.Identifier.GetLocation(), notImplEvent.ToDisplayString());
                }
            }
        }

        private void OnBaseTypeDeclaration(SyntaxNodeAnalysisContext contextAnalysis, INamedTypeSymbol notImplSymbol)
        {
            var baseType = contextAnalysis.Node as SimpleBaseTypeSyntax;

            if (baseType != null)
            {
                var symbol = contextAnalysis.SemanticModel.GetSymbolInfo(baseType.Type);

                if (HasNotImplementedAttribute(notImplSymbol, symbol.Symbol as INamedTypeSymbol, out ISymbol notImplType))
                {
                    ReportDiagnostic(contextAnalysis, baseType.Type.GetLocation(), notImplType.ToDisplayString());
                }
            }
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, Location location, string messageArg)
        {
            context.ReportDiagnostic(Diagnostic.Create(OS0001, location, messageArg));
        }

        /// <summary>
        /// Check if a type is not implemented in OpenSilver.
        /// Include base types.
        /// </summary>
        /// <param name="notImplSymbol">
        /// OpenSilver.NotImplementedAttribute symbol
        /// </param>
        /// <param name="namedTypeSymbol">
        /// The type to check
        /// </param>
        /// <param name="notImplType">
        /// The type that is not implemented found in the inheritance
        /// hierarchy, if any.
        /// </param>
        /// <returns>
        /// <c>true</c> if the type or any of its base types is not implemented
        /// in OpenSilver, <c>false</c> otherwise.
        /// </returns>
        private static bool HasNotImplementedAttribute(INamedTypeSymbol notImplSymbol,
            INamedTypeSymbol namedTypeSymbol,
            out ISymbol notImplType)
        {
            for (var type = namedTypeSymbol; type != null; type = type.BaseType)
            {
                if (IsOpenSilverSymbol(type)
                    && HasNotImplementedAttribute(notImplSymbol, type))
                {
                    notImplType = type.IsGenericType ? type.OriginalDefinition : type;
                    return true;
                }
            }

            notImplType = null;
            return false;
        }

        /// <summary>
        /// Variant of <see cref="HasNotImplementedAttribute(INamedTypeSymbol, INamedTypeSymbol, out ISymbol)"/>
        /// for methods.
        /// </summary>
        private static bool HasNotImplementedAttribute(INamedTypeSymbol notImplSymbol,
            IMethodSymbol methodSymbol,
            out ISymbol notImplMethod)
        {
            for (var method = methodSymbol; method != null; method = method.OverriddenMethod)
            {
                if (IsOpenSilverSymbol(method)
                    && HasNotImplementedAttribute(notImplSymbol, method))
                {
                    notImplMethod = method.IsGenericMethod ? method.OriginalDefinition : method;
                    return true;
                }
            }

            notImplMethod = null;
            return false;
        }

        /// <summary>
        /// Variant of <see cref="HasNotImplementedAttribute(INamedTypeSymbol, INamedTypeSymbol, out ISymbol)"/>
        /// for properties.
        /// </summary>
        private static bool HasNotImplementedAttribute(INamedTypeSymbol notImplSymbol,
            IPropertySymbol propertySymbol,
            out ISymbol notImplProperty)
        {
            for (var property = propertySymbol; property != null; property = property.OverriddenProperty)
            {
                if (IsOpenSilverSymbol(property)
                    && HasNotImplementedAttribute(notImplSymbol, property))
                {
                    notImplProperty = property;
                    return true;
                }
            }

            notImplProperty = null;
            return false;
        }

        /// <summary>
        /// Variant of <see cref="HasNotImplementedAttribute(INamedTypeSymbol, INamedTypeSymbol, out ISymbol)"/>
        /// for events.
        /// </summary>
        private static bool HasNotImplementedAttribute(INamedTypeSymbol notImplSymbol,
            IEventSymbol eventSymbol,
            out ISymbol notImplEvent)
        {
            for (var @event = eventSymbol; @event != null; @event = @event.OverriddenEvent)
            {
                if (IsOpenSilverSymbol(@event)
                    && HasNotImplementedAttribute(notImplSymbol, @event))
                {
                    notImplEvent = @event;
                    return true;
                }
            }

            notImplEvent = null;
            return false;
        }

        private static bool HasNotImplementedAttribute(INamedTypeSymbol notImplSymbol, ISymbol symbol)
        {
            return symbol != null 
                && symbol
                .GetAttributes()
                .FirstOrDefault(
                    a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, notImplSymbol)
                ) != null;
        }

        private static bool IsOpenSilverSymbol(ISymbol member)
        {
            // For unit tests, the code in generated in an assembly named 
            // 'TestProject', so we need a special case for these.

            string name = member?.ContainingAssembly?.Name ?? "";

            return (name.StartsWith("OpenSilver", StringComparison.Ordinal) ||
                    (name.Equals("TestProject", StringComparison.Ordinal) &&
                     (member?.ContainingNamespace?.Name ?? "").Equals("OpenSilver", StringComparison.Ordinal)));

        }
    }
}
