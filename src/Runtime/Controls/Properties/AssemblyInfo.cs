// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

[assembly: XmlnsPrefix("http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk", "sdk")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk", "System.Windows.Controls")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk", "System.Windows.Controls.Primitives")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Controls")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Controls.Primitives")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/client/2007", "System.Windows.Controls")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/client/2007", "System.Windows.Controls.Primitives")]

[assembly: TypeForwardedTo(typeof(ChildWindow))]
[assembly: TypeForwardedTo(typeof(TreeView))]
[assembly: TypeForwardedTo(typeof(TreeViewItem))]
[assembly: TypeForwardedTo(typeof(HeaderedItemsControl))]
[assembly: TypeForwardedTo(typeof(HierarchicalDataTemplate))]