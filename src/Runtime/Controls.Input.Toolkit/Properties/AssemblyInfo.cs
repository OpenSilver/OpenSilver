// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;

[assembly: XmlnsPrefix("http://schemas.microsoft.com/winfx/2006/xaml/presentation/toolkit", "toolkit")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/toolkit", "System.Windows.Controls")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/toolkit", "System.Windows.Controls.Primitives")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Controls")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Controls.Primitives")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/client/2007", "System.Windows.Controls")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/client/2007", "System.Windows.Controls.Primitives")]

[assembly: TypeForwardedTo(typeof(ContextMenu))]
[assembly: TypeForwardedTo(typeof(ContextMenuService))]
[assembly: TypeForwardedTo(typeof(MenuBase))]
[assembly: TypeForwardedTo(typeof(MenuItem))]
[assembly: TypeForwardedTo(typeof(Separator))]