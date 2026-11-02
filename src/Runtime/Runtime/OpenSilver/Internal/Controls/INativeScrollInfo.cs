
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
    /// Gets the scroll offset currently applied to the DOM element. Visual children are
    /// rendered shifted by the opposite of this value.
    /// </summary>
    Vector NativeOffset { get; }
}
