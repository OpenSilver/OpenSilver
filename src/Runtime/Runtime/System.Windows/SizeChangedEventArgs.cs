
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

using System.ComponentModel;
using System.Diagnostics;

namespace System.Windows;

/// <summary>
/// Provides data related to the <see cref="FrameworkElement.SizeChanged"/> event.
/// </summary>
public sealed class SizeChangedEventArgs : RoutedEventArgs
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public SizeChangedEventArgs(Size newSize)
    {
        NewSize = newSize;
    }

    internal SizeChangedEventArgs(SizeChangedInfo info)
    {
        Debug.Assert(info is not null);

        PreviousSize = info.PreviousSize;
        NewSize = info.NewSize;
        WidthChanged = info.WidthChanged;
        HeightChanged = info.HeightChanged;
    }

    /// <summary>
    /// Gets the new <see cref="Size"/> of the object.
    /// </summary>
    /// <returns>
    /// The new <see cref="Size"/> of the object.
    /// </returns>
    public Size NewSize { get; }

    /// <summary>
    /// Gets the previous <see cref="Size"/> of the object.
    /// </summary>
    /// <returns>
    /// The previous <see cref="Size"/> of the object.
    /// </returns>
    public Size PreviousSize { get; }

    /// <summary>
    /// Gets a value that indicates whether the <see cref="FrameworkElement.Width"/> component of the size changed.
    /// </summary>
    /// <returns>
    /// true if the <see cref="FrameworkElement.Width"/> component of the size changed; otherwise, false.
    /// </returns>
    public bool WidthChanged { get; }

    /// <summary>
    /// Gets a value that indicates whether the <see cref="FrameworkElement.Height"/> component of the size changed.
    /// </summary>
    /// <returns>
    /// true if the <see cref="FrameworkElement.Height"/> component of the size changed; otherwise, false.
    /// </returns>
    public bool HeightChanged { get; }

    /// <inheritdoc />
    protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        => ((SizeChangedEventHandler)genericHandler)(genericTarget, this);
}
