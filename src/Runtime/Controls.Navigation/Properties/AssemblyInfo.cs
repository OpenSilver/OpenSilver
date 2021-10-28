//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Markup;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
#if OPENSILVER
[assembly: AssemblyTitle("OpenSilver.Controls.Navigation")]
#elif BRIDGE
[assembly: AssemblyTitle("CSHTML5.Controls.Navigation")]
#endif
[assembly: AssemblyDescription("Silverlight Navigation Controls for OpenSilver")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("")]
[assembly: AssemblyCopyright("© Copyright 2021.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

#if false
[assembly: CLSCompliant(true)]
#endif

[assembly: XmlnsPrefix("http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk", "sdk")]

#if MIGRATION
// Note: Silverlight does not define an XmlnsDefinition for the default xaml namespace, this
// is only here for backward compatibility reasons
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Controls")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Navigation")]

[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk", "System.Windows.Controls")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk", "System.Windows.Navigation")]
#else
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Windows.UI.Xaml.Controls")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Windows.UI.Xaml.Navigation")]

[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk", "Windows.UI.Xaml.Controls")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk", "Windows.UI.Xaml.Navigation")]
#endif

[assembly: NeutralResourcesLanguageAttribute("en-us")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:
[assembly: AssemblyVersion("2.0.5.0")]
[assembly: AssemblyFileVersion("4.0.40413.2002")]

#if false
// Type-forwarding the types that used to be defined in this assembly and are now
// defined in core's System.Windows.dll assembly.
[assembly: TypeForwardedTo(typeof(System.Windows.Navigation.NavigationEventArgs))]
[assembly: TypeForwardedTo(typeof(System.Windows.Navigation.NavigatingCancelEventHandler))]
[assembly: TypeForwardedTo(typeof(System.Windows.Navigation.NavigatedEventHandler))]
[assembly: TypeForwardedTo(typeof(System.Windows.Navigation.NavigatingCancelEventArgs))]
[assembly: TypeForwardedTo(typeof(System.Windows.Navigation.NavigationMode))]
#endif
