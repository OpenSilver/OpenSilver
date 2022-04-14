// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Resources;
#if __WPF__
using Microsoft.Expression.BlendSDK;
#endif

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
#if __WPF__
[assembly: AssemblyTitle("System.Windows.Interactivity")]
[assembly: AssemblyDescription("System.Windows.Interactivity")]
[assembly: AssemblyCompany("Microsoft Corporation")]
[assembly: AssemblyProduct("System.Windows.Interactivity")]
[assembly: AssemblyCopyright("Copyright (c) Microsoft Corporation. All rights reserved.")]

// The Revision number needs to be different from the Revision number for the WPF Behaviors assembly. Otherwise the CLR 
// will attempt to unify these assemblies when they are loaded into Blend.
// The AssemblyVersion is in this file, rather than Version, because we don't want the daily build to be reflected in the assembly version.
[assembly: AssemblyVersion(RuntimeVersion.AssemblyVersion)]
[assembly: AssemblyFileVersion(VersionConstants.AssemblyFileVersion)]
#endif

[assembly: System.Windows.Markup.XmlnsPrefix(@"http://schemas.microsoft.com/expression/2010/interactivity", "i")]
[assembly: System.Windows.Markup.XmlnsDefinition(@"http://schemas.microsoft.com/expression/2010/interactivity", "System.Windows.Interactivity")]

[assembly: SuppressMessage("Microsoft.Performance", "CA1824:MarkAssembliesWithNeutralResourcesLanguage")]
[assembly: NeutralResourcesLanguage("en", UltimateResourceFallbackLocation.MainAssembly)]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: System.Runtime.InteropServices.ComVisible(false)]
#if __CLSCOMPLIANT__
[assembly: System.CLSCompliant(true)]
#endif

#if __WPF__
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("UnitTests")]
#endif