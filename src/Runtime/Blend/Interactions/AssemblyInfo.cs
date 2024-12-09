// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Resources;
using System.Windows.Markup;

[module: SuppressMessage("Microsoft.MSInternal", "CA905:SystemNamespacesRequireApproval", Scope = "namespace", Target = "System.Windows.Interactivity")]
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes",
    Scope = "namespace", Target = "Microsoft.Expression.Interactivity.Input")]
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes",
    Scope = "namespace", Target = "Microsoft.Expression.Interactivity.Layout")]
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes",
    Scope = "namespace", Target = "Microsoft.Expression.Media.Effects")]

[assembly: SuppressMessage("Microsoft.Performance", "CA1824:MarkAssembliesWithNeutralResourcesLanguage")]
[assembly: NeutralResourcesLanguage("en", UltimateResourceFallbackLocation.MainAssembly)]

[assembly: XmlnsPrefix(@"http://schemas.microsoft.com/expression/2010/interactions", "ei")]
[assembly: XmlnsDefinition(@"http://schemas.microsoft.com/expression/2010/interactions", "Microsoft.Expression.Interactivity.Core")]
[assembly: XmlnsDefinition(@"http://schemas.microsoft.com/expression/2010/interactions", "Microsoft.Expression.Interactivity.Input")]
[assembly: XmlnsDefinition(@"http://schemas.microsoft.com/expression/2010/interactions", "Microsoft.Expression.Interactivity.Layout")]
[assembly: XmlnsDefinition(@"http://schemas.microsoft.com/expression/2010/interactions", "Microsoft.Expression.Interactivity.Media")]

[assembly: XmlnsPrefix(@"http://schemas.microsoft.com/expression/2010/effects", "ee")]
[assembly: XmlnsDefinition(@"http://schemas.microsoft.com/expression/2010/effects", "Microsoft.Expression.Media.Effects")]

[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Microsoft.Expression.Interactivity")]