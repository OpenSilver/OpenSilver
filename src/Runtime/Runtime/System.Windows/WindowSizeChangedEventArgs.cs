
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

namespace System.Windows;

/// <summary>
/// Contains the argument returned by a window size change event.
/// </summary>
public sealed class WindowSizeChangedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowSizeChangedEventArgs"/> class.
    /// </summary>
    public WindowSizeChangedEventArgs() { }

    internal WindowSizeChangedEventArgs(Size size)
    {
        Size = size;
    }

    /// <summary>
    /// Gets the new size of the window.
    /// </summary>
    public Size Size { get; }

    /// <summary>
    /// Gets or sets whether the window size event was handled.
    /// </summary>
    public bool Handled { get; set; }
}