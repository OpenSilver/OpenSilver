
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

using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace OpenSilver.Internal.Controls;

/// <summary>
/// Implemented by elements whose own DOM element is scrolled by the browser (CSS overflow) to
/// implement <see cref="IScrollInfo"/>, rather than by moving their content during arrange.
/// </summary>
internal interface INativeScrollInfo
{
    /// <summary>
    /// Gets whether the content is currently scrolled natively by the browser.
    /// </summary>
    bool IsNativeScrolling { get; }

    /// <summary>
    /// Gets whether the browser can currently scroll the DOM element along at least one axis
    /// (and therefore pans touches along that axis).
    /// </summary>
    bool CanNativelyScroll { get; }

    /// <summary>
    /// Gets the scroll offset currently applied to the DOM element. Visual children are
    /// rendered shifted by the opposite of this value.
    /// </summary>
    Vector NativeOffset { get; }

    /// <summary>
    /// Called when the browser has scrolled the DOM element.
    /// </summary>
    void OnNativeScroll(double scrollLeft, double scrollTop);

    /// <summary>
    /// Lets the browser scroll the DOM element for a mouse wheel event.
    /// </summary>
    void HandleNativeMouseWheel(MouseWheelEventArgs e);
}
