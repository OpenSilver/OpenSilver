// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents the method that will handle the
    /// <see cref="E:System.Windows.Controls.AutoCompleteBox.Populated" />
    /// event of a <see cref="T:System.Windows.Controls.AutoCompleteBox" />
    /// control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A
    /// <see cref="T:System.Windows.Controls.PopulatedEventArgs" /> that
    /// contains the event data.</param>
    /// <QualityBand>Stable</QualityBand>
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances", Justification = "There is no generic RoutedEventHandler.")]
    public delegate void PopulatedEventHandler(object sender, PopulatedEventArgs e);
}