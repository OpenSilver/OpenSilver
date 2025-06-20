
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

using System.Windows.Controls;

namespace System.Windows;

/// <summary>
/// Provides data for the <see cref="VisualStateGroup.CurrentStateChanging"/> and <see cref="VisualStateGroup.CurrentStateChanged"/> 
/// events.
/// </summary>
/// <remark>
/// When the <see cref="VisualStateGroup"/> that raises the event is set on the <see cref="ControlTemplate"/> of a control, the 
/// <see cref="Control"/> property is the control that owns the <see cref="ControlTemplate"/>. When the <see cref="VisualStateGroup"/> 
/// is set on a <see cref="FrameworkElement"/> that is not in a <see cref="ControlTemplate"/>, the <see cref="Control"/> property is 
/// null and you should use the <see cref="StateGroupsRoot"/> property.
/// </remark>
public sealed class VisualStateChangedEventArgs : EventArgs
{
    internal VisualStateChangedEventArgs(VisualState oldState, VisualState newState, FrameworkElement control, FrameworkElement stateGroupsRoot)
    {
        OldState = oldState;
        NewState = newState;
        Control = control;
        StateGroupsRoot = stateGroupsRoot;
    }

    /// <summary>
    /// Gets the state that the element is transitioning to or has transitioned from.
    /// </summary>
    /// <returns>
    /// The state that the element is transitioning to or has transitioned from.
    /// </returns>
    public VisualState OldState { get; }

    /// <summary>
    /// Gets the state that the element is transitioning to or has transitioned to.
    /// </summary>
    /// <returns>
    /// The state that the element is transitioning to or has transitioned to.
    /// </returns>
    public VisualState NewState { get; }

    /// <summary>
    /// Gets the element that is transitioning states.
    /// </summary>
    /// <returns>
    /// The element that is transitioning states if the <see cref="VisualStateGroup"/> is in a <see cref="ControlTemplate"/>; otherwise, null.
    /// </returns>
    /// <remarks>
    /// When the <see cref="VisualStateGroup"/> is set on a <see cref="FrameworkElement"/> that is not in a <see cref="ControlTemplate"/>, 
    /// the <see cref="Control"/> property is null and you should use the <see cref="StateGroupsRoot"/> property.
    /// </remarks>
    public FrameworkElement Control { get; }

    /// <summary>
    /// Gets the root element that contains the <see cref="VisualStateManager"/>.
    /// </summary>
    /// <returns>
    /// The root element that contains the <see cref="VisualStateManager"/>.
    /// </returns>
    public FrameworkElement StateGroupsRoot { get; }
}
