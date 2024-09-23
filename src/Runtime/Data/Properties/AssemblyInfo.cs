// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Markup;

[assembly: XmlnsPrefix("http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk", "sdk")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk", "System.Windows.Data")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Data")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/client/2007", "System.Windows.Data")]

[assembly: TypeForwardedTo(typeof(GroupDescriptionSelectorCallback))]
[assembly: TypeForwardedTo(typeof(CollectionViewGroup))]
[assembly: TypeForwardedTo(typeof(PropertyGroupDescription))]
[assembly: TypeForwardedTo(typeof(NewItemPlaceholderPosition))]
[assembly: TypeForwardedTo(typeof(IEditableCollectionView))]