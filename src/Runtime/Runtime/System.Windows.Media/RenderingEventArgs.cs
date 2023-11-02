
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

namespace System.Windows.Media;

/// <summary>
/// Provides event data for the <see cref="CompositionTarget.Rendering"/> event.
/// </summary>
public sealed class RenderingEventArgs : EventArgs
{
    internal RenderingEventArgs(TimeSpan renderingTime)
    {
        RenderingTime = renderingTime;
    }

    /// <summary>
    /// Gets a date/time when the frame rendered.
    /// </summary>
    /// <returns>
    /// The date/time when the frame rendered.
    /// </returns>
    public TimeSpan RenderingTime { get; }
}
