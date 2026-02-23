
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
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OpenSilver.CodeAnalysis;

[ExportCodeFixProvider(LanguageNames.CSharp), Shared]
public class UseRootVisualOrStartupUriCodeFixProvider : CodeFixProvider
{
    private const string Title = "Replace with 'RootVisual'";

    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        [UseRootVisualOrStartupUriAnalyzer.OS0002.Id];

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null || root.FindNode(context.Span) is not SyntaxNode node)
        {
            return;
        }

        // The diagnostic is reported on the left side of the assignment (Window.Current.Content),
        // whose parent is the AssignmentExpressionSyntax.
        if (node.Parent is not AssignmentExpressionSyntax assignment)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                title: Title,
                createChangedDocument: ct => ReplaceWithRootVisualAsync(context.Document, assignment, ct),
                equivalenceKey: Title),
            context.Diagnostics);
    }

    private static async Task<Document> ReplaceWithRootVisualAsync(
        Document document,
        AssignmentExpressionSyntax assignment,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return document;
        }

        // Replace "Window.Current.Content" with "RootVisual", keeping the right-hand side intact
        var rootVisualIdentifier = SyntaxFactory.IdentifierName("RootVisual")
            .WithLeadingTrivia(assignment.Left.GetLeadingTrivia())
            .WithTrailingTrivia(assignment.Left.GetTrailingTrivia());

        var newAssignment = assignment.WithLeft(rootVisualIdentifier);

        var newRoot = root.ReplaceNode(assignment, newAssignment);

        return document.WithSyntaxRoot(newRoot);
    }
}
