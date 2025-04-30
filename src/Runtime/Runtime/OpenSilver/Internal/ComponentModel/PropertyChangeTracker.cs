
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System;
using System.Diagnostics;
using System.Windows;
using OpenSilver.Internal.Data;

namespace OpenSilver.Internal.ComponentModel;

/// <summary>
///     A change tracking expression that is used to raise property change events.
/// </summary>
internal sealed class PropertyChangeTracker
{
    private readonly PropertyChangeListener _listener;

    internal PropertyChangeTracker(DependencyObject obj, DependencyProperty property)
    {
        Debug.Assert(obj is not null && property is not null);
        _listener = PropertyChangeListener.CreateListener(obj, property, OnPropertyInvalidation);
    }

    private void OnPropertyInvalidation(DependencyObject d, DependencyPropertyChangedEventArgs args) => Changed?.Invoke(d, EventArgs.Empty);

    internal void Close() => _listener?.Dispose();

    internal bool CanClose => Changed is null;

    internal EventHandler Changed;
}