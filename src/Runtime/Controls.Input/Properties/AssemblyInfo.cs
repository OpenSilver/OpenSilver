// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows.Markup;

[assembly: NeutralResourcesLanguage("en-US")]
[assembly: ComVisible(false)]

#if __CLSCOMPLIANT__
[assembly: CLSCompliant(true)]
#endif
[assembly: XmlnsPrefix("http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk", "sdk")]
#if MIGRATION
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk", "System.Windows.Controls")]
#else
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk", "Windows.UI.Xaml.Controls")]
#endif
