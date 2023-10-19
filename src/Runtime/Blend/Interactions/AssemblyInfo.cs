// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System.Reflection;

#if __WPF__
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Microsoft.Expression.Interactions")]
[assembly: AssemblyDescription("Microsoft.Expression.Interactions")]
[assembly: AssemblyCompany("Microsoft Corporation")]
[assembly: AssemblyProduct("Microsoft.Expression.Interactions")]
[assembly: AssemblyCopyright("Copyright (c) Microsoft Corporation. All rights reserved.")]

// The Revision number needs to be different from the Revision number for the WPF Behaviors assembly. Otherwise the CLR 
// will attempt to unify these assemblies when they are loaded into Blend.
// The AssemblyVersion is in this file, rather than Version, because we don't want the daily build to be reflected in the assembly version.
[assembly: AssemblyVersion(Microsoft.Expression.BlendSDK.RuntimeVersion.AssemblyVersion)]
[assembly: AssemblyFileVersion(Microsoft.Expression.BlendSDK.VersionConstants.AssemblyFileVersion)]
#endif

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: System.Runtime.InteropServices.ComVisible(false)]
#if __CLSCOMPLIANT__
[assembly: System.CLSCompliant(true)]
#endif

[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.MSInternal", "CA905:SystemNamespacesRequireApproval", Scope = "namespace", Target = "System.Windows.Interactivity")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes",
    Scope = "namespace", Target = "Microsoft.Expression.Interactivity.Input")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes",
    Scope = "namespace", Target = "Microsoft.Expression.Interactivity.Layout")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes",
    Scope = "namespace", Target = "Microsoft.Expression.Media.Effects")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1824:MarkAssembliesWithNeutralResourcesLanguage")]
[assembly: System.Resources.NeutralResourcesLanguage("en", System.Resources.UltimateResourceFallbackLocation.MainAssembly)]

[assembly: System.Windows.Markup.XmlnsPrefix(@"http://schemas.microsoft.com/expression/2010/interactions", "ei")]
[assembly: System.Windows.Markup.XmlnsDefinition(@"http://schemas.microsoft.com/expression/2010/interactions", "Microsoft.Expression.Interactivity.Core")]
[assembly: System.Windows.Markup.XmlnsDefinition(@"http://schemas.microsoft.com/expression/2010/interactions", "Microsoft.Expression.Interactivity.Input")]
[assembly: System.Windows.Markup.XmlnsDefinition(@"http://schemas.microsoft.com/expression/2010/interactions", "Microsoft.Expression.Interactivity.Layout")]
[assembly: System.Windows.Markup.XmlnsDefinition(@"http://schemas.microsoft.com/expression/2010/interactions", "Microsoft.Expression.Interactivity.Media")]

[assembly: System.Windows.Markup.XmlnsPrefix(@"http://schemas.microsoft.com/expression/2010/effects", "ee")]
[assembly: System.Windows.Markup.XmlnsDefinition(@"http://schemas.microsoft.com/expression/2010/effects", "Microsoft.Expression.Media.Effects")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Microsoft.Expression.Interactivity")]

#if __WPF__
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("UnitTests")]
#endif