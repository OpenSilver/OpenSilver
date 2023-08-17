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

[assembly: XmlnsPrefix("http://schemas.microsoft.com/winfx/2006/xaml/presentation/toolkit", "toolkit")]

#if MIGRATION
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Controls")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Controls.Primitives")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/toolkit", "System.Windows.Controls")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/toolkit", "System.Windows.Controls.Primitives")]
#else
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Windows.UI.Xaml.Controls")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Windows.UI.Xaml.Controls.Primitives")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/toolkit", "Windows.UI.Xaml.Controls")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/toolkit", "Windows.UI.Xaml.Controls.Primitives")]
#endif
